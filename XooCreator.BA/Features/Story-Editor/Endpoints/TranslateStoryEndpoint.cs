using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class TranslateStoryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<TranslateStoryEndpoint> _logger;
    private readonly IStoryTranslationQueue _queue;
    private readonly IJobEventsHub _jobEvents;

    public TranslateStoryEndpoint(
        XooDbContext db,
        IStoryCraftsRepository crafts,
        IAuth0UserService auth0,
        ILogger<TranslateStoryEndpoint> logger,
        IStoryTranslationQueue queue,
        IJobEventsHub jobEvents)
    {
        _db = db;
        _crafts = crafts;
        _auth0 = auth0;
        _logger = logger;
        _queue = queue;
        _jobEvents = jobEvents;
    }

    public record TranslateStoryRequest
    {
        public string? ReferenceLanguage { get; init; }
        public List<string>? TargetLanguages { get; init; }
        public string? ApiKey { get; init; }
        public string? Provider { get; init; }
        /// <summary>Optional. When set, only these tiles (by Guid from pages-for-audio-export) are translated; story title/summary are always translated.</summary>
        public List<string>? SelectedTileIds { get; init; }
    }

    /// <summary>Response when translation is started as a background job (202 Accepted).</summary>
    public record TranslateStoryJobResponse
    {
        [JsonPropertyName("jobId")]
        public Guid JobId { get; init; }
    }

    [Route("/api/{locale}/stories/{storyId}/translate")]
    [Authorize]
    public static async Task<Results<Accepted<TranslateStoryJobResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] TranslateStoryRequest request,
        [FromServices] TranslateStoryEndpoint ep,
        CancellationToken ct)
    {
        _ = locale;
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);
        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isCreator && !isAdmin)
        {
            ep._logger.LogWarning("Translate forbidden: userId={UserId} not creator/admin", user.Id);
            return TypedResults.Forbid();
        }

        if (request == null)
            return TypedResults.BadRequest("Request body is required.");

        if (string.IsNullOrWhiteSpace(request.ReferenceLanguage))
            return TypedResults.BadRequest("ReferenceLanguage is required.");

        if (request.TargetLanguages == null || request.TargetLanguages.Count == 0)
            return TypedResults.BadRequest("TargetLanguages is required.");

        if (string.IsNullOrWhiteSpace(request.ApiKey))
            return TypedResults.BadRequest("ApiKey is required.");

        if (string.IsNullOrWhiteSpace(storyId))
            return TypedResults.BadRequest("StoryId is required.");

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
            return TypedResults.NotFound();

        if (craft.OwnerUserId != user.Id && !isAdmin)
        {
            ep._logger.LogWarning("Translate forbidden: userId={UserId} storyId={StoryId} not owner", user.Id, storyId);
            return TypedResults.Forbid();
        }

        var status = (craft.Status ?? "draft").ToLowerInvariant();
        if (status is "sent_for_approval" or "in_review" or "approved" or "published")
        {
            ep._logger.LogWarning("Translate read-only: storyId={StoryId} status={Status}", storyId, status);
            return TypedResults.Conflict("Story is read-only in the current status.");
        }

        var targetLanguagesJson = JsonSerializer.Serialize(request.TargetLanguages);
        string? selectedTileIdsJson = null;
        if (request.SelectedTileIds != null && request.SelectedTileIds.Count > 0)
            selectedTileIdsJson = JsonSerializer.Serialize(request.SelectedTileIds);

        var job = new StoryTranslationJob
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            OwnerUserId = craft.OwnerUserId,
            ReferenceLanguage = request.ReferenceLanguage!.Trim(),
            TargetLanguagesJson = targetLanguagesJson,
            SelectedTileIdsJson = selectedTileIdsJson,
            ApiKeyOverride = request.ApiKey!.Trim(),
            Status = StoryTranslationJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        ep._db.StoryTranslationJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        ep._jobEvents.Publish(JobTypes.StoryTranslation, job.Id, new
        {
            jobId = job.Id,
            storyId = job.StoryId,
            status = job.Status,
            queuedAtUtc = job.QueuedAtUtc,
            startedAtUtc = job.StartedAtUtc,
            completedAtUtc = job.CompletedAtUtc,
            errorMessage = job.ErrorMessage,
            fieldsTranslated = job.FieldsTranslated,
            updatedLanguagesJson = job.UpdatedLanguagesJson
        });

        await ep._queue.EnqueueAsync(job, ct);

        return TypedResults.Accepted(
            $"/api/stories/{storyId}/translation-jobs/{job.Id}",
            new TranslateStoryJobResponse { JobId = job.Id });
    }
}
