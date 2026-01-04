using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class ExportEpicEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IEpicExportQueueService _queueService;
    private readonly ILogger<ExportEpicEndpoint> _logger;

    public ExportEpicEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IEpicExportQueueService queueService,
        ILogger<ExportEpicEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _queueService = queueService;
        _logger = logger;
    }

    [Route("/api/story-editor/epics/{epicId}/export")]
    [Authorize]
    public static async Task<Results<Accepted<EpicExportResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromBody] EpicExportRequest? request,
        [FromServices] ExportEpicEndpoint ep,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var outcome = "Unknown";

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            outcome = "Unauthorized";
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);

        if (!isAdmin && !isCreator)
        {
            outcome = "Forbidden";
            return TypedResults.Forbid();
        }

        // Check if published epic exists
        var definition = await ep._db.StoryEpicDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == epicId, ct);

        if (definition == null)
        {
            outcome = "NotFound";
            return TypedResults.NotFound();
        }

        var isOwner = definition.OwnerUserId == user.Id;

        // If not Admin, verify ownership
        if (!isAdmin && !isOwner)
        {
            ep._logger.LogWarning("Export forbidden: userId={UserId} epicId={EpicId} not owner", user.Id, epicId);
            outcome = "Forbidden";
            return TypedResults.Forbid();
        }

        try
        {
            var options = request ?? new EpicExportRequest();

            // Enqueue export job
            var jobId = await ep._queueService.EnqueueExportAsync(
                epicId,
                options,
                isDraft: false,
                user.Id,
                user.Email ?? string.Empty,
                "ro-ro", // TODO: Get from request context
                ct);

            outcome = "Queued";
            ep._logger.LogInformation("Epic export job queued: jobId={JobId} epicId={EpicId} isDraft={IsDraft}",
                jobId, epicId, false);

            return TypedResults.Accepted($"/api/story-editor/epics/export-jobs/{jobId}", 
                new EpicExportResponse { JobId = jobId });
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Failed to queue epic export: epicId={EpicId}", epicId);
            outcome = "Error";
            throw;
        }
        finally
        {
            stopwatch.Stop();
            ep._logger.LogInformation(
                "Export epic telemetry | epicId={EpicId} outcome={Outcome} durationMs={DurationMs} isAdmin={IsAdmin} isCreator={IsCreator} isOwner={IsOwner}",
                epicId, outcome, stopwatch.ElapsedMilliseconds, isAdmin, isCreator, isOwner);
        }
    }
}

