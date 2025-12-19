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
public class CreateRegionVersionEndpoint
{
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IEpicAggregatesQueue _queue;
    private readonly ILogger<CreateRegionVersionEndpoint> _logger;

    public CreateRegionVersionEndpoint(
        IAuth0UserService auth0,
        XooDbContext db,
        IEpicAggregatesQueue queue,
        ILogger<CreateRegionVersionEndpoint> logger)
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

    [Route("/api/story-editor/regions/{regionId}/create-version")]
    [Authorize]
    public static async Task<
        Results<
            Accepted<CreateVersionResponse>,
            BadRequest<string>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult,
            Conflict<string>>> HandlePost(
        [FromRoute] string regionId,
        [FromServices] CreateRegionVersionEndpoint ep,
        CancellationToken ct)
    {
        var (user, outcome) = await ep.AuthorizeCreatorAsync(ct);
        if (outcome == AuthorizationOutcome.Unauthorized) return TypedResults.Unauthorized();
        if (outcome == AuthorizationOutcome.Forbidden) return TypedResults.Forbid();
        var currentUser = user!;

        if (!ep.IsValidRegionId(regionId))
        {
            return TypedResults.BadRequest("regionId is required and cannot be 'new'");
        }

        var (definition, validationResult) = await ep.LoadPublishedRegionAsync(regionId, ct);
        if (validationResult != null) return validationResult;

        if (!ep.HasOwnership(currentUser, definition!))
        {
            return TypedResults.Forbid();
        }

        var existingDraftResult = await ep.CheckExistingDraftAsync(regionId, ct);
        if (existingDraftResult != null) return existingDraftResult;

        // Create version job
        var job = new RegionVersionJob
        {
            Id = Guid.NewGuid(),
            RegionId = regionId,
            OwnerUserId = currentUser.Id,
            RequestedByEmail = currentUser.Email ?? string.Empty,
            BaseVersion = definition!.Version,
            Status = RegionVersionJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        await ep.CreateVersionJobAsync(job, ct);
        await ep._queue.EnqueueRegionVersionAsync(job, ct);

        ep._logger.LogInformation(
            "Create version job queued: userId={UserId} regionId={RegionId} jobId={JobId} baseVersion={BaseVersion}",
            currentUser.Id,
            regionId,
            job.Id,
            definition.Version);

        return TypedResults.Accepted($"/api/story-editor/regions/{regionId}/version-jobs/{job.Id}", new CreateVersionResponse { JobId = job.Id });
    }

    [Route("/api/story-editor/regions/{regionId}/version-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<RegionVersionJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string regionId,
        [FromRoute] Guid jobId,
        [FromServices] CreateRegionVersionEndpoint ep,
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

        var job = await ep._db.RegionVersionJobs
            .FirstOrDefaultAsync(j => j.Id == jobId && j.RegionId == regionId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        var response = new RegionVersionJobStatusResponse
        {
            JobId = job.Id,
            RegionId = job.RegionId,
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

    private bool IsValidRegionId(string regionId)
    {
        return !(string.IsNullOrWhiteSpace(regionId) || regionId.Equals("new", StringComparison.OrdinalIgnoreCase));
    }

    private async Task<(StoryRegionDefinition? Definition, Results<Accepted<CreateVersionResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>? Error)> LoadPublishedRegionAsync(string regionId, CancellationToken ct)
    {
        var def = await _db.StoryRegionDefinitions
            .FirstOrDefaultAsync(d => d.Id == regionId, ct);

        if (def == null)
        {
            _logger.LogWarning("Create version failed. Region not found: {RegionId}", regionId);
            return (null, TypedResults.NotFound());
        }

        if (def.Status != "published")
        {
            return (null, TypedResults.BadRequest($"Region is not published (status: {def.Status})"));
        }

        return (def, null);
    }

    private bool HasOwnership(AlchimaliaUser user, StoryRegionDefinition definition)
    {
        if (_auth0.HasRole(user, UserRole.Admin))
        {
            return true;
        }

        return definition.OwnerUserId == user.Id;
    }

    private async Task<Results<Accepted<CreateVersionResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>?> CheckExistingDraftAsync(string regionId, CancellationToken ct)
    {
        var existingCraft = await _db.StoryRegionCrafts.FirstOrDefaultAsync(c => c.Id == regionId, ct);
        if (existingCraft != null && existingCraft.Status != "published")
        {
            return TypedResults.Conflict("A draft already exists for this region. Please edit or publish it first.");
        }

        return null;
    }

    private async Task CreateVersionJobAsync(RegionVersionJob job, CancellationToken ct)
    {
        // Mark any queued/running jobs for same region as superseded
        var existingJobs = await _db.RegionVersionJobs
            .Where(j => j.RegionId == job.RegionId && (j.Status == RegionVersionJobStatus.Queued || j.Status == RegionVersionJobStatus.Running))
            .ToListAsync(ct);

        foreach (var existing in existingJobs)
        {
            existing.Status = RegionVersionJobStatus.Superseded;
            existing.CompletedAtUtc = DateTime.UtcNow;
        }

        _db.RegionVersionJobs.Add(job);
        await _db.SaveChangesAsync(ct);
    }
}

public record RegionVersionJobStatusResponse
{
    public Guid JobId { get; init; }
    public string RegionId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime QueuedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
    public int DequeueCount { get; init; }
    public int BaseVersion { get; init; }
}

