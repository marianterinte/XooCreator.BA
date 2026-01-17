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
public class CreateAnimalVersionEndpoint
{
    private readonly IAnimalCraftRepository _repository;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IAnimalVersionQueue _queue;
    private readonly ILogger<CreateAnimalVersionEndpoint> _logger;
    private readonly IJobEventsHub _jobEvents;

    public CreateAnimalVersionEndpoint(
        IAnimalCraftRepository repository,
        IAuth0UserService auth0,
        XooDbContext db,
        IAnimalVersionQueue queue,
        IJobEventsHub jobEvents,
        ILogger<CreateAnimalVersionEndpoint> logger)
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

    [Route("/api/{locale}/alchimalia-universe/animal-definitions/{definitionId}/create-version")]
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
        [FromRoute] Guid definitionId, // Animal Id is Guid
        [FromServices] CreateAnimalVersionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var definition = await ep._db.AnimalDefinitions
            .FirstOrDefaultAsync(d => d.Id == definitionId, ct);

        if (definition == null)
        {
            return TypedResults.NotFound();
        }

        // Ownership check: AnimalDefinition might not have CreatedBy, rely on Role check for now
        // if (definition.CreatedBy != user.Id && !ep._auth0.HasRole(user, UserRole.Admin)) ...
        
        var existingCraft = await ep._repository.GetAsync(definitionId, ct);
        if (existingCraft != null) 
        {
             return TypedResults.Conflict($"A draft already exists for this animal (Id: {definitionId}). Please edit the existing draft.");
        }

        // Create version job
        var job = new AnimalVersionJob
        {
            Id = Guid.NewGuid(),
            AnimalId = definitionId, 
            OwnerUserId = user.Id,
            RequestedByEmail = user.Email ?? string.Empty,
            BaseVersion = definition.Version,
            Status = AnimalVersionJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        // Mark superseded jobs
        var existingJobs = await ep._db.AnimalVersionJobs
            .Where(j => j.AnimalId == job.AnimalId && (j.Status == AnimalVersionJobStatus.Queued || j.Status == AnimalVersionJobStatus.Running))
            .ToListAsync(ct);

        foreach (var existing in existingJobs)
        {
            existing.Status = AnimalVersionJobStatus.Superseded;
            existing.CompletedAtUtc = DateTime.UtcNow;
        }

        ep._db.AnimalVersionJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        ep._jobEvents.Publish("AnimalVersion", job.Id, new
        {
            jobId = job.Id,
            animalId = job.AnimalId,
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
            "Create Animal version job queued: userId={UserId} animalId={AnimalId} jobId={JobId} baseVersion={BaseVersion}",
            user.Id,
            definitionId,
            job.Id,
            definition.Version);

        return TypedResults.Accepted($"/api/{locale}/alchimalia-universe/animal-version-jobs/{job.Id}", new CreateVersionResponse { JobId = job.Id });
    }

    [Route("/api/{locale}/alchimalia-universe/animal-version-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<AnimalVersionJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] Guid jobId,
        [FromServices] CreateAnimalVersionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var job = await ep._db.AnimalVersionJobs
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }
        
        if (job.OwnerUserId != user.Id && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var response = new AnimalVersionJobStatusResponse
        {
            JobId = job.Id,
            AnimalId = job.AnimalId,
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

public record AnimalVersionJobStatusResponse
{
    public Guid JobId { get; init; }
    public Guid AnimalId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime QueuedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
    public int DequeueCount { get; init; }
    public int BaseVersion { get; init; }
}
