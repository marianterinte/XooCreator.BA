using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetAudioExportJobStatusEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly ILogger<GetAudioExportJobStatusEndpoint> _logger;

    public GetAudioExportJobStatusEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IBlobSasService sas,
        ILogger<GetAudioExportJobStatusEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _sas = sas;
        _logger = logger;
    }

    public record AudioExportJobStatusResponse
    {
        public Guid JobId { get; init; }
        public string StoryId { get; init; } = string.Empty;
        public string Status { get; init; } = StoryAudioExportJobStatus.Queued;
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string? ErrorMessage { get; init; }
        public string? ZipDownloadUrl { get; init; }
        public string? ZipFileName { get; init; }
        public long? ZipSizeBytes { get; init; }
        public int? AudioCount { get; init; }
    }

    [Route("/api/stories/{storyId}/audio-export-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<AudioExportJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromRoute] Guid jobId,
        [FromServices] GetAudioExportJobStatusEndpoint ep,
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
            ep._logger.LogWarning("Audio export job access forbidden: userId={UserId} jobId={JobId} not admin",
                user.Id, jobId);
            return TypedResults.Forbid();
        }

        var job = await ep._db.StoryAudioExportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.StoryId == storyId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        string? zipDownloadUrl = null;
        if (job.Status == StoryAudioExportJobStatus.Completed && !string.IsNullOrWhiteSpace(job.ZipBlobPath))
        {
            try
            {
                var sasUri = await ep._sas.GetReadSasAsync(ep._sas.DraftContainer, job.ZipBlobPath, TimeSpan.FromHours(1), ct);
                zipDownloadUrl = sasUri.ToString();
            }
            catch (Exception ex)
            {
                ep._logger.LogWarning(ex, "Failed to generate SAS URL for audio export job: jobId={JobId} blobPath={BlobPath}",
                    jobId, job.ZipBlobPath);
            }
        }

        var response = new AudioExportJobStatusResponse
        {
            JobId = job.Id,
            StoryId = job.StoryId,
            Status = job.Status,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            ZipDownloadUrl = zipDownloadUrl,
            ZipFileName = job.ZipFileName,
            ZipSizeBytes = job.ZipSizeBytes,
            AudioCount = job.AudioCount
        };

        return TypedResults.Ok(response);
    }
}
