using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class GetHeroPublishJobStatusEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetHeroPublishJobStatusEndpoint> _logger;

    public GetHeroPublishJobStatusEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        ILogger<GetHeroPublishJobStatusEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _logger = logger;
    }

    public record HeroPublishJobStatusResponse
    {
        public Guid JobId { get; init; }
        public string HeroId { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string? ErrorMessage { get; init; }
        public int DequeueCount { get; init; }
    }

    [Route("/api/alchimalia-universe/hero-crafts/{heroId}/publish-jobs/{jobId}")]
    [Route("/api/alchimalia-universe/toh-hero-crafts/{heroId}/publish-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<HeroPublishJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string heroId,
        [FromRoute] Guid jobId,
        [FromServices] GetHeroPublishJobStatusEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var job = await ep._db.HeroPublishJobs.FirstOrDefaultAsync(j => j.Id == jobId && j.HeroId == heroId, ct);
        if (job == null) return TypedResults.NotFound();

        if (job.OwnerUserId != user.Id && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var response = new HeroPublishJobStatusResponse
        {
            JobId = job.Id,
            HeroId = job.HeroId,
            Status = job.Status,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            DequeueCount = job.DequeueCount
        };

        return TypedResults.Ok(response);
    }
}
