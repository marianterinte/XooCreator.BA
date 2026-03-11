using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.CreatureBuilder;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.CreatureBuilder.Endpoints;

[Endpoint]
public sealed class StartGenerativeLoiJobEndpoint
{
    private readonly XooDbContext _db;
    private readonly IUserContextService _userContext;
    private readonly IGenerativeLoiQueue _queue;
    private readonly IJobEventsHub _jobEvents;

    public StartGenerativeLoiJobEndpoint(
        XooDbContext db,
        IUserContextService userContext,
        IGenerativeLoiQueue queue,
        IJobEventsHub jobEvents)
    {
        _db = db;
        _userContext = userContext;
        _queue = queue;
        _jobEvents = jobEvents;
    }

    /// <summary>Combination: full map of body part key to animal label e.g. {"head":"Bunny","body":"Hippo","arms":"Giraffe","legs":"Fox",...}.</summary>
    public record StartGenerativeLoiRequest(Dictionary<string, string>? Combination, DiscoverSelectionDto? Selection);
    public record StartGenerativeLoiResponse(Guid JobId, string Message = "Job queued. Use GET /api/{locale}/creature-builder/generative-loi-jobs/{jobId} or SSE /api/jobs/generative-loi/{jobId}/events for progress.");

    [Route("/api/{locale}/creature-builder/generative-loi-jobs")]
    [Authorize]
    public static async Task<Results<Accepted<StartGenerativeLoiResponse>, BadRequest<StartGenerativeLoiResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] StartGenerativeLoiRequest? request,
        [FromServices] StartGenerativeLoiJobEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        if (request?.Combination == null || request.Combination.Count == 0)
            return TypedResults.BadRequest(new StartGenerativeLoiResponse(Guid.Empty, "Combination is required (object with part keys and animal labels)."));

        var wallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);
        if (wallet == null || wallet.GenerativeBalance < 1)
            return TypedResults.BadRequest(new StartGenerativeLoiResponse(Guid.Empty, "Insufficient generative credits (need 1)."));

        var combinationJson = JsonSerializer.Serialize(request.Combination);
        var job = new GenerativeLoiJob
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            Locale = locale?.Trim().Length > 0 ? locale : "ro-RO",
            CombinationJson = combinationJson,
            Status = "Queued",
            QueuedAtUtc = DateTime.UtcNow
        };
        ep._db.GenerativeLoiJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        ep._jobEvents.Publish(JobTypes.GenerativeLoi, job.Id, new
        {
            jobId = job.Id,
            status = job.Status,
            progressMessage = (string?)null,
            errorMessage = (string?)null,
            queuedAtUtc = job.QueuedAtUtc,
            startedAtUtc = job.StartedAtUtc,
            completedAtUtc = job.CompletedAtUtc,
            bestiaryItemId = (Guid?)null,
            resultName = (string?)null,
            resultImageUrl = (string?)null,
            resultStoryText = (string?)null
        });

        await ep._queue.EnqueueAsync(job, ct);

        return TypedResults.Accepted(
            $"/api/{locale}/creature-builder/generative-loi-jobs/{job.Id}",
            new StartGenerativeLoiResponse(job.Id));
    }
}
