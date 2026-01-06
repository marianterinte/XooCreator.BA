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
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Queue;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Infrastructure.Services.Jobs;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public partial class PublishStoryEndpoint
{
    public record PublishRequest(bool ForceFull = false);

    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<PublishStoryEndpoint> _logger;
    private readonly IStoryPublishAssetService _assetService;
    private readonly IStoryDraftAssetCleanupService _cleanupService;
    private readonly IStoryPublishQueue _queue;
    private readonly XooDbContext _db;
    private readonly TelemetryClient? _telemetryClient;
    private readonly IJobEventsHub _jobEvents;

    public PublishStoryEndpoint(
        IStoryCraftsRepository crafts, 
        IUserContextService userContext, 
        IAuth0UserService auth0, 
        ILogger<PublishStoryEndpoint> logger, 
        IStoryPublishAssetService assetService,
        IStoryDraftAssetCleanupService cleanupService,
        IStoryPublishQueue queue,
        XooDbContext db,
        IJobEventsHub jobEvents,
        TelemetryClient? telemetryClient = null)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _logger = logger;
        _assetService = assetService;
        _cleanupService = cleanupService;
        _queue = queue;
        _db = db;
        _jobEvents = jobEvents;
        _telemetryClient = telemetryClient;
    }

    [Route("/api/stories/{storyId}/publish")]
    [Authorize]
    public static async Task<Results<Accepted<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string storyId,
        [FromServices] PublishStoryEndpoint ep,
        [FromBody] PublishRequest? request,
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

            // Use first available translation or ro-ro as fallback
            langTag = craft.Translations.FirstOrDefault(t => t.LanguageCode == "ro-ro")?.LanguageCode
                ?? craft.Translations.FirstOrDefault()?.LanguageCode
                ?? "ro-ro";

            var forceFull = request?.ForceFull ?? false;

            // Create publish job
            var job = new StoryPublishJob
            {
                Id = Guid.NewGuid(),
                StoryId = craft.StoryId,
                OwnerUserId = craft.OwnerUserId,
                RequestedByEmail = user.Email ?? string.Empty,
                LangTag = langTag,
                DraftVersion = craft.LastDraftVersion,
                ForceFull = forceFull,
                Status = StoryPublishJobStatus.Queued,
                QueuedAtUtc = DateTime.UtcNow
            };

            await ep.CreatePublishJobAsync(job, ct);

            ep._jobEvents.Publish(JobTypes.StoryPublish, job.Id, new
            {
                jobId = job.Id,
                storyId = job.StoryId,
                status = job.Status.ToString(),
                queuedAtUtc = job.QueuedAtUtc,
                startedAtUtc = job.StartedAtUtc,
                completedAtUtc = job.CompletedAtUtc,
                errorMessage = job.ErrorMessage,
                dequeueCount = job.DequeueCount
            });

            await ep._queue.EnqueueAsync(job, ct);

            outcome = "Queued";
            return TypedResults.Accepted($"/api/stories/{storyId}/publish-jobs/{job.Id}", new PublishResponse { JobId = job.Id });
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
            Accepted<PublishResponse> => "Success",
            _ => result.GetType().Name
        };
    }

    private async Task<(AlchimaliaUser? User, Results<Accepted<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>? Result)> ValidateAuthorizationAsync(CancellationToken ct)
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

    private Results<Accepted<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>? ValidatePublishPermissions(AlchimaliaUser user, StoryCraft craft)
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

    [Route("/api/stories/{storyId}/publish-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<PublishJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromRoute] Guid jobId,
        [FromServices] PublishStoryEndpoint ep,
        CancellationToken ct)
    {
        // Authorization check
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        // Use AsNoTracking for read-only query to improve performance
        var job = await ep._db.StoryPublishJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.StoryId == storyId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        var response = new PublishJobStatusResponse
        {
            JobId = job.Id,
            StoryId = job.StoryId,
            Status = job.Status.ToString(),
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            DequeueCount = job.DequeueCount
        };

        return TypedResults.Ok(response);
    }

    private async Task CreatePublishJobAsync(StoryPublishJob job, CancellationToken ct)
    {
        // Mark any queued/running jobs for same story as superseded
        var existingJobs = await _db.StoryPublishJobs
            .Where(j => j.StoryId == job.StoryId && (j.Status == StoryPublishJobStatus.Queued || j.Status == StoryPublishJobStatus.Running))
            .ToListAsync(ct);

        foreach (var existing in existingJobs)
        {
            existing.Status = StoryPublishJobStatus.Superseded;
            existing.CompletedAtUtc = DateTime.UtcNow;
        }

        _db.StoryPublishJobs.Add(job);
        await _db.SaveChangesAsync(ct);
    }
}

public record PublishJobStatusResponse
{
    public Guid JobId { get; init; }
    public string StoryId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime QueuedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
    public int DequeueCount { get; init; }
}

