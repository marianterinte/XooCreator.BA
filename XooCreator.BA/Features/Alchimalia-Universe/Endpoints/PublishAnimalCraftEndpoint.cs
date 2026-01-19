using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class PublishAnimalCraftEndpoint
{
    private readonly IAnimalCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IAnimalPublishQueue _queue;
    private readonly IJobEventsHub _jobEvents;
    private readonly ILogger<PublishAnimalCraftEndpoint> _logger;

    public PublishAnimalCraftEndpoint(
        IAnimalCraftService service,
        IAuth0UserService auth0,
        XooDbContext db,
        IAnimalPublishQueue queue,
        IJobEventsHub jobEvents,
        ILogger<PublishAnimalCraftEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _db = db;
        _queue = queue;
        _jobEvents = jobEvents;
        _logger = logger;
    }

    public record PublishAnimalResponse
    {
        public Guid JobId { get; init; }
    }

    [Route("/api/alchimalia-universe/animal-crafts/{animalId}/publish")]
    [Authorize]
    public static async Task<Results<Accepted<PublishAnimalResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult, BadRequest<string>>> HandlePost(
        [FromRoute] Guid animalId,
        [FromServices] PublishAnimalCraftEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("PublishAnimalCraft forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            // Fail fast if not found or unauthorized owner
            var craft = await ep._db.AnimalCrafts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == animalId, ct);
            if (craft == null) return TypedResults.NotFound();

            if (craft.CreatedByUserId != user.Id && !ep._auth0.HasRole(user, UserRole.Admin))
            {
                return TypedResults.Forbid();
            }

            // Supersede any previous queued/running publish jobs for this animal to avoid race conditions
            var existingJobs = await ep._db.AnimalPublishJobs
                .Where(j => j.AnimalId == animalId.ToString() &&
                            (j.Status == AnimalPublishJobStatus.Queued || j.Status == AnimalPublishJobStatus.Running))
                .ToListAsync(ct);

            foreach (var existing in existingJobs)
            {
                existing.Status = AnimalPublishJobStatus.Superseded;
                existing.CompletedAtUtc = DateTime.UtcNow;
            }

            var job = new AnimalPublishJob
            {
                Id = Guid.NewGuid(),
                AnimalId = animalId.ToString(),
                OwnerUserId = craft.CreatedByUserId ?? user.Id,
                RequestedByEmail = user.Email ?? string.Empty,
                Status = AnimalPublishJobStatus.Queued,
                QueuedAtUtc = DateTime.UtcNow
            };

            ep._db.AnimalPublishJobs.Add(job);
            await ep._db.SaveChangesAsync(ct);

            ep._jobEvents.Publish(JobTypes.AnimalPublish, job.Id, new
            {
                jobId = job.Id,
                animalId = job.AnimalId,
                status = job.Status,
                queuedAtUtc = job.QueuedAtUtc
            });

            await ep._queue.EnqueueAsync(job, ct);

            ep._logger.LogInformation("AnimalPublishJob queued: jobId={JobId} animalId={AnimalId} userId={UserId}", job.Id, animalId, user.Id);

            return TypedResults.Accepted($"/api/alchimalia-universe/animal-crafts/{animalId}/publish-jobs/{job.Id}", new PublishAnimalResponse { JobId = job.Id });
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Failed to queue publish job for animal {AnimalId}", animalId);
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
