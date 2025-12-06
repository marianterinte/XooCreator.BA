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
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.DTOs;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ExportDraftStoryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IUserContextService _userContext;
    private readonly IBlobSasService _sas;
    private readonly IStoryCraftsRepository _crafts;
    private readonly ILogger<ExportDraftStoryEndpoint> _logger;
    private readonly TelemetryClient? _telemetryClient;
    private readonly IStoryExportQueue _queue;

    public ExportDraftStoryEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IUserContextService userContext,
        IBlobSasService sas,
        IStoryCraftsRepository crafts,
        ILogger<ExportDraftStoryEndpoint> logger,
        IStoryExportQueue queue,
        TelemetryClient? telemetryClient = null)
    {
        _db = db;
        _auth0 = auth0;
        _userContext = userContext;
        _sas = sas;
        _crafts = crafts;
        _logger = logger;
        _queue = queue;
        _telemetryClient = telemetryClient;
    }

    [Route("/api/{locale}/stories/{storyId}/export-draft")]
    [Authorize]
    public static async Task<Results<Accepted<ExportResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ExportDraftStoryEndpoint ep,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var outcome = "Unknown";
        AlchimaliaUser? currentUser = null;
        string? userId = null;
        string? userEmail = null;
        var isAdmin = false;
        var isCreator = false;
        var isOwner = false;

        try
        {
            currentUser = await ep._auth0.GetCurrentUserAsync(ct);
            if (currentUser == null)
            {
                outcome = "Unauthorized";
                return TypedResults.Unauthorized();
            }

            userId = currentUser.Id.ToString();
            userEmail = currentUser.Email;

            // Allow Creator (owner) or Admin to export
            isAdmin = ep._auth0.HasRole(currentUser, UserRole.Admin);
            isCreator = ep._auth0.HasRole(currentUser, UserRole.Creator);

            if (!isAdmin && !isCreator)
            {
                outcome = "Forbidden";
                return TypedResults.Forbid();
            }

            var craft = await ep._crafts.GetAsync(storyId, ct);
            if (craft == null)
            {
                outcome = "NotFound";
                return TypedResults.NotFound();
            }

            isOwner = craft.OwnerUserId == currentUser.Id;

            // If not Admin, verify ownership
            if (!isAdmin && !isOwner)
            {
                ep._logger.LogWarning("Export draft forbidden: userId={UserId} storyId={StoryId} not owner", currentUser.Id, storyId);
                outcome = "Forbidden";
                return TypedResults.Forbid();
            }

            // Create export job
            var job = new StoryExportJob
            {
                Id = Guid.NewGuid(),
                StoryId = storyId,
                OwnerUserId = craft.OwnerUserId,
                RequestedByEmail = currentUser.Email ?? string.Empty,
                Locale = locale,
                IsDraft = true,
                Status = StoryExportJobStatus.Queued,
                QueuedAtUtc = DateTime.UtcNow
            };

            ep._db.StoryExportJobs.Add(job);
            await ep._db.SaveChangesAsync(ct);

            // Enqueue job
            await ep._queue.EnqueueAsync(job, ct);

            outcome = "Queued";
            ep._logger.LogInformation("Export draft job queued: jobId={JobId} storyId={StoryId} isDraft={IsDraft}",
                job.Id, storyId, true);

            return TypedResults.Accepted($"/api/stories/{storyId}/export-jobs/{job.Id}", new ExportResponse { JobId = job.Id });
        }
        finally
        {
            stopwatch.Stop();
            ep.TrackExportTelemetry(
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
            "Export draft telemetry | storyId={StoryId} locale={Locale} outcome={Outcome} durationMs={DurationMs} isAdmin={IsAdmin} isCreator={IsCreator} isOwner={IsOwner}",
            storyId,
            locale,
            outcome,
            durationMs,
            isAdmin,
            isCreator,
            isOwner);

        var properties = new Dictionary<string, string?>
        {
            ["IsDraft"] = bool.TrueString,
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
