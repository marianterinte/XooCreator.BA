using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs.Models;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;
using Azure.Storage.Blobs;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class PublishStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly ILogger<PublishStoryEndpoint> _logger;
    private readonly XooCreator.BA.Features.StoryEditor.Services.IStoryPublishingService _publisher;

    public PublishStoryEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0, IBlobSasService sas, ILogger<PublishStoryEndpoint> logger, XooCreator.BA.Features.StoryEditor.Services.IStoryPublishingService publisher)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _sas = sas;
        _logger = logger;
        _publisher = publisher;
    }

    public record PublishResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "Published";
    }

    /// <summary>
    /// Strong-typed result for asset copy operations.
    /// Eliminates the need for complex casts and provides clear error information.
    /// </summary>
    private record AssetCopyResult
    {
        public bool HasError => ErrorResult != null;
        public Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>? ErrorResult { get; init; }
        public string? AssetFilename { get; init; }
        public string? ErrorMessage { get; init; }

        public static AssetCopyResult Success() => new AssetCopyResult { ErrorResult = null };
        
        public static AssetCopyResult AssetNotFound(string filename, string storyId)
        {
            return new AssetCopyResult
            {
                ErrorResult = TypedResults.BadRequest($"Draft asset not found: {filename}. StoryId: {storyId}"),
                AssetFilename = filename,
                ErrorMessage = $"Draft asset not found: {filename}"
            };
        }

        public static AssetCopyResult CopyFailed(string filename, string storyId, string reason)
        {
            return new AssetCopyResult
            {
                ErrorResult = TypedResults.BadRequest($"Failed to copy asset '{filename}': {reason}. StoryId: {storyId}"),
                AssetFilename = filename,
                ErrorMessage = $"Failed to copy asset '{filename}': {reason}"
            };
        }

        public static AssetCopyResult CopyTimeout(string filename, string storyId)
        {
            return new AssetCopyResult
            {
                ErrorResult = TypedResults.BadRequest($"Timeout while copying asset '{filename}'. StoryId: {storyId}"),
                AssetFilename = filename,
                ErrorMessage = $"Timeout while copying asset '{filename}'"
            };
        }
    }

    [Route("/api/{locale}/stories/{storyId}/publish")]
    [Authorize]
    public static async Task<Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] PublishStoryEndpoint ep,
        CancellationToken ct)
    {
        // Authorization check
        var authResult = await ep.ValidateAuthorizationAsync(ct);
        if (authResult.Result != null) return authResult.Result;
        var user = authResult.User!;

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");

        // Load craft
        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        // Validate permissions and status
        var permissionResult = ep.ValidatePublishPermissions(user, craft);
        if (permissionResult != null) return permissionResult;

        // Copy assets to published container
        var assets = StoryAssetPathMapper.ExtractAssets(craft, langTag);
        var copyResult = await ep.CopyAssetsToPublishedAsync(assets, user.Email, storyId, ct);
        if (copyResult.HasError) return copyResult.ErrorResult;

        // Finalize publishing
        await ep.FinalizePublishingAsync(craft, user.Email, langTag, assets.Count, storyId, ct);

        return TypedResults.Ok(new PublishResponse());
    }

    private async Task<(AlchimaliaUser? User, Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>? Result)> ValidateAuthorizationAsync(CancellationToken ct)
    {
        var user = await _auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return (null, TypedResults.Unauthorized());
        }

        if (!_auth0.HasRole(user, Data.Enums.UserRole.Creator))
        {
            return (null, TypedResults.Forbid());
        }

        return (user, null);
    }

    private Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>? ValidatePublishPermissions(AlchimaliaUser user, StoryCraft craft)
    {
        var isAdmin = _auth0.HasRole(user, Data.Enums.UserRole.Admin);

        if (!isAdmin && craft.OwnerUserId != user.Id)
        {
            return TypedResults.BadRequest("Only the owner can publish this story.");
        }

        var current = StoryStatusExtensions.FromDb(craft.Status);
        if (!isAdmin && current != StoryStatus.Approved)
        {
            _logger.LogWarning("Publish invalid state: storyId={StoryId} state={State}", craft.StoryId, current);
            return TypedResults.Conflict("Invalid state transition. Expected Approved.");
        }

        return null;
    }

    private async Task<AssetCopyResult> CopyAssetsToPublishedAsync(
        List<StoryAssetPathMapper.AssetInfo> assets,
        string userEmail,
        string storyId,
        CancellationToken ct)
    {
        foreach (var asset in assets)
        {
            var sourceResult = await FindSourceAssetAsync(asset, userEmail, storyId, ct);
            if (sourceResult.Result.HasError) return sourceResult.Result;

            var copyResult = await CopyAssetWithPollingAsync(
                sourceResult.SourceBlobPath!,
                asset,
                userEmail,
                storyId,
                ct);
            if (copyResult.HasError) return copyResult;
        }

        return AssetCopyResult.Success();
    }

    private async Task<(string? SourceBlobPath, AssetCopyResult Result)> FindSourceAssetAsync(
        StoryAssetPathMapper.AssetInfo asset,
        string userEmail,
        string storyId,
        CancellationToken ct)
    {
        var sourceBlobPath = StoryAssetPathMapper.BuildDraftPath(asset, userEmail, storyId);
        var sourceClient = _sas.GetBlobClient(_sas.DraftContainer, sourceBlobPath);

        if (await sourceClient.ExistsAsync(ct))
        {
            return (sourceBlobPath, AssetCopyResult.Success());
        }

        // Try fallback path for images
        if (asset.Type == StoryAssetPathMapper.AssetType.Image)
        {
            var altPath = StoryAssetPathMapper.BuildDraftPath(
                new StoryAssetPathMapper.AssetInfo(asset.Filename, asset.Type, null),
                userEmail,
                storyId);
            var altClient = _sas.GetBlobClient(_sas.DraftContainer, altPath);
            if (await altClient.ExistsAsync(ct))
            {
                return (altPath, AssetCopyResult.Success());
            }
        }

        _logger.LogWarning(
            "Publish asset not found in draft container: storyId={StoryId} filename={Filename} type={Type} userEmail={UserEmail}",
            storyId, asset.Filename, asset.Type, userEmail);
        
        return (null, AssetCopyResult.AssetNotFound(asset.Filename, storyId));
    }

    private async Task<AssetCopyResult> CopyAssetWithPollingAsync(
        string sourceBlobPath,
        StoryAssetPathMapper.AssetInfo asset,
        string userEmail,
        string storyId,
        CancellationToken ct)
    {
        try
        {
            var targetBlobPath = StoryAssetPathMapper.BuildPublishedPath(asset, userEmail, storyId);
            var sourceSas = await _sas.GetReadSasAsync(_sas.DraftContainer, sourceBlobPath, TimeSpan.FromMinutes(10), ct);

            var targetClient = _sas.GetBlobClient(_sas.PublishedContainer, targetBlobPath);
            var pollUntil = DateTime.UtcNow.AddSeconds(30);

            _logger.LogDebug(
                "Starting asset copy: storyId={StoryId} filename={Filename} source={SourcePath} target={TargetPath}",
                storyId, asset.Filename, sourceBlobPath, targetBlobPath);

            var copyOperation = await targetClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);

            while (true)
            {
                var props = await targetClient.GetPropertiesAsync(cancellationToken: ct);
                var copyStatus = props.Value.CopyStatus;

                if (copyStatus == CopyStatus.Success)
                {
                    _logger.LogDebug(
                        "Asset copy succeeded: storyId={StoryId} filename={Filename}",
                        storyId, asset.Filename);
                    break;
                }

                if (copyStatus == CopyStatus.Failed || copyStatus == CopyStatus.Aborted)
                {
                    var errorDetails = $"CopyStatus: {copyStatus}";
                    _logger.LogError(
                        "Publish copy failed: storyId={StoryId} filename={Filename} status={Status} source={SourcePath} target={TargetPath}",
                        storyId, asset.Filename, copyStatus, sourceBlobPath, targetBlobPath);
                    return AssetCopyResult.CopyFailed(asset.Filename, storyId, errorDetails);
                }

                if (DateTime.UtcNow > pollUntil)
                {
                    _logger.LogError(
                        "Publish copy timeout: storyId={StoryId} filename={Filename} source={SourcePath} target={TargetPath}",
                        storyId, asset.Filename, sourceBlobPath, targetBlobPath);
                    return AssetCopyResult.CopyTimeout(asset.Filename, storyId);
                }

                await Task.Delay(250, ct);
            }

            return AssetCopyResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Exception during asset copy: storyId={StoryId} filename={Filename} source={SourcePath}",
                storyId, asset.Filename, sourceBlobPath);
            return AssetCopyResult.CopyFailed(asset.Filename, storyId, ex.Message);
        }
    }

    private async Task FinalizePublishingAsync(
        StoryCraft craft,
        string userEmail,
        string langTag,
        int assetsCount,
        string storyId,
        CancellationToken ct)
    {
        var newVersion = await _publisher.UpsertFromCraftAsync(craft, userEmail, langTag, ct);
        craft.Status = StoryStatus.Published.ToDb();
        craft.BaseVersion = newVersion;
        await _crafts.SaveAsync(craft, ct);
        _logger.LogInformation("Published story: storyId={StoryId} assets={Count}", storyId, assetsCount);
    }
}

