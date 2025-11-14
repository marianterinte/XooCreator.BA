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
        if (authResult.Result != null) return (Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>)authResult.Result;
        var user = authResult.User!;

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");

        // Load craft
        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        // Validate permissions and status
        var permissionResult = ep.ValidatePublishPermissions(user, craft);
        if (permissionResult != null) return (Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>)permissionResult;

        // Copy assets to published container
        var assets = StoryAssetPathMapper.ExtractAssets(craft, langTag);
        var copyResult = await ep.CopyAssetsToPublishedAsync(assets, user.Email, storyId, ct);
        if (copyResult != null) return (Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>)copyResult;

        // Finalize publishing
        await ep.FinalizePublishingAsync(craft, user.Email, langTag, assets.Count, storyId, ct);

        return TypedResults.Ok(new PublishResponse());
    }

    private async Task<(AlchimaliaUser? User, IResult? Result)> ValidateAuthorizationAsync(CancellationToken ct)
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

    private IResult? ValidatePublishPermissions(AlchimaliaUser user, StoryCraft craft)
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

    private async Task<IResult?> CopyAssetsToPublishedAsync(
        List<StoryAssetPathMapper.AssetInfo> assets,
        string userEmail,
        string storyId,
        CancellationToken ct)
    {
        foreach (var asset in assets)
        {
            var sourceResult = await FindSourceAssetAsync(asset, userEmail, storyId, ct);
            if (sourceResult.Error != null) return sourceResult.Error;

            var copyResult = await CopyAssetWithPollingAsync(
                sourceResult.SourceBlobPath!,
                asset,
                userEmail,
                storyId,
                ct);
            if (copyResult != null) return copyResult;
        }

        return null;
    }

    private async Task<(string? SourceBlobPath, IResult? Error)> FindSourceAssetAsync(
        StoryAssetPathMapper.AssetInfo asset,
        string userEmail,
        string storyId,
        CancellationToken ct)
    {
        var sourceBlobPath = StoryAssetPathMapper.BuildDraftPath(asset, userEmail, storyId);
        var sourceClient = _sas.GetBlobClient(_sas.DraftContainer, sourceBlobPath);

        if (await sourceClient.ExistsAsync(ct))
        {
            return (sourceBlobPath, null);
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
                return (altPath, null);
            }
        }

        _logger.LogWarning("Publish asset not found in draft container: storyId={StoryId} filename={Filename}", storyId, asset.Filename);
        return (null, TypedResults.BadRequest($"Draft asset not found: {asset.Filename}"));
    }

    private async Task<IResult?> CopyAssetWithPollingAsync(
        string sourceBlobPath,
        StoryAssetPathMapper.AssetInfo asset,
        string userEmail,
        string storyId,
        CancellationToken ct)
    {
        var targetBlobPath = StoryAssetPathMapper.BuildPublishedPath(asset, userEmail, storyId);
        var sourceSas = await _sas.GetReadSasAsync(_sas.DraftContainer, sourceBlobPath, TimeSpan.FromMinutes(10), ct);

        var targetClient = _sas.GetBlobClient(_sas.PublishedContainer, targetBlobPath);
        var pollUntil = DateTime.UtcNow.AddSeconds(30);

        var _ = await targetClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);

        while (true)
        {
            var props = await targetClient.GetPropertiesAsync(cancellationToken: ct);
            if (props.Value.CopyStatus == CopyStatus.Success)
            {
                break;
            }
            if (props.Value.CopyStatus == CopyStatus.Failed || props.Value.CopyStatus == CopyStatus.Aborted)
            {
                _logger.LogError("Publish copy failed: storyId={StoryId} filename={Filename}", storyId, asset.Filename);
                return TypedResults.BadRequest($"Failed to copy asset: {asset.Filename}");
            }
            if (DateTime.UtcNow > pollUntil)
            {
                _logger.LogError("Publish copy timeout: storyId={StoryId} filename={Filename}", storyId, asset.Filename);
                return TypedResults.BadRequest($"Timeout while copying asset: {asset.Filename}");
            }
            await Task.Delay(250, ct);
        }

        return null;
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

