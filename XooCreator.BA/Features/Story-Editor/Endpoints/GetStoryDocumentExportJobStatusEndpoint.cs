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
public class GetStoryDocumentExportJobStatusEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly ILogger<GetStoryDocumentExportJobStatusEndpoint> _logger;

    public GetStoryDocumentExportJobStatusEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IBlobSasService sas,
        ILogger<GetStoryDocumentExportJobStatusEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _sas = sas;
        _logger = logger;
    }

    public record StoryDocumentExportJobStatusResponse
    {
        public Guid JobId { get; init; }
        public string StoryId { get; init; } = string.Empty;
        public string Status { get; init; } = StoryDocumentExportJobStatus.Queued;
        public string Format { get; init; } = StoryDocumentExportFormat.Pdf;
        public string Locale { get; init; } = "ro-ro";
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public string? ErrorMessage { get; init; }
        public string? DownloadUrl { get; init; } // SAS URL for download (consistent with Export ZIP)
        public string? OutputFileName { get; init; }
        public long? OutputSizeBytes { get; init; }
    }

    [Route("/api/stories/{storyId}/pdf-jobs/{jobId}")]
    [Authorize]
    public static async Task<Results<Ok<StoryDocumentExportJobStatusResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromRoute] Guid jobId,
        [FromServices] GetStoryDocumentExportJobStatusEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);

        var job = await ep._db.StoryDocumentExportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.StoryId == storyId, ct);

        if (job == null) return TypedResults.NotFound();

        // Allow: admin OR requestor OR story owner.
        if (!isAdmin && job.RequestedByUserId != user.Id && job.StoryOwnerUserId != user.Id)
        {
            ep._logger.LogWarning("Document export job access forbidden: userId={UserId} jobId={JobId} requestedBy={RequestedBy} owner={Owner}",
                user.Id, jobId, job.RequestedByUserId, job.StoryOwnerUserId);
            return TypedResults.Forbid();
        }

        string? downloadUrl = null;
        if (job.Status == StoryDocumentExportJobStatus.Completed && !string.IsNullOrWhiteSpace(job.OutputBlobPath))
        {
            try
            {
                // Generate SAS URL valid for 1 hour (consistent with ZIP export jobs)
                var sasUri = await ep._sas.GetReadSasAsync(ep._sas.DraftContainer, job.OutputBlobPath, TimeSpan.FromHours(1), ct);
                downloadUrl = sasUri.ToString();
            }
            catch (Exception ex)
            {
                ep._logger.LogWarning(ex, "Failed to generate SAS URL for document export job: jobId={JobId} blobPath={BlobPath}",
                    jobId, job.OutputBlobPath);
            }
        }

        var response = new StoryDocumentExportJobStatusResponse
        {
            JobId = job.Id,
            StoryId = job.StoryId,
            Status = job.Status,
            Format = job.Format,
            Locale = job.Locale,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ErrorMessage = job.ErrorMessage,
            DownloadUrl = downloadUrl,
            OutputFileName = job.OutputFileName,
            OutputSizeBytes = job.OutputSizeBytes
        };

        return TypedResults.Ok(response);
    }
}


