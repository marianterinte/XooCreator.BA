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
public class GetAnimalPublishJobStatusEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetAnimalPublishJobStatusEndpoint> _logger;

    public GetAnimalPublishJobStatusEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        ILogger<GetAnimalPublishJobStatusEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _logger = logger;
    }

    public record AnimalPublishJobStatusResponse
    {
        public Guid JobId { get; init; }
        public string AnimalId { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string? ErrorMessage { get; init; }
        public int DequeueCount { get; init; }
    }

    [Route("/api/alchimalia-universe/animal-crafts/{animalId}/publish-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<AnimalPublishJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] Guid animalId,
        [FromRoute] Guid jobId,
        [FromServices] GetAnimalPublishJobStatusEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var job = await ep._db.AnimalPublishJobs.FirstOrDefaultAsync(j => j.Id == jobId && j.AnimalId == animalId.ToString(), ct);
        if (job == null) return TypedResults.NotFound();

        if (job.OwnerUserId != user.Id && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var response = new AnimalPublishJobStatusResponse
        {
            JobId = job.Id,
            AnimalId = job.AnimalId,
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
