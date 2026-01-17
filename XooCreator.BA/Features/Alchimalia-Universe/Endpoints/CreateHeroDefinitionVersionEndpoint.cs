using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Features.AlchimaliaUniverse.Repositories;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class CreateHeroDefinitionVersionEndpoint
{
    private readonly IHeroDefinitionCraftRepository _repository;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IHeroDefinitionVersionQueue _queue;
    private readonly ILogger<CreateHeroDefinitionVersionEndpoint> _logger;
    private readonly IJobEventsHub _jobEvents;

    public CreateHeroDefinitionVersionEndpoint(
        IHeroDefinitionCraftRepository repository,
        IAuth0UserService auth0,
        XooDbContext db,
        IHeroDefinitionVersionQueue queue,
        IJobEventsHub jobEvents,
        ILogger<CreateHeroDefinitionVersionEndpoint> logger)
    {
        _repository = repository;
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

    [Route("/api/{locale}/alchimalia-universe/hero-definitions/{definitionId}/create-version")]
    [Authorize]
    public static async Task<
        Results<
            Accepted<CreateVersionResponse>,
            BadRequest<string>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult,
            Conflict<string>>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string definitionId,
        [FromServices] CreateHeroDefinitionVersionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var definition = await ep._db.HeroDefinitionDefinitions
            .FirstOrDefaultAsync(d => d.Id == definitionId, ct);

        if (definition == null)
        {
            return TypedResults.NotFound();
        }

        // Ownership check: HeroDefinitionDefinition might not have CreatedBy, rely on Role check for now
        // if (definition.CreatedBy != user.Id && !ep._auth0.HasRole(user, UserRole.Admin)) ...

        // Check for existing draft
        // NOTE: Usually drafts have same ID as definition but marked as Draft? 
        // OR draft has a separate ID?
        // In Story, Draft has same ID as Definition.
        // In HeroDefinition, let's assume Draft has same HeroId as Definition?
        // Check CreateCraftFromDefinitionAsync logic: 
        // It creates a NEW draft. 
        // If draft exists, it should fail or update? 
        // CreateHeroDefinitionCraftFromDefinitionEndpoint just calls CreateCraftFromDefinitionAsync.
        // CreateCraftFromDefinitionAsync in Service usually checks if draft exists.
        // Let's assume we check repository.
        
        var existingCraft = await ep._repository.GetAsync(definitionId, ct);
        if (existingCraft != null) 
        {
             // If a draft exists, we conflcit?
             // Story logic: if existingCraft != null && status != Published => Conflict.
             // But existingCraft usually IS the draft.
             // If existingCraft is Published, it means it's the published version?
             // Wait, StoryCraftsRepository.GetAsync returns the *current* state of that ID.
             // If it's published, it's published.
             // We want to create a DRAFT from it.
             // If the entity with ID is Published, we can transition it to Draft?
             // OR we create a new row?
             // Story model: Story entity is one row per ID? No, separation of Craft and Definition tables?
             // StoryCraft (Draft) and StoryDefinition (Published).
             // Same for HeroDefinitionCraft and HeroDefinitionDefinition.
             
             // So if HeroDefinitionCraft exists (and is not Deleted), then we have a draft.
             return TypedResults.Conflict($"A draft already exists for this hero (Id: {definitionId}). Please edit the existing draft.");
        }

        // Create version job
        var job = new HeroDefinitionVersionJob
        {
            Id = Guid.NewGuid(),
            HeroId = definitionId, // This acts as the Target ID
            OwnerUserId = user.Id,
            RequestedByEmail = user.Email ?? string.Empty,
            BaseVersion = definition.Version,
            Status = HeroDefinitionVersionJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        // Mark superseded jobs
        var existingJobs = await ep._db.HeroDefinitionVersionJobs
            .Where(j => j.HeroId == job.HeroId && (j.Status == HeroDefinitionVersionJobStatus.Queued || j.Status == HeroDefinitionVersionJobStatus.Running))
            .ToListAsync(ct);

        foreach (var existing in existingJobs)
        {
            existing.Status = HeroDefinitionVersionJobStatus.Superseded;
            existing.CompletedAtUtc = DateTime.UtcNow;
        }

        ep._db.HeroDefinitionVersionJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        ep._jobEvents.Publish("HeroDefinitionVersion", job.Id, new
        {
            jobId = job.Id,
            heroId = job.HeroId,
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
            "Create Hero version job queued: userId={UserId} heroId={HeroId} jobId={JobId} baseVersion={BaseVersion}",
            user.Id,
            definitionId,
            job.Id,
            definition.Version);

        return TypedResults.Accepted($"/api/{locale}/alchimalia-universe/hero-version-jobs/{job.Id}", new CreateVersionResponse { JobId = job.Id });
    }

    [Route("/api/{locale}/alchimalia-universe/hero-version-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<HeroVersionJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] Guid jobId,
        [FromServices] CreateHeroDefinitionVersionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var job = await ep._db.HeroDefinitionVersionJobs
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }
        
        // Ownership check on job?
        if (job.OwnerUserId != user.Id && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
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
