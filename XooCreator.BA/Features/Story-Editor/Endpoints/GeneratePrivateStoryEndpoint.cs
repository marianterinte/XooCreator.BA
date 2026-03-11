using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

/// <summary>
/// Enqueues a private story generation (your-story). Consumes full-story credits; always runs in background.
/// </summary>
[Endpoint]
public class GeneratePrivateStoryEndpoint
{
    private const int MaxIdeaLength = 2000;
    private const int MinIdeaLength = 20;
    private const int MinPageCount = 5;
    private const int MaxPageCount = 10;

    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GeneratePrivateStoryEndpoint> _logger;
    private readonly XooDbContext _db;
    private readonly IStoryAIGenerateQueue _queue;
    private readonly IJobEventsHub _jobEvents;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public GeneratePrivateStoryEndpoint(
        IAuth0UserService auth0,
        ILogger<GeneratePrivateStoryEndpoint> logger,
        XooDbContext db,
        IStoryAIGenerateQueue queue,
        IJobEventsHub jobEvents)
    {
        _auth0 = auth0;
        _logger = logger;
        _db = db;
        _queue = queue;
        _jobEvents = jobEvents;
    }

    [Route("/api/{locale}/your-story/generate")]
    [Authorize]
    public static async Task<Results<Accepted<GeneratePrivateStoryAsyncResponse>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] GeneratePrivateStoryEndpoint ep,
        [FromBody] GeneratePrivateStoryRequest? request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (request == null)
            return TypedResults.BadRequest("Request body is required.");
        var idea = request.Idea?.Trim() ?? string.Empty;
        if (idea.Length < MinIdeaLength)
            return TypedResults.BadRequest($"Idea must be at least {MinIdeaLength} characters.");
        if (idea.Length > MaxIdeaLength)
            return TypedResults.BadRequest($"Idea must be at most {MaxIdeaLength} characters.");

        var pageCountRaw = request.PageCount;
        var pageCount = pageCountRaw <= 0 ? MinPageCount : pageCountRaw;
        if (pageCount < MinPageCount || pageCount > MaxPageCount)
            return TypedResults.BadRequest($"PageCount must be between {MinPageCount} and {MaxPageCount}.");

        var wallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == user.Id, ct);
        if (wallet == null || wallet.FullStoryGenerationBalance < 1)
        {
            ep._logger.LogWarning("GeneratePrivateStory: insufficient credits userId={UserId}", user.Id);
            return TypedResults.BadRequest("Insufficient full story generation credits.");
        }

        var job = new StoryAIGenerateJob
        {
            Id = Guid.NewGuid(),
            OwnerUserId = user.Id,
            RequestedByEmail = user.Email ?? string.Empty,
            OwnerFirstName = user.FirstName,
            OwnerLastName = user.LastName,
            Locale = locale,
            Status = StoryAIGenerateJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow,
            IsPrivateStoryGeneration = true,
            RequestJson = JsonSerializer.Serialize(new GeneratePrivateStoryRequest
            {
                Idea = idea,
                LanguageCode = request.LanguageCode?.Trim() ?? "ro-ro",
                Title = request.Title?.Trim(),
                PageCount = pageCount,
                GenerateImages = request.GenerateImages,
                GenerateAudio = request.GenerateAudio
            }, JsonOptions)
        };
        ep._db.StoryAIGenerateJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        ep._jobEvents.Publish(JobTypes.StoryAIGenerate, job.Id, new
        {
            jobId = job.Id,
            storyId = (string?)null,
            status = job.Status,
            progressMessage = (string?)null,
            queuedAtUtc = job.QueuedAtUtc
        });
        await ep._queue.EnqueueAsync(job, ct);

        var location = $"/api/{locale}/your-story/jobs/{job.Id}";
        return TypedResults.Accepted(location, new GeneratePrivateStoryAsyncResponse { JobId = job.Id });
    }
}

public sealed class GeneratePrivateStoryAsyncResponse
{
    public Guid JobId { get; init; }
}
