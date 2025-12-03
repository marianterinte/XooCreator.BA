using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public partial class PublishStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<PublishStoryEndpoint> _logger;
    private readonly IStoryPublishingService _publisher;
    private readonly IStoryPublishAssetService _assetService;
    private readonly IStoryDraftAssetCleanupService _cleanupService;
    private readonly TelemetryClient? _telemetryClient;

    public PublishStoryEndpoint(
        IStoryCraftsRepository crafts, 
        IUserContextService userContext, 
        IAuth0UserService auth0, 
        ILogger<PublishStoryEndpoint> logger, 
        IStoryPublishingService publisher,
        IStoryPublishAssetService assetService,
        IStoryDraftAssetCleanupService cleanupService,
        TelemetryClient? telemetryClient = null)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _logger = logger;
        _publisher = publisher;
        _assetService = assetService;
        _cleanupService = cleanupService;
        _telemetryClient = telemetryClient;
    }

    [Route("/api/stories/{storyId}/publish")]
    [Authorize]
    public static async Task<Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string storyId,
        [FromServices] PublishStoryEndpoint ep,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var outcome = "Unknown";
        string? userId = null;
        string? userEmail = null;
        int assetsToCopy = 0;
        var assetCopySuccess = false;
        string? copyFailureAsset = null;
        string? copyFailureReason = null;
        int translationCount = 0;
        int tileCount = 0;
        int audioAssetCount = 0;
        int videoAssetCount = 0;
        int newVersion = 0;
        string? langTag = null;

        try
        {
            // Authorization check
            var authResult = await ep.ValidateAuthorizationAsync(ct);
            if (authResult.Result != null)
            {
                outcome = MapResultOutcome(authResult.Result);
                return authResult.Result;
            }

            var user = authResult.User!;
            userId = user.Id.ToString();
            userEmail = user.Email;

            // Load craft
            var craft = await ep._crafts.GetAsync(storyId, ct);
            if (craft == null)
            {
                outcome = "NotFound";
                return TypedResults.NotFound();
            }

            translationCount = craft.Translations.Count;
            tileCount = craft.Tiles.Count;

            // Validate permissions and status
            var permissionResult = ep.ValidatePublishPermissions(user, craft);
            if (permissionResult != null)
            {
                outcome = MapResultOutcome(permissionResult);
                return permissionResult;
            }

            // Collect assets for all available languages
            // Images are common (extract once), Audio/Video sunt pe limbă
            var allAssets = ep._assetService.CollectAllAssets(craft);
            assetsToCopy = allAssets.Count;
            audioAssetCount = allAssets.Count(a => a.Type == XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper.AssetType.Audio);
            videoAssetCount = allAssets.Count(a => a.Type == XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper.AssetType.Video);

            var copyResult = await ep._assetService.CopyAssetsToPublishedAsync(allAssets, user.Email, storyId, ct);
            if (copyResult.HasError)
            {
                assetCopySuccess = false;
                copyFailureAsset = copyResult.AssetFilename;
                copyFailureReason = copyResult.ErrorMessage;
                if (copyResult.ErrorResult != null)
                {
                    outcome = MapResultOutcome(copyResult.ErrorResult);
                    return copyResult.ErrorResult;
                }

                outcome = "AssetCopyFailed";
                return TypedResults.BadRequest("Asset copy failed.");
            }

            assetCopySuccess = true;

            // Use first available translation or ro-ro as fallback for title/summary (publish processes all translations anyway)
            langTag = craft.Translations.FirstOrDefault(t => t.LanguageCode == "ro-ro")?.LanguageCode
                ?? craft.Translations.FirstOrDefault()?.LanguageCode
                ?? "ro-ro";

            // Finalize publishing
            newVersion = await ep.FinalizePublishingAsync(craft, user.Email, langTag, allAssets.Count, storyId, ct);

            outcome = "Success";
            return TypedResults.Ok(new PublishResponse());
        }
        finally
        {
            stopwatch.Stop();
            ep.TrackPublishTelemetry(
                stopwatch.ElapsedMilliseconds,
                outcome,
                storyId,
                userId,
                userEmail,
                assetsToCopy,
                assetCopySuccess,
                copyFailureAsset,
                copyFailureReason,
                newVersion,
                translationCount,
                tileCount,
                audioAssetCount,
                videoAssetCount,
                langTag);
        }
    }

    private void TrackPublishTelemetry(
        long durationMs,
        string outcome,
        string storyId,
        string? userId,
        string? userEmail,
        int assetsToCopy,
        bool assetCopySuccess,
        string? copyFailureAsset,
        string? copyFailureReason,
        int newVersion,
        int translationCount,
        int tileCount,
        int audioAssetCount,
        int videoAssetCount,
        string? languageTag)
    {
        _logger.LogInformation(
            "Publish story telemetry | storyId={StoryId} outcome={Outcome} durationMs={DurationMs} assets={Assets} copySuccess={CopySuccess} newVersion={NewVersion} translations={Translations} tiles={Tiles} audioAssets={AudioAssets} videoAssets={VideoAssets} langTag={LangTag}",
            storyId,
            outcome,
            durationMs,
            assetsToCopy,
            assetCopySuccess,
            newVersion,
            translationCount,
            tileCount,
            audioAssetCount,
            videoAssetCount,
            languageTag ?? "(none)");

        var properties = new Dictionary<string, string?>
        {
            ["Outcome"] = outcome,
            ["StoryId"] = storyId,
            ["UserId"] = userId,
            ["UserEmail"] = userEmail,
            ["AssetCopySuccess"] = assetCopySuccess.ToString(CultureInfo.InvariantCulture),
            ["CopyFailureAsset"] = copyFailureAsset,
            ["CopyFailureReason"] = copyFailureReason,
            ["NewVersion"] = newVersion > 0 ? newVersion.ToString(CultureInfo.InvariantCulture) : null,
            ["Translations"] = translationCount.ToString(CultureInfo.InvariantCulture),
            ["Tiles"] = tileCount.ToString(CultureInfo.InvariantCulture),
            ["AudioAssets"] = audioAssetCount.ToString(CultureInfo.InvariantCulture),
            ["VideoAssets"] = videoAssetCount.ToString(CultureInfo.InvariantCulture),
            ["LangTag"] = languageTag,
            ["AssetsToCopy"] = assetsToCopy.ToString(CultureInfo.InvariantCulture)
        };

        TrackMetric("PublishStory_Duration", durationMs, properties);
        TrackMetric("PublishStory_AssetsCopied", assetsToCopy, properties);
        TrackMetric("PublishStory_CopySuccess", assetCopySuccess ? 1 : 0, properties);

        if (newVersion > 0)
        {
            TrackMetric("PublishStory_NewVersion", newVersion, properties);
        }
    }

    private void TrackMetric(string metricName, double value, IReadOnlyDictionary<string, string?> properties)
    {
        if (_telemetryClient == null)
        {
            return;
        }

        var metric = new MetricTelemetry(metricName, value);
        foreach (var kvp in properties)
        {
            if (!string.IsNullOrEmpty(kvp.Value))
            {
                metric.Properties[kvp.Key] = kvp.Value!;
            }
        }

        _telemetryClient.TrackMetric(metric);
    }

    private static string MapResultOutcome(IResult result)
    {
        return result switch
        {
            UnauthorizedHttpResult => "Unauthorized",
            ForbidHttpResult => "Forbidden",
            NotFound => "NotFound",
            BadRequest<string> => "BadRequest",
            Conflict<string> => "Conflict",
            Ok<PublishResponse> => "Success",
            _ => result.GetType().Name
        };
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

    private async Task<int> FinalizePublishingAsync(
        StoryCraft craft,
        string userEmail,
        string langTag,
        int assetsCount,
        string storyId,
        CancellationToken ct)
    {
        var newVersion = await _publisher.UpsertFromCraftAsync(craft, userEmail, langTag, ct);

        await _cleanupService.DeleteDraftAssetsAsync(userEmail, storyId, ct);
        await _crafts.DeleteAsync(storyId, ct);

        _logger.LogInformation(
            "Published story: storyId={StoryId} version={Version} assets={Count} draftDeleted=true",
            storyId,
            newVersion,
            assetsCount);

        return newVersion;
    }
}

