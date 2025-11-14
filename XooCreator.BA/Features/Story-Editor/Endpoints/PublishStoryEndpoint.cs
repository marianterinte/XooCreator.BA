using Microsoft.AspNetCore.Authorization;
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
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        var isAdmin = ep._auth0.HasRole(user, Data.Enums.UserRole.Admin);

        if (!isAdmin && craft.OwnerUserId != user.Id)
        {
            return TypedResults.BadRequest("Only the owner can publish this story.");
        }

        var current = StoryStatusExtensions.FromDb(craft.Status);
        if (!isAdmin && current != StoryStatus.Approved)
        {
            ep._logger.LogWarning("Publish invalid state: storyId={StoryId} state={State}", storyId, current);
            return TypedResults.Conflict("Invalid state transition. Expected Approved.");
        }

        var assets = StoryAssetPathMapper.ExtractAssets(craft, langTag);

        foreach (var asset in assets)
        {
            var sourceBlobPath = StoryAssetPathMapper.BuildDraftPath(asset, user.Email, storyId);
            var sourceClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, sourceBlobPath);

            if (!await sourceClient.ExistsAsync(ct))
            {
                if (asset.Type == StoryAssetPathMapper.AssetType.Image)
                {
                    var altPath = StoryAssetPathMapper.BuildDraftPath(
                        new StoryAssetPathMapper.AssetInfo(asset.Filename, asset.Type, null),
                        user.Email,
                        storyId);
                    var altClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, altPath);
                    if (await altClient.ExistsAsync(ct))
                    {
                        sourceBlobPath = altPath;
                        sourceClient = altClient;
                    }
                    else
                    {
                        ep._logger.LogWarning("Publish asset not found in draft container: storyId={StoryId} filename={Filename}", storyId, asset.Filename);
                        return TypedResults.BadRequest($"Draft asset not found: {asset.Filename}");
                    }
                }
                else
                {
                    ep._logger.LogWarning("Publish asset not found in draft container: storyId={StoryId} filename={Filename}", storyId, asset.Filename);
                    return TypedResults.BadRequest($"Draft asset not found: {asset.Filename}");
                }
            }

            var targetBlobPath = StoryAssetPathMapper.BuildPublishedPath(asset, user.Email, storyId);
            var sourceSas = await ep._sas.GetReadSasAsync(ep._sas.DraftContainer, sourceBlobPath, TimeSpan.FromMinutes(10), ct);

            var targetClient = ep._sas.GetBlobClient(ep._sas.PublishedContainer, targetBlobPath);
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
                    ep._logger.LogError("Publish copy failed: storyId={StoryId} filename={Filename}", storyId, asset.Filename);
                    return TypedResults.BadRequest($"Failed to copy asset: {asset.Filename}");
                }
                if (DateTime.UtcNow > pollUntil)
                {
                    ep._logger.LogError("Publish copy timeout: storyId={StoryId} filename={Filename}", storyId, asset.Filename);
                    return TypedResults.BadRequest($"Timeout while copying asset: {asset.Filename}");
                }
                await Task.Delay(250, ct);
            }
        }

        var newVersion = await ep._publisher.UpsertFromCraftAsync(craft, user.Email, langTag, ct);
        craft.Status = StoryStatus.Published.ToDb();
        craft.BaseVersion = newVersion;
        await ep._crafts.SaveAsync(craft, ct);
        ep._logger.LogInformation("Published story: storyId={StoryId} assets={Count}", storyId, assets.Count);
        return TypedResults.Ok(new PublishResponse());
    }
}

