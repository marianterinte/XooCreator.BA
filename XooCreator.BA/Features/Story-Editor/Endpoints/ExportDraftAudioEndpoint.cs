using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
public class ExportDraftAudioEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IStoryCraftsRepository _crafts;
    private readonly ILogger<ExportDraftAudioEndpoint> _logger;
    private readonly IStoryAudioExportQueue _queue;
    private readonly IJobEventsHub _jobEvents;

    public ExportDraftAudioEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IStoryCraftsRepository crafts,
        ILogger<ExportDraftAudioEndpoint> logger,
        IStoryAudioExportQueue queue,
        IJobEventsHub jobEvents)
    {
        _db = db;
        _auth0 = auth0;
        _crafts = crafts;
        _logger = logger;
        _queue = queue;
        _jobEvents = jobEvents;
    }

    public record AudioExportRequest
    {
        public List<Guid>? SelectedTileIds { get; init; }
        public string? ApiKey { get; init; } // Optional API key from UI
    }

    public record AudioExportResponse
    {
        public Guid JobId { get; init; }
    }

    [Route("/api/{locale}/stories/{storyId}/audio-export-draft")]
    [Authorize]
    public static async Task<Results<Accepted<AudioExportResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, BadRequest<string>>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] AudioExportRequest? request,
        [FromServices] ExportDraftAudioEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);

        if (!isAdmin)
        {
            ep._logger.LogWarning("Audio export forbidden: userId={UserId} storyId={StoryId} not admin", user.Id, storyId);
            return TypedResults.Forbid();
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
        {
            return TypedResults.NotFound();
        }

        string? selectedTileIdsJson = null;
        if (request?.SelectedTileIds != null && request.SelectedTileIds.Count > 0)
        {
            selectedTileIdsJson = JsonSerializer.Serialize(request.SelectedTileIds);
        }

        var job = new StoryAudioExportJob
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            OwnerUserId = craft.OwnerUserId,
            RequestedByEmail = user.Email ?? string.Empty,
            Locale = locale,
            Status = StoryAudioExportJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow,
            SelectedTileIdsJson = selectedTileIdsJson,
            ApiKeyOverride = !string.IsNullOrWhiteSpace(request?.ApiKey) ? request.ApiKey : null
        };

        ep._db.StoryAudioExportJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        ep._jobEvents.Publish(JobTypes.StoryAudioExport, job.Id, new
        {
            jobId = job.Id,
            storyId = job.StoryId,
            status = job.Status,
            queuedAtUtc = job.QueuedAtUtc,
            startedAtUtc = job.StartedAtUtc,
            completedAtUtc = job.CompletedAtUtc,
            errorMessage = job.ErrorMessage,
            zipDownloadUrl = (string?)null,
            zipFileName = job.ZipFileName,
            zipSizeBytes = job.ZipSizeBytes,
            audioCount = job.AudioCount
        });

        await ep._queue.EnqueueAsync(job, ct);

        return TypedResults.Accepted($"/api/stories/{storyId}/audio-export-jobs/{job.Id}", new AudioExportResponse { JobId = job.Id });
    }
}
