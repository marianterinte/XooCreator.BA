using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Queue;
using XooCreator.BA.Infrastructure.Services.Jobs;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class CreateVersionEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IStoryVersionQueue _queue;
    private readonly ILogger<CreateVersionEndpoint> _logger;
    private readonly IJobEventsHub _jobEvents;

    public CreateVersionEndpoint(
        IStoryCraftsRepository crafts,
        IAuth0UserService auth0,
        XooDbContext db,
        IStoryVersionQueue queue,
        IJobEventsHub jobEvents,
        ILogger<CreateVersionEndpoint> logger)
    {
        _crafts = crafts;
        _auth0 = auth0;
        _db = db;
        _queue = queue;
        _jobEvents = jobEvents;
        _logger = logger;
    }

    public record CreateVersionResponse
    {
        public Guid JobId { get; init; }
    }

    [Route("/api/stories/{storyId}/create-version")]
    [Authorize]
    public static async Task<
        Results<
            Accepted<CreateVersionResponse>,
            BadRequest<string>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult,
            Conflict<string>>> HandlePost(
        [FromRoute] string storyId,
        [FromServices] CreateVersionEndpoint ep,
        CancellationToken ct)
    {
        var (user, outcome) = await ep.AuthorizeCreatorAsync(ct);
        if (outcome == AuthorizationOutcome.Unauthorized) return TypedResults.Unauthorized();
        if (outcome == AuthorizationOutcome.Forbidden) return TypedResults.Forbid();
        var currentUser = user!;

        if (!ep.IsValidStoryId(storyId))
        {
            return TypedResults.BadRequest("storyId is required and cannot be 'new'");
        }

        var (definition, validationResult) = await ep.LoadPublishedStoryAsync(storyId, ct);
        if (validationResult != null) return validationResult;

        if (!ep.HasOwnership(currentUser, definition!))
        {
            return TypedResults.Forbid();
        }

        var existingDraftResult = await ep.CheckExistingDraftAsync(storyId, ct);
        if (existingDraftResult != null) return existingDraftResult;

        // Create version job
        var job = new StoryVersionJob
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            OwnerUserId = currentUser.Id,
            RequestedByEmail = currentUser.Email ?? string.Empty,
            BaseVersion = definition!.Version,
            Status = StoryVersionJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        await ep.CreateVersionJobAsync(job, ct);

        ep._jobEvents.Publish(JobTypes.StoryVersion, job.Id, new
        {
            jobId = job.Id,
            storyId = job.StoryId,
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
            "Create version job queued: userId={UserId} storyId={StoryId} jobId={JobId} baseVersion={BaseVersion}",
            currentUser.Id,
            storyId,
            job.Id,
            definition.Version);

        return TypedResults.Accepted($"/api/stories/{storyId}/version-jobs/{job.Id}", new CreateVersionResponse { JobId = job.Id });
    }

    [Route("/api/stories/{storyId}/version-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<VersionJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
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

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var job = await ep._db.StoryVersionJobs
            .FirstOrDefaultAsync(j => j.Id == jobId && j.StoryId == storyId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        var response = new VersionJobStatusResponse
        {
            JobId = job.Id,
            StoryId = job.StoryId,
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

    private enum AuthorizationOutcome
    {
        Ok,
        Unauthorized,
        Forbidden
    }

    private async Task<(AlchimaliaUser? User, AuthorizationOutcome Outcome)> AuthorizeCreatorAsync(CancellationToken ct)
    {
        var user = await _auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return (null, AuthorizationOutcome.Unauthorized);
        }

        if (!_auth0.HasRole(user, UserRole.Creator) && !_auth0.HasRole(user, UserRole.Admin))
        {
            _logger.LogWarning("Create version forbidden: userId={UserId}", user.Id);
            return (user, AuthorizationOutcome.Forbidden);
        }

        return (user, AuthorizationOutcome.Ok);
    }

    private bool IsValidStoryId(string storyId)
    {
        return !(string.IsNullOrWhiteSpace(storyId) || storyId.Equals("new", StringComparison.OrdinalIgnoreCase));
    }

    private async Task<(StoryDefinition? Definition, Results<Accepted<CreateVersionResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>? Error)> LoadPublishedStoryAsync(string storyId, CancellationToken ct)
    {
        var def = await _db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);

        if (def == null)
        {
            _logger.LogWarning("Create version failed. Story not found: {StoryId}", storyId);
            return (null, TypedResults.NotFound());
        }

        if (def.Status != StoryStatus.Published)
        {
            return (null, TypedResults.BadRequest($"Story is not published (status: {def.Status})"));
        }

        return (def, null);
    }

    private bool HasOwnership(AlchimaliaUser user, StoryDefinition definition)
    {
        if (_auth0.HasRole(user, UserRole.Admin))
        {
            return true;
        }

        return definition.CreatedBy == user.Id;
    }

    private async Task<Results<Accepted<CreateVersionResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>?> CheckExistingDraftAsync(string storyId, CancellationToken ct)
    {
        var existingCraft = await _crafts.GetAsync(storyId, ct);
        if (existingCraft != null && existingCraft.Status != StoryStatus.Published.ToDb())
        {
            return TypedResults.Conflict("A draft already exists for this story. Please edit or publish it first.");
        }

        return null;
    }

    private async Task CreateVersionJobAsync(StoryVersionJob job, CancellationToken ct)
    {
        // Mark any queued/running jobs for same story as superseded
        var existingJobs = await _db.StoryVersionJobs
            .Where(j => j.StoryId == job.StoryId && (j.Status == StoryVersionJobStatus.Queued || j.Status == StoryVersionJobStatus.Running))
            .ToListAsync(ct);

        foreach (var existing in existingJobs)
        {
            existing.Status = StoryVersionJobStatus.Superseded;
            existing.CompletedAtUtc = DateTime.UtcNow;
        }

        _db.StoryVersionJobs.Add(job);
        await _db.SaveChangesAsync(ct);
    }
}

public record VersionJobStatusResponse
{
    public Guid JobId { get; init; }
    public string StoryId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime QueuedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
    public int DequeueCount { get; init; }
    public int BaseVersion { get; init; }
}

