using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs.Models;
using System.Text.Json;
using System.Collections.Generic;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
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

        // 1) Extract referenced asset relative paths from craft structure
        var relPaths = ExtractAssetRelPaths(craft);

        // 2) Copy assets: drafts -> published
        var emailEsc = Uri.EscapeDataString(user.Email);
        foreach (var rel in relPaths)
        {
            var category = rel.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)
                ? "audio"
                : rel.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
                    ? "video"
                    : "images";

            var normalizedRel = rel.TrimStart('/');
            var baseDraftPath = $"draft/u/{emailEsc}/stories/{storyId}";
            var sourcePathCandidates = new List<string>();
            // Images (cover and tiles) are language-agnostic; audio and video are language-specific
            var isImageAsset = rel.StartsWith("tiles/", StringComparison.OrdinalIgnoreCase) 
                || rel.StartsWith("cover/", StringComparison.OrdinalIgnoreCase);

            if (isImageAsset)
            {
                // New structure (language-agnostic) first, then legacy with language segment
                sourcePathCandidates.Add($"{baseDraftPath}/{normalizedRel}");
                sourcePathCandidates.Add($"{baseDraftPath}/{langTag}/{normalizedRel}");
            }
            else
            {
                sourcePathCandidates.Add($"{baseDraftPath}/{langTag}/{normalizedRel}");
            }

            string? sourceBlobPath = null;
            BlobClient? sourceClient = null;
            foreach (var candidate in sourcePathCandidates)
            {
                var candidateClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, candidate);
                if (await candidateClient.ExistsAsync(ct))
                {
                    sourceBlobPath = candidate;
                    sourceClient = candidateClient;
                    break;
                }
            }

            if (sourceBlobPath is null || sourceClient is null)
            {
                ep._logger.LogWarning("Publish asset not found in draft container: storyId={StoryId} rel={Rel}", storyId, rel);
                return TypedResults.BadRequest($"Draft asset not found: {rel}");
            }

            // New published structure:
            // images: images/tales-of-alchimalia/stories/{email}/{storyId}/{fileName}
            // video:  video/tales-of-alchimalia/stories/{email}/{storyId}/{fileName}
            // audio:  audio/tales-of-alchimalia/stories/{email}/{storyId}/{lang}/{fileName}
            // - No 'cover' or 'tiles' folders in published
            // - Images and videos are language-agnostic; audio is language-specific
            var owner = user.Email; // keep '@', avoid % encoding
            var publishedRoot = $"{category}/tales-of-alchimalia/stories/{owner}/{storyId}";
            if (string.Equals(category, "audio", StringComparison.OrdinalIgnoreCase))
            {
                publishedRoot = $"{publishedRoot}/{langTag}";
            }

            var targetFileName = ComputePublishedFileName(rel);
            var targetBlobPath = $"{publishedRoot}/{targetFileName}";

            // Issue short-lived read SAS for the source
            var sourceSas = await ep._sas.GetReadSasAsync(ep._sas.DraftContainer, sourceBlobPath, TimeSpan.FromMinutes(10), ct);

            var targetClient = ep._sas.GetBlobClient(ep._sas.PublishedContainer, targetBlobPath);
            var pollUntil = DateTime.UtcNow.AddSeconds(30);

            var _ = await targetClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);

            // Poll copy status (simple bounded loop)
            while (true)
            {
                var props = await targetClient.GetPropertiesAsync(cancellationToken: ct);
                if (props.Value.CopyStatus == CopyStatus.Success)
                {
                    break;
                }
                if (props.Value.CopyStatus == CopyStatus.Failed || props.Value.CopyStatus == CopyStatus.Aborted)
                {
                    ep._logger.LogError("Publish copy failed: storyId={StoryId} rel={Rel}", storyId, rel);
                    return TypedResults.BadRequest($"Failed to copy asset: {rel}");
                }
                if (DateTime.UtcNow > pollUntil)
                {
                    ep._logger.LogError("Publish copy timeout: storyId={StoryId} rel={Rel}", storyId, rel);
                    return TypedResults.BadRequest($"Timeout while copying asset: {rel}");
                }
                await Task.Delay(250, ct);
            }

            // Delete source after successful copy
            try { await sourceClient.DeleteIfExistsAsync(cancellationToken: ct); ep._logger.LogInformation("Deleted draft asset: {Path}", sourceBlobPath); } catch { /* best-effort */ }
        }

        // 3) Upsert StoryDefinition from craft (single source of truth for marketplace), bump version
        var newVersion = await ep._publisher.UpsertFromCraftAsync(craft, user.Email, langTag, ct);

        // 4) Mark craft as published and record base version
        craft.Status = StoryStatus.Published.ToDb();
        craft.BaseVersion = newVersion;
        await ep._crafts.SaveAsync(craft, ct);
        ep._logger.LogInformation("Published story: storyId={StoryId} assets={Count}", storyId, relPaths.Count);
        return TypedResults.Ok(new PublishResponse());
    }

    private static string ComputePublishedFileName(string relPath)
    {
        // Examples of incoming relPath (from craft JSON):
        // - cover/0.cover.png            -> cover.png
        // - tiles/p1/bg.webp             -> p1.webp
        // - audio/p3/intro.m4a           -> p3.m4a   (audio is lang-specific in target but filename rule same)
        // - video/p2/intro.mp4           -> p2.mp4
        // Fallback: last segment if pattern unknown
        try
        {
            var parts = relPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0) return Path.GetFileName(relPath);

            var ext = Path.GetExtension(parts[^1]);
            if (parts[0].Equals("cover", StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrWhiteSpace(ext) ? "cover" : $"cover{ext}";
            }

            if (parts.Length >= 2 && (parts[0].Equals("tiles", StringComparison.OrdinalIgnoreCase)
                || parts[0].Equals("audio", StringComparison.OrdinalIgnoreCase)
                || parts[0].Equals("video", StringComparison.OrdinalIgnoreCase)))
            {
                var tileId = parts[1];
                return string.IsNullOrWhiteSpace(ext) ? tileId : $"{tileId}{ext}";
            }

            // Default to last segment
            return parts[^1];
        }
        catch
        {
            return Path.GetFileName(relPath);
        }
    }

    private static List<string> ExtractAssetRelPaths(StoryCraft craft)
    {
        var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        // Extract cover image
        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var coverPath = craft.CoverImageUrl;
            if (coverPath.StartsWith("cover/", StringComparison.OrdinalIgnoreCase) ||
                coverPath.StartsWith("tiles/", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(coverPath);
            }
        }
        
        // Extract tile assets
        foreach (var tile in craft.Tiles)
        {
            if (!string.IsNullOrWhiteSpace(tile.ImageUrl))
            {
                var imgPath = tile.ImageUrl;
                if (imgPath.StartsWith("tiles/", StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(imgPath);
                }
            }
            
            if (!string.IsNullOrWhiteSpace(tile.AudioUrl))
            {
                var audioPath = tile.AudioUrl;
                if (audioPath.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(audioPath);
                }
            }
            
            if (!string.IsNullOrWhiteSpace(tile.VideoUrl))
            {
                var videoPath = tile.VideoUrl;
                if (videoPath.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(videoPath);
                }
            }
        }
        
        return results.ToList();
    }
}

