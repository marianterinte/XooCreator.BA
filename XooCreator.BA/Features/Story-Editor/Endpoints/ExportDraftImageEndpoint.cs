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
public class ExportDraftImageEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IStoryCraftsRepository _crafts;
    private readonly ILogger<ExportDraftImageEndpoint> _logger;
    private readonly IStoryImageExportQueue _queue;
    private readonly IJobEventsHub _jobEvents;

    public ExportDraftImageEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IStoryCraftsRepository crafts,
        ILogger<ExportDraftImageEndpoint> logger,
        IStoryImageExportQueue queue,
        IJobEventsHub jobEvents)
    {
        _db = db;
        _auth0 = auth0;
        _crafts = crafts;
        _logger = logger;
        _queue = queue;
        _jobEvents = jobEvents;
    }

    public record ImageExportRequest
    {
        public List<Guid>? SelectedTileIds { get; init; }
        /// <summary>Google API key (required). Must be provided by the user for image generation.</summary>
        public string? ApiKey { get; init; }
    }

    public record ImageExportResponse
    {
        public Guid JobId { get; init; }
    }

    [Route("/api/{locale}/stories/{storyId}/image-export-draft")]
    [Authorize]
    public static async Task<Results<Accepted<ImageExportResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, BadRequest<string>>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] ImageExportRequest? request,
        [FromServices] ExportDraftImageEndpoint ep,
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
            ep._logger.LogWarning("Image export forbidden: userId={UserId} storyId={StoryId} not admin", user.Id, storyId);
            return TypedResults.Forbid();
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
        {
            return TypedResults.NotFound();
        }

        if (string.IsNullOrWhiteSpace(request?.ApiKey))
        {
            return TypedResults.BadRequest("ApiKey is required for image export. Please provide your Google API key in the Generate Images modal.");
        }

        string? selectedTileIdsJson = null;
        if (request?.SelectedTileIds != null && request.SelectedTileIds.Count > 0)
        {
            selectedTileIdsJson = JsonSerializer.Serialize(request.SelectedTileIds);
        }

        var job = new StoryImageExportJob
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            OwnerUserId = craft.OwnerUserId,
            RequestedByEmail = user.Email ?? string.Empty,
            Locale = locale,
            Provider = "Google",
            Status = StoryImageExportJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow,
            SelectedTileIdsJson = selectedTileIdsJson,
            ApiKeyOverride = request!.ApiKey!.Trim()
        };

        ep._db.StoryImageExportJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        ep._jobEvents.Publish(JobTypes.StoryImageExport, job.Id, new
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
            imageCount = job.ImageCount
        });

        await ep._queue.EnqueueAsync(job, ct);

        return TypedResults.Accepted($"/api/stories/{storyId}/image-export-jobs/{job.Id}", new ImageExportResponse { JobId = job.Id });
    }
}
