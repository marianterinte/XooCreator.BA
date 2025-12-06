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
public class GetExportJobStatusEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly ILogger<GetExportJobStatusEndpoint> _logger;

    public GetExportJobStatusEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IBlobSasService sas,
        ILogger<GetExportJobStatusEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _sas = sas;
        _logger = logger;
    }

    public record ExportJobStatusResponse
    {
        public Guid JobId { get; init; }
        public string StoryId { get; init; } = string.Empty;
        public string Status { get; init; } = StoryExportJobStatus.Queued;
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string? ErrorMessage { get; init; }
        public string? ZipDownloadUrl { get; init; } // SAS URL pentru download
        public string? ZipFileName { get; init; }
        public long? ZipSizeBytes { get; init; }
        public int? MediaCount { get; init; }
        public int? LanguageCount { get; init; }
    }

    [Route("/api/stories/{storyId}/export-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<ExportJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromRoute] Guid jobId,
        [FromServices] GetExportJobStatusEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);

        if (!isAdmin && !isCreator)
        {
            return TypedResults.Forbid();
        }

        var job = await ep._db.StoryExportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.StoryId == storyId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        // Verify ownership (unless admin)
        if (!isAdmin)
        {
            if (job.OwnerUserId != user.Id)
            {
                ep._logger.LogWarning("Export job access forbidden: userId={UserId} jobId={JobId} ownerId={OwnerId}",
                    user.Id, jobId, job.OwnerUserId);
                return TypedResults.Forbid();
            }
        }

        // Generate SAS URL if job is completed and ZIP exists
        string? zipDownloadUrl = null;
        if (job.Status == StoryExportJobStatus.Completed && !string.IsNullOrWhiteSpace(job.ZipBlobPath))
        {
            try
            {
                // Generate SAS URL valid for 1 hour
                var sasUri = await ep._sas.GetReadSasAsync(ep._sas.DraftContainer, job.ZipBlobPath, TimeSpan.FromHours(1), ct);
                zipDownloadUrl = sasUri.ToString();
            }
            catch (Exception ex)
            {
                ep._logger.LogWarning(ex, "Failed to generate SAS URL for export job: jobId={JobId} blobPath={BlobPath}",
                    jobId, job.ZipBlobPath);
            }
        }

        var response = new ExportJobStatusResponse
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
            MediaCount = job.MediaCount,
            LanguageCount = job.LanguageCount
        };

        return TypedResults.Ok(response);
    }
}
