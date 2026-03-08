using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
/// Generates a full story draft from the story list using the user's API key.
/// Sync: 200 + storyId. Async (RunInBackground): 202 + jobId, worker processes and sends SSE.
/// </summary>
[Endpoint]
public class GenerateFullStoryDraftEndpoint
{
    private readonly IGenerateFullStoryDraftHandler _handler;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GenerateFullStoryDraftEndpoint> _logger;
    private readonly XooDbContext _db;
    private readonly IStoryAIGenerateQueue _queue;
    private readonly IJobEventsHub _jobEvents;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public GenerateFullStoryDraftEndpoint(
        IGenerateFullStoryDraftHandler handler,
        IAuth0UserService auth0,
        ILogger<GenerateFullStoryDraftEndpoint> logger,
        XooDbContext db,
        IStoryAIGenerateQueue queue,
        IJobEventsHub jobEvents)
    {
        _handler = handler;
        _auth0 = auth0;
        _logger = logger;
        _db = db;
        _queue = queue;
        _jobEvents = jobEvents;
    }

    [Route("/api/{locale}/story-editor/generate-full-story-draft")]
    [Authorize]
    public static async Task<Results<Ok<GenerateFullStoryDraftResponse>, Accepted<GenerateFullStoryDraftAsyncResponse>, BadRequest<string>, IResult, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] GenerateFullStoryDraftEndpoint ep,
        [FromBody] GenerateFullStoryDraftRequest request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("GenerateFullStoryDraft forbidden: userId={UserId} not a creator", user.Id);
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(request?.ApiKey))
            return TypedResults.BadRequest("ApiKey is required.");
        if (string.IsNullOrWhiteSpace(request.LanguageCode))
            return TypedResults.BadRequest("LanguageCode is required.");
        if (string.IsNullOrWhiteSpace(request.TextSeed) || request.TextSeed.Trim().Length < 20)
            return TypedResults.BadRequest("TextSeed must be at least 20 characters.");
        if (request.NumberOfPages < 1 || request.NumberOfPages > 10)
            return TypedResults.BadRequest("NumberOfPages must be between 1 and 10.");
        var provider = (request.Provider ?? "Google").Trim();
        if (!string.Equals(provider, "Google", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(provider, "OpenAI", StringComparison.OrdinalIgnoreCase))
            return TypedResults.BadRequest("Provider must be Google or OpenAI.");
        if (request.GenerateImages && request.ImageSeedInstructions != null && request.ImageSeedInstructions.Trim().Length > 1500)
            return TypedResults.BadRequest("Image seed instructions must be at most 1500 characters.");
        const int maxInstructionsLength = 3000;
        if (request.Instructions != null && request.Instructions.Trim().Length > maxInstructionsLength)
            return TypedResults.BadRequest("Instructions must be at most 3000 characters.");

        if (request.RunInBackground)
        {
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
                RequestJson = JsonSerializer.Serialize(request, JsonOptions)
            };
            ep._db.StoryAIGenerateJobs.Add(job);
            await ep._db.SaveChangesAsync(ct);

            ep._jobEvents.Publish(JobTypes.StoryAIGenerate, job.Id, new
            {
                jobId = job.Id,
                storyId = (string?)null,
                status = job.Status,
                progressMessage = (string?)null,
                queuedAtUtc = job.QueuedAtUtc,
                startedAtUtc = job.StartedAtUtc,
                completedAtUtc = job.CompletedAtUtc,
                errorMessage = (string?)null,
                errorCode = (string?)null
            });
            await ep._queue.EnqueueAsync(job, ct);

            var location = $"/api/{locale}/story-editor/generate-full-story-draft/jobs/{job.Id}";
            return TypedResults.Accepted(location, new GenerateFullStoryDraftAsyncResponse { JobId = job.Id });
        }

        try
        {
            var response = await ep._handler.ExecuteAsync(
                request!,
                user.Id,
                user.FirstName ?? string.Empty,
                user.LastName ?? string.Empty,
                user.Email ?? string.Empty,
                ct);
            return TypedResults.Ok(response);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex) when (IsRateLimitException(ex))
        {
            ep._logger.LogWarning(ex, "GenerateFullStoryDraft sync: rate limit (429) from provider");
            var body = new { errorCode = "RateLimitExceeded", message = "Generation stopped: rate limit exceeded (429). Please try again in a few minutes." };
            return (Results<Ok<GenerateFullStoryDraftResponse>, Accepted<GenerateFullStoryDraftAsyncResponse>, BadRequest<string>, IResult, UnauthorizedHttpResult, ForbidHttpResult>)
                Results.Json(body, statusCode: StatusCodes.Status429TooManyRequests);
        }
    }

    private static bool IsRateLimitException(Exception ex)
    {
        var message = (ex.Message + " " + ex.InnerException?.Message).ToLowerInvariant();
        return message.Contains("429") || message.Contains("too many requests") || message.Contains("rate limit");
    }
}
