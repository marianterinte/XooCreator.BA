using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class CreateHeroVersionEndpoint
{
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IEpicAggregatesQueue _queue;
    private readonly ILogger<CreateHeroVersionEndpoint> _logger;

    public CreateHeroVersionEndpoint(
        IAuth0UserService auth0,
        XooDbContext db,
        IEpicAggregatesQueue queue,
        ILogger<CreateHeroVersionEndpoint> logger)
    {
        _auth0 = auth0;
        _db = db;
        _queue = queue;
        _logger = logger;
    }

    public record CreateVersionResponse
    {
        public Guid JobId { get; init; }
    }

    [Route("/api/story-editor/heroes/{heroId}/create-version")]
    [Authorize]
    public static async Task<
        Results<
            Accepted<CreateVersionResponse>,
            BadRequest<string>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult,
            Conflict<string>>> HandlePost(
        [FromRoute] string heroId,
        [FromServices] CreateHeroVersionEndpoint ep,
        CancellationToken ct)
    {
        var (user, outcome) = await ep.AuthorizeCreatorAsync(ct);
        if (outcome == AuthorizationOutcome.Unauthorized) return TypedResults.Unauthorized();
        if (outcome == AuthorizationOutcome.Forbidden) return TypedResults.Forbid();
        var currentUser = user!;

        if (!ep.IsValidHeroId(heroId))
        {
            return TypedResults.BadRequest("heroId is required and cannot be 'new'");
        }

        var (definition, validationResult) = await ep.LoadPublishedHeroAsync(heroId, ct);
        if (validationResult != null) return validationResult;

        if (!ep.HasOwnership(currentUser, definition!))
        {
            return TypedResults.Forbid();
        }

        var existingDraftResult = await ep.CheckExistingDraftAsync(heroId, ct);
        if (existingDraftResult != null) return existingDraftResult;

        // Create version job
        var job = new HeroVersionJob
        {
            Id = Guid.NewGuid(),
            HeroId = heroId,
            OwnerUserId = currentUser.Id,
            RequestedByEmail = currentUser.Email ?? string.Empty,
            BaseVersion = definition!.Version,
            Status = HeroVersionJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        await ep.CreateVersionJobAsync(job, ct);
        await ep._queue.EnqueueHeroVersionAsync(job, ct);

        ep._logger.LogInformation(
            "Create version job queued: userId={UserId} heroId={HeroId} jobId={JobId} baseVersion={BaseVersion}",
            currentUser.Id,
            heroId,
            job.Id,
            definition.Version);

        return TypedResults.Accepted($"/api/story-editor/heroes/{heroId}/version-jobs/{job.Id}", new CreateVersionResponse { JobId = job.Id });
    }

    [Route("/api/story-editor/heroes/{heroId}/version-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<HeroVersionJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string heroId,
        [FromRoute] Guid jobId,
        [FromServices] CreateHeroVersionEndpoint ep,
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

        var job = await ep._db.HeroVersionJobs
            .FirstOrDefaultAsync(j => j.Id == jobId && j.HeroId == heroId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        var response = new HeroVersionJobStatusResponse
        {
            JobId = job.Id,
            HeroId = job.HeroId,
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

    private bool IsValidHeroId(string heroId)
    {
        return !(string.IsNullOrWhiteSpace(heroId) || heroId.Equals("new", StringComparison.OrdinalIgnoreCase));
    }

    private async Task<(EpicHeroDefinition? Definition, Results<Accepted<CreateVersionResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>? Error)> LoadPublishedHeroAsync(string heroId, CancellationToken ct)
    {
        var def = await _db.EpicHeroDefinitions
            .FirstOrDefaultAsync(d => d.Id == heroId, ct);

        if (def == null)
        {
            _logger.LogWarning("Create version failed. Hero not found: {HeroId}", heroId);
            return (null, TypedResults.NotFound());
        }

        if (def.Status != "published")
        {
            return (null, TypedResults.BadRequest($"Hero is not published (status: {def.Status})"));
        }

        return (def, null);
    }

    private bool HasOwnership(AlchimaliaUser user, EpicHeroDefinition definition)
    {
        if (_auth0.HasRole(user, UserRole.Admin))
        {
            return true;
        }

        return definition.OwnerUserId == user.Id;
    }

    private async Task<Results<Accepted<CreateVersionResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>?> CheckExistingDraftAsync(string heroId, CancellationToken ct)
    {
        var existingCraft = await _db.EpicHeroCrafts.FirstOrDefaultAsync(c => c.Id == heroId, ct);
        if (existingCraft != null && existingCraft.Status != "published")
        {
            return TypedResults.Conflict("A draft already exists for this hero. Please edit or publish it first.");
        }

        return null;
    }

    private async Task CreateVersionJobAsync(HeroVersionJob job, CancellationToken ct)
    {
        // Mark any queued/running jobs for same hero as superseded
        var existingJobs = await _db.HeroVersionJobs
            .Where(j => j.HeroId == job.HeroId && (j.Status == HeroVersionJobStatus.Queued || j.Status == HeroVersionJobStatus.Running))
            .ToListAsync(ct);

        foreach (var existing in existingJobs)
        {
            existing.Status = HeroVersionJobStatus.Superseded;
            existing.CompletedAtUtc = DateTime.UtcNow;
        }

        _db.HeroVersionJobs.Add(job);
        await _db.SaveChangesAsync(ct);
    }
}

public record HeroVersionJobStatusResponse
{
    public Guid JobId { get; init; }
    public string HeroId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime QueuedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
    public int DequeueCount { get; init; }
    public int BaseVersion { get; init; }
}

