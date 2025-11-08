using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs.Models;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class PublishStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly ILogger<PublishStoryEndpoint> _logger;

    public PublishStoryEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0, IBlobSasService sas, ILogger<PublishStoryEndpoint> logger)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _sas = sas;
        _logger = logger;
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
        var lang = LanguageCodeExtensions.FromTag(langTag);

        var craft = await ep._crafts.GetAsync(storyId, lang, ct);
        if (craft == null) return TypedResults.NotFound();

        if (craft.OwnerUserId != user.Id)
        {
            return TypedResults.BadRequest("Only the owner can publish this story.");
        }

        var current = StoryStatusExtensions.FromDb(craft.Status);
        if (current != StoryStatus.Approved)
        {
            ep._logger.LogWarning("Publish invalid state: storyId={StoryId} state={State}", storyId, current);
            return TypedResults.Conflict("Invalid state transition. Expected Approved.");
        }

        // 1) Extract referenced asset relative paths from draft JSON
        var relPaths = ExtractAssetRelPaths(craft.Json);

        // 2) Copy assets: drafts -> published
        var emailEsc = Uri.EscapeDataString(user.Email);
        foreach (var rel in relPaths)
        {
            var category = rel.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)
                ? "audio"
                : rel.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
                    ? "video"
                    : "images";

            var sourceBlobPath = $"draft/u/{emailEsc}/stories/{storyId}/{langTag}/{rel}";
            var targetBlobPath = $"{category}/stories/{storyId}/{langTag}/{rel}";

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
            var sourceClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, sourceBlobPath);
            try { await sourceClient.DeleteIfExistsAsync(cancellationToken: ct); ep._logger.LogInformation("Deleted draft asset: {Path}", sourceBlobPath); } catch { /* best-effort */ }
        }

        // 3) Mark as published (keep JSON as-is)
        craft.Status = StoryStatus.Published.ToDb();
        await ep._crafts.SaveAsync(craft, ct);
        ep._logger.LogInformation("Published story: storyId={StoryId} lang={Lang} assets={Count}", storyId, langTag, relPaths.Count);
        return TypedResults.Ok(new PublishResponse());
    }

    private static List<string> ExtractAssetRelPaths(string json)
    {
        var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(json)) return results.ToList();

        using var doc = JsonDocument.Parse(json);

        void Walk(JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in el.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.String)
                        {
                            var val = prop.Value.GetString() ?? string.Empty;
                            if (!string.IsNullOrWhiteSpace(val) &&
                                (val.StartsWith("cover/", StringComparison.OrdinalIgnoreCase) ||
                                 val.StartsWith("tiles/", StringComparison.OrdinalIgnoreCase) ||
                                 val.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ||
                                 val.StartsWith("video/", StringComparison.OrdinalIgnoreCase)))
                            {
                                results.Add(val);
                            }
                        }
                        else
                        {
                            Walk(prop.Value);
                        }
                    }
                    break;
                case JsonValueKind.Array:
                    foreach (var item in el.EnumerateArray())
                    {
                        Walk(item);
                    }
                    break;
            }
        }

        Walk(doc.RootElement);
        return results.ToList();
    }
}

