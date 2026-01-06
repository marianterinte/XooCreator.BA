using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.DTOs;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ExportPublishedStoryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IUserContextService _userContext;
    private readonly IBlobSasService _sas;
    private readonly ILogger<ExportPublishedStoryEndpoint> _logger;
    private readonly TelemetryClient? _telemetryClient;
    private readonly IStoryExportQueue _queue;
    private readonly IJobEventsHub _jobEvents;

    public ExportPublishedStoryEndpoint(
        XooDbContext db, 
        IAuth0UserService auth0, 
        IUserContextService userContext, 
        IBlobSasService sas,
        ILogger<ExportPublishedStoryEndpoint> logger,
        IStoryExportQueue queue,
        IJobEventsHub jobEvents,
        TelemetryClient? telemetryClient = null)
    {
        _db = db;
        _auth0 = auth0;
        _userContext = userContext;
        _sas = sas;
        _logger = logger;
        _queue = queue;
        _jobEvents = jobEvents;
        _telemetryClient = telemetryClient;
    }

    [Route("/api/{locale}/stories/{storyId}/export")]
    [Authorize]
    public static async Task<Results<Accepted<ExportResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ExportPublishedStoryEndpoint ep,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var outcome = "Unknown";
        string? userId = null;
        string? userEmail = null;
        var isAdmin = false;
        var isCreator = false;
        var isOwner = false;

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            outcome = "Unauthorized";
            return TypedResults.Unauthorized();
        }
        userId = user.Id.ToString();
        userEmail = user.Email;

        // Allow Creator (owner) or Admin to export
        isAdmin = ep._auth0.HasRole(user, Data.Enums.UserRole.Admin);
        isCreator = ep._auth0.HasRole(user, Data.Enums.UserRole.Creator);
        
        if (!isAdmin && !isCreator)
        {
            outcome = "Forbidden";
            return TypedResults.Forbid();
        }

        var def = await ep._db.StoryDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);
        if (def == null)
        {
            outcome = "NotFound";
            return TypedResults.NotFound();
        }

        isOwner = def.CreatedBy.HasValue && def.CreatedBy.Value == user.Id;

        // If not Admin, verify ownership
        if (!isAdmin)
        {
            if (!isOwner)
            {
                ep._logger.LogWarning("Export forbidden: userId={UserId} storyId={StoryId} not owner", user.Id, storyId);
                outcome = "Forbidden";
                return TypedResults.Forbid();
            }
        }

        try
        {
            // Create export job
            var job = new StoryExportJob
            {
                Id = Guid.NewGuid(),
                StoryId = storyId,
                OwnerUserId = def.CreatedBy ?? user.Id,
                RequestedByEmail = user.Email ?? string.Empty,
                Locale = locale,
                IsDraft = false,
                Status = StoryExportJobStatus.Queued,
                QueuedAtUtc = DateTime.UtcNow
            };

            ep._db.StoryExportJobs.Add(job);
            await ep._db.SaveChangesAsync(ct);

            ep._jobEvents.Publish(JobTypes.StoryExport, job.Id, new
            {
                jobId = job.Id,
                storyId = job.StoryId,
                status = job.Status,
                queuedAtUtc = job.QueuedAtUtc,
                startedAtUtc = job.StartedAtUtc,
                completedAtUtc = job.CompletedAtUtc,
                errorMessage = job.ErrorMessage,
                zipDownloadUrl = (string?)null,
                zipFileName = job.ZipFileName,
                zipSizeBytes = job.ZipSizeBytes,
                mediaCount = job.MediaCount,
                languageCount = job.LanguageCount
            });

            // Enqueue job
            await ep._queue.EnqueueAsync(job, ct);

            outcome = "Queued";
            ep._logger.LogInformation("Export job queued: jobId={JobId} storyId={StoryId} isDraft={IsDraft}",
                job.Id, storyId, false);

            return TypedResults.Accepted($"/api/stories/{storyId}/export-jobs/{job.Id}", new ExportResponse { JobId = job.Id });
        }
        finally
        {
            stopwatch.Stop();
            ep.TrackExportTelemetry(
                isDraft: false,
                durationMs: stopwatch.ElapsedMilliseconds,
                outcome: outcome,
                locale: locale,
                storyId: storyId,
                userId: userId,
                userEmail: userEmail,
                isAdmin: isAdmin,
                isCreator: isCreator,
                isOwner: isOwner);
        }
    }

    private void TrackExportTelemetry(
        bool isDraft,
        long durationMs,
        string outcome,
        string locale,
        string storyId,
        string? userId,
        string? userEmail,
        bool isAdmin,
        bool isCreator,
        bool isOwner)
    {
        _logger.LogInformation(
            "Export story telemetry | storyId={StoryId} locale={Locale} outcome={Outcome} durationMs={DurationMs} isDraft={IsDraft} isAdmin={IsAdmin} isCreator={IsCreator} isOwner={IsOwner}",
            storyId,
            locale,
            outcome,
            durationMs,
            isDraft,
            isAdmin,
            isCreator,
            isOwner);

        var properties = new Dictionary<string, string?>
        {
            ["IsDraft"] = isDraft.ToString(CultureInfo.InvariantCulture),
            ["Outcome"] = outcome,
            ["Locale"] = locale,
            ["StoryId"] = storyId,
            ["UserId"] = userId,
            ["UserEmail"] = userEmail,
            ["IsAdmin"] = isAdmin.ToString(CultureInfo.InvariantCulture),
            ["IsCreator"] = isCreator.ToString(CultureInfo.InvariantCulture),
            ["IsOwner"] = isOwner.ToString(CultureInfo.InvariantCulture)
        };

        TrackMetric("ExportStory_QueueDuration", durationMs, properties);
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
}
