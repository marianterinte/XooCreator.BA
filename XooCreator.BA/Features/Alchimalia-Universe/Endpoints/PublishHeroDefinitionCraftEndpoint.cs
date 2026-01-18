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
public class PublishHeroDefinitionCraftEndpoint
{
    private readonly IHeroDefinitionCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly IHeroPublishQueue _queue;
    private readonly IJobEventsHub _jobEvents;
    private readonly ILogger<PublishHeroDefinitionCraftEndpoint> _logger;

    public PublishHeroDefinitionCraftEndpoint(
        IHeroDefinitionCraftService service,
        IAuth0UserService auth0,
        XooDbContext db,
        IHeroPublishQueue queue,
        IJobEventsHub jobEvents,
        ILogger<PublishHeroDefinitionCraftEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _db = db;
        _queue = queue;
        _jobEvents = jobEvents;
        _logger = logger;
    }

    public record PublishHeroResponse
    {
        public Guid JobId { get; init; }
    }

    [Route("/api/alchimalia-universe/hero-crafts/{heroId}/publish")]
    [Authorize]
    public static async Task<Results<Accepted<PublishHeroResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult, BadRequest<string>>> HandlePost(
        [FromRoute] string heroId,
        [FromServices] PublishHeroDefinitionCraftEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("PublishHeroDefinitionCraft forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            // Validate status before queuing (optional but good for UX to fail fast)
            // But we don't want to duplicate logic. The service check is sufficient, but happens in background.
            // Let's do a quick check here to return Conflict immediately if possible.
            var craft = await ep._db.HeroDefinitionCrafts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == heroId, ct);
            if (craft == null) return TypedResults.NotFound();

            // Validate ownership/status if strictly needed, but let worker handle logic for simplicity & consistency.
            // However, endpoint usually does authz check.
            if (craft.CreatedByUserId != user.Id && !ep._auth0.HasRole(user, UserRole.Admin))
            {
                return TypedResults.Forbid();
            }

            var job = new HeroPublishJob
            {
                Id = Guid.NewGuid(),
                HeroId = heroId,
                OwnerUserId = craft.CreatedByUserId ?? user.Id,
                RequestedByEmail = user.Email ?? string.Empty,
                Status = HeroPublishJobStatus.Queued,
                QueuedAtUtc = DateTime.UtcNow
            };

            ep._db.HeroPublishJobs.Add(job);
            await ep._db.SaveChangesAsync(ct);

            // Notify UI
            ep._jobEvents.Publish(JobTypes.HeroPublish, job.Id, new
            {
                jobId = job.Id,
                heroId = job.HeroId,
                status = job.Status,
                queuedAtUtc = job.QueuedAtUtc
            });

            await ep._queue.EnqueueAsync(job, ct);

            ep._logger.LogInformation("HeroPublishJob queued: jobId={JobId} heroId={HeroId} userId={UserId}", job.Id, heroId, user.Id);

            return TypedResults.Accepted($"/api/alchimalia-universe/hero-crafts/{heroId}/publish-jobs/{job.Id}", new PublishHeroResponse { JobId = job.Id });
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Failed to queue publish job for hero {HeroId}", heroId);
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
