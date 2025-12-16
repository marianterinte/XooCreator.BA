using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class PublishStoryEpicEndpoint
{
    private readonly IStoryEpicPublishingService _publishingService;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IEpicPublishQueue _queue;
    private readonly ILogger<PublishStoryEpicEndpoint> _logger;

    public PublishStoryEpicEndpoint(
        IStoryEpicPublishingService publishingService,
        IAuth0UserService auth0,
        XooDbContext db,
        IEpicPublishQueue queue,
        ILogger<PublishStoryEpicEndpoint> logger)
    {
        _publishingService = publishingService;
        _auth0 = auth0;
        _db = db;
        _queue = queue;
        _logger = logger;
    }

    public record PublishEpicRequest
    {
        public bool ForceFull { get; init; } = false;
    }

    public record PublishEpicResponse
    {
        public Guid JobId { get; init; }
    }

    [Route("/api/story-editor/epics/{epicId}/publish")]
    [Authorize]
    public static async Task<Results<Accepted<PublishEpicResponse>, UnauthorizedHttpResult, ForbidHttpResult, BadRequest<string>, Conflict<string>, NotFound>> HandlePost(
        [FromRoute] string epicId,
        [FromServices] PublishStoryEpicEndpoint ep,
        [FromBody] PublishEpicRequest? request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("PublishStoryEpic forbidden: userId={UserId} roles={Roles}",
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);

        try
        {
            // Load craft
            var craft = await ep._db.StoryEpicCrafts
                .Include(c => c.Translations)
                .FirstOrDefaultAsync(c => c.Id == epicId, ct);
            
            if (craft == null)
            {
                return TypedResults.NotFound();
            }

            // Validate permissions and status
            if (craft.OwnerUserId != user.Id && !isAdmin)
            {
                ep._logger.LogWarning("PublishStoryEpic unauthorized: userId={UserId} epicId={EpicId} ownerId={OwnerId}", 
                    user.Id, epicId, craft.OwnerUserId);
                return TypedResults.Forbid();
            }

            if (craft.Status != "approved")
            {
                return TypedResults.BadRequest($"Epic is not approved (status: {craft.Status}). Only approved epics can be published.");
            }

            // Use first available translation or ro-ro as fallback
            var langTag = craft.Translations.FirstOrDefault(t => t.LanguageCode == "ro-ro")?.LanguageCode
                ?? craft.Translations.FirstOrDefault()?.LanguageCode
                ?? "ro-ro";

            var forceFull = request?.ForceFull ?? false;

            // Create publish job
            var job = new EpicPublishJob
            {
                Id = Guid.NewGuid(),
                EpicId = craft.Id,
                OwnerUserId = craft.OwnerUserId,
                RequestedByEmail = user.Email ?? string.Empty,
                LangTag = langTag,
                DraftVersion = craft.LastDraftVersion,
                ForceFull = forceFull,
                Status = EpicPublishJobStatus.Queued,
                QueuedAtUtc = DateTime.UtcNow
            };

            await ep.CreatePublishJobAsync(job, ct);
            await ep._queue.EnqueueAsync(job, ct);

            ep._logger.LogInformation("PublishStoryEpic job queued: userId={UserId} epicId={EpicId} jobId={JobId} draftVersion={DraftVersion}",
                user.Id, epicId, job.Id, craft.LastDraftVersion);

            return TypedResults.Accepted($"/api/story-editor/epics/{epicId}/publish-jobs/{job.Id}", new PublishEpicResponse { JobId = job.Id });
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning("PublishStoryEpic unauthorized: {Error}", ex.Message);
            return TypedResults.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            // Validation errors (unpublished dependencies, etc.)
            ep._logger.LogWarning("PublishStoryEpic validation failed: {Error}", ex.Message);
            return TypedResults.Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "PublishStoryEpic error: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.BadRequest($"Failed to publish epic: {ex.Message}");
        }
    }

    [Route("/api/story-editor/epics/{epicId}/publish-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<EpicPublishJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string epicId,
        [FromRoute] Guid jobId,
        [FromServices] PublishStoryEpicEndpoint ep,
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

        var job = await ep._db.EpicPublishJobs
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
                ep._logger.LogWarning("Epic publish job access forbidden: userId={UserId} jobId={JobId} ownerId={OwnerId}",
                    user.Id, jobId, job.OwnerUserId);
                return TypedResults.Forbid();
            }
        }

        var response = new EpicPublishJobStatusResponse
        {
            JobId = job.Id,
            EpicId = job.EpicId,
            Status = job.Status,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            DequeueCount = job.DequeueCount,
            DraftVersion = job.DraftVersion
        };

        return TypedResults.Ok(response);
    }

    private async Task CreatePublishJobAsync(EpicPublishJob job, CancellationToken ct)
    {
        // Mark any queued/running jobs for same epic as superseded
        var existingJobs = await _db.EpicPublishJobs
            .Where(j => j.EpicId == job.EpicId && (j.Status == EpicPublishJobStatus.Queued || j.Status == EpicPublishJobStatus.Running))
            .ToListAsync(ct);

        foreach (var existing in existingJobs)
        {
            existing.Status = EpicPublishJobStatus.Superseded;
            existing.CompletedAtUtc = DateTime.UtcNow;
        }

        _db.EpicPublishJobs.Add(job);
        await _db.SaveChangesAsync(ct);
    }
}

public record EpicPublishJobStatusResponse
{
    public Guid JobId { get; init; }
    public string EpicId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime QueuedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
    public int DequeueCount { get; init; }
    public int DraftVersion { get; init; }
}
