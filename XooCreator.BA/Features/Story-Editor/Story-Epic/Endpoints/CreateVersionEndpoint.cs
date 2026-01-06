using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Queue;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Services.Jobs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class CreateVersionEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IEpicVersionQueue _queue;
    private readonly ILogger<CreateVersionEndpoint> _logger;
    private readonly IJobEventsHub _jobEvents;

    public CreateVersionEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        XooDbContext db,
        IEpicVersionQueue queue,
        IJobEventsHub jobEvents,
        ILogger<CreateVersionEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _db = db;
        _queue = queue;
        _jobEvents = jobEvents;
        _logger = logger;
    }

    public record CreateEpicVersionResponse
    {
        public Guid JobId { get; init; }
    }

    [Route("/api/story-editor/epics/{epicId}/create-version")]
    [Authorize]
    public static async Task<
        Results<
            Accepted<CreateEpicVersionResponse>,
            BadRequest<string>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult,
            Conflict<string>>> HandlePost(
        [FromRoute] string epicId,
        [FromServices] CreateVersionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Creator-only guard
        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("Create version forbidden: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(epicId) || epicId.Equals("new", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("epicId is required and cannot be 'new'");
        }

        try
        {
            // Get published StoryEpicDefinition to verify it exists and is published
            var definition = await ep._db.StoryEpicDefinitions.FirstOrDefaultAsync(d => d.Id == epicId, ct);
            if (definition == null)
            {
                ep._logger.LogWarning("Create version failed. Epic not found: {EpicId}", epicId);
                return TypedResults.NotFound();
            }

            if (definition.Status != "published")
            {
                return TypedResults.BadRequest($"Epic is not published (status: {definition.Status})");
            }

            // Check if draft already exists
            var existingCraft = await ep._db.StoryEpicCrafts.FirstOrDefaultAsync(c => c.Id == epicId, ct);
            if (existingCraft != null && existingCraft.Status != "published")
            {
                return TypedResults.Conflict("A draft already exists for this epic. Please edit or publish it first.");
            }

            // Create version job
            var job = new EpicVersionJob
            {
                Id = Guid.NewGuid(),
                EpicId = epicId,
                OwnerUserId = user.Id,
                RequestedByEmail = user.Email ?? string.Empty,
                BaseVersion = definition.Version,
                Status = EpicVersionJobStatus.Queued,
                QueuedAtUtc = DateTime.UtcNow
            };

            await ep.CreateVersionJobAsync(job, ct);

            ep._jobEvents.Publish(JobTypes.EpicVersion, job.Id, new
            {
                jobId = job.Id,
                epicId = job.EpicId,
                status = job.Status,
                queuedAtUtc = job.QueuedAtUtc,
                startedAtUtc = job.StartedAtUtc,
                completedAtUtc = job.CompletedAtUtc,
                errorMessage = job.ErrorMessage,
                dequeueCount = job.DequeueCount,
                baseVersion = job.BaseVersion
            });

            await ep._queue.EnqueueAsync(job, ct);

            ep._logger.LogInformation(
                "Create version job queued: userId={UserId} epicId={EpicId} jobId={JobId} baseVersion={BaseVersion}",
                user.Id,
                epicId,
                job.Id,
                definition.Version);

            return TypedResults.Accepted($"/api/story-editor/epics/{epicId}/version-jobs/{job.Id}", new CreateEpicVersionResponse { JobId = job.Id });
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning("Create version unauthorized: {Message}", ex.Message);
            return TypedResults.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("already exists"))
            {
                return TypedResults.Conflict(ex.Message);
            }
            ep._logger.LogWarning("Create version failed: {Message}", ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Create version error: epicId={EpicId}", epicId);
            return TypedResults.BadRequest($"Failed to create version: {ex.Message}");
        }
    }

    [Route("/api/story-editor/epics/{epicId}/version-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<EpicVersionJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string epicId,
        [FromRoute] Guid jobId,
        [FromServices] CreateVersionEndpoint ep,
        CancellationToken ct)
    {
        // Authorization check
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);

        if (!isAdmin && !isCreator)
        {
            return TypedResults.Forbid();
        }

        var job = await ep._db.EpicVersionJobs
            .FirstOrDefaultAsync(j => j.Id == jobId && j.EpicId == epicId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        // Verify ownership (unless admin)
        if (!isAdmin)
        {
            if (job.OwnerUserId != user.Id)
            {
                ep._logger.LogWarning("Epic version job access forbidden: userId={UserId} jobId={JobId} ownerId={OwnerId}",
                    user.Id, jobId, job.OwnerUserId);
                return TypedResults.Forbid();
            }
        }

        var response = new EpicVersionJobStatusResponse
        {
            JobId = job.Id,
            EpicId = job.EpicId,
            Status = job.Status,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            DequeueCount = job.DequeueCount,
            BaseVersion = job.BaseVersion
        };

        return TypedResults.Ok(response);
    }

    private async Task CreateVersionJobAsync(EpicVersionJob job, CancellationToken ct)
    {
        // Mark any queued/running jobs for same epic as superseded
        var existingJobs = await _db.EpicVersionJobs
            .Where(j => j.EpicId == job.EpicId && (j.Status == EpicVersionJobStatus.Queued || j.Status == EpicVersionJobStatus.Running))
            .ToListAsync(ct);

        foreach (var existing in existingJobs)
        {
            existing.Status = EpicVersionJobStatus.Superseded;
            existing.CompletedAtUtc = DateTime.UtcNow;
        }

        _db.EpicVersionJobs.Add(job);
        await _db.SaveChangesAsync(ct);
    }
}

public record EpicVersionJobStatusResponse
{
    public Guid JobId { get; init; }
    public string EpicId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime QueuedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
    public int DequeueCount { get; init; }
    public int BaseVersion { get; init; }
}

