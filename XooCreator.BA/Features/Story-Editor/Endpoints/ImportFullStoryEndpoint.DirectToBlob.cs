using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

/// <summary>
/// [Obsolete] Legacy direct-to-blob full story import: client uploads whole ZIP to SAS; server processes ZIP.
/// Prefer client-side ZIP flow: prepare-from-manifest + confirm-from-assets (browser unzips, uploads manifest + assets to staging).
/// </summary>
public partial class ImportFullStoryEndpoint
{
    public record ImportFullStoryRequestUploadRequest(
        string FileName,
        long? ExpectedSize,
        bool? IncludeImages,
        bool? IncludeAudio,
        bool? IncludeVideo);

    public record ImportFullStoryRequestUploadResponse(string PutUrl, Guid UploadId);

    public record ImportFullStoryConfirmUploadRequest(
        Guid UploadId,
        long? Size,
        string? FileName,
        bool? IncludeImages,
        bool? IncludeAudio,
        bool? IncludeVideo);

    [Obsolete("Prefer prepare-from-manifest + confirm-from-assets (client-side ZIP). Kept for backward compatibility.")]
    [Route("/api/{locale}/stories/import-full/request-upload")]
    [Authorize]
    public static async Task<Results<Ok<ImportFullStoryRequestUploadResponse>, BadRequest<ImportFullStoryResponse>, UnauthorizedHttpResult, ForbidHttpResult, ProblemHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] ImportFullStoryRequestUploadRequest body,
        [FromServices] ImportFullStoryEndpoint ep,
        [FromServices] IDirectUploadRateLimitService rateLimit,
        CancellationToken ct)
    {
        var errors = new List<string>();

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (!rateLimit.TryAcquire(user.Id, ct))
            return TypedResults.Problem("Too many request-upload requests. Try again later.", statusCode: 429);

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);
        if (!isAdmin && !isCreator)
        {
            ep._logger.LogWarning("Full story request-upload forbidden: userId={UserId} not creator or admin", user.Id);
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(body?.FileName))
        {
            errors.Add("FileName is required.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        if (!body.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("File must be a ZIP archive.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        if (body.ExpectedSize.HasValue && body.ExpectedSize.Value > MaxZipSizeBytes)
        {
            errors.Add($"File size exceeds maximum allowed size of {MaxZipSizeBytes / (1024 * 1024)}MB");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var uploadId = Guid.NewGuid();
        var tempBlobPath = $"imports/{user.Id}/temp/{uploadId}.zip";

        var putUrl = await ep._sas.GetPutSasAsync(
            ep._sas.DraftContainer,
            tempBlobPath,
            "application/zip",
            TimeSpan.FromMinutes(ep._sasValidityMinutes),
            ct);

        return TypedResults.Ok(new ImportFullStoryRequestUploadResponse(putUrl.ToString(), uploadId));
    }

    [Obsolete("Prefer prepare-from-manifest + confirm-from-assets (client-side ZIP). Kept for backward compatibility.")]
    [Route("/api/{locale}/stories/import-full/confirm-upload")]
    [Authorize]
    public static async Task<Results<Accepted<ImportFullStoryEnqueueResponse>, BadRequest<ImportFullStoryResponse>, Conflict<ImportFullStoryResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] ImportFullStoryConfirmUploadRequest body,
        [FromServices] ImportFullStoryEndpoint ep,
        CancellationToken ct)
    {
        var errors = new List<string>();

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);
        if (!isAdmin && !isCreator)
        {
            ep._logger.LogWarning("Full story confirm-upload forbidden: userId={UserId} not creator or admin", user.Id);
            return TypedResults.Forbid();
        }

        if (body.UploadId == Guid.Empty)
        {
            errors.Add("UploadId is required.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var tempBlobPath = $"imports/{user.Id}/temp/{body.UploadId}.zip";
        var sourceBlobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, tempBlobPath);

        if (!await sourceBlobClient.ExistsAsync(ct))
        {
            errors.Add("ZIP file not found in storage. Upload may have failed or expired.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var props = await sourceBlobClient.GetPropertiesAsync(cancellationToken: ct);
        var blobSize = props.Value.ContentLength;
        if (blobSize > MaxZipSizeBytes)
        {
            try { await sourceBlobClient.DeleteIfExistsAsync(cancellationToken: ct); } catch { /* best effort */ }
            errors.Add($"Uploaded file size ({blobSize / (1024 * 1024)}MB) exceeds maximum allowed ({MaxZipSizeBytes / (1024 * 1024)}MB).");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var includeImages = body.IncludeImages ?? true;
        var includeAudio = body.IncludeAudio ?? true;
        var includeVideo = body.IncludeVideo ?? true;

        var download = await sourceBlobClient.DownloadStreamingAsync(cancellationToken: ct);
        await using var mem = new MemoryStream();
        using (var contentStream = download.Value.Content)
        {
            await contentStream.CopyToAsync(mem, ct);
        }
        mem.Position = 0;

        var manifestResult = await ep.TryReadManifestAsync(mem, ct);
        if (!manifestResult.Success)
        {
            try { await sourceBlobClient.DeleteIfExistsAsync(cancellationToken: ct); } catch { /* best effort */ }
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = manifestResult.Errors, Warnings = manifestResult.Warnings });
        }

        var finalStoryId = await ep.ResolveStoryIdConflictAsync(manifestResult.StoryId, user, isAdmin, ct);

        var tenMinutesAgo = DateTime.UtcNow.AddMinutes(-10);
        var stuckJobs = await ep._db.StoryImportJobs
            .Where(j => j.StoryId == finalStoryId &&
                       j.Status == StoryImportJobStatus.Running &&
                       j.StartedAtUtc.HasValue &&
                       j.StartedAtUtc.Value <= tenMinutesAgo)
            .ToListAsync(ct);

        if (stuckJobs.Count > 0)
        {
            foreach (var stuckJob in stuckJobs)
            {
                stuckJob.Status = StoryImportJobStatus.Failed;
                stuckJob.ErrorMessage = "Job timed out (stuck in Running status for more than 10 minutes).";
                stuckJob.CompletedAtUtc = DateTime.UtcNow;
                ep._logger.LogWarning("Marked stuck import job as Failed: jobId={JobId} storyId={StoryId}", stuckJob.Id, stuckJob.StoryId);
            }
            await ep._db.SaveChangesAsync(ct);
        }

        var activeJob = await ep._db.StoryImportJobs
            .FirstOrDefaultAsync(j => j.StoryId == finalStoryId &&
                                      (j.Status == StoryImportJobStatus.Queued || j.Status == StoryImportJobStatus.Running), ct);

        if (activeJob != null)
        {
            var message = activeJob.Status == StoryImportJobStatus.Queued
                ? "An import job is already queued for this story."
                : "An import job is currently running for this story.";
            try { await sourceBlobClient.DeleteIfExistsAsync(cancellationToken: ct); } catch { /* best effort */ }
            return TypedResults.Conflict(new ImportFullStoryResponse { Success = false, Errors = new List<string> { message } });
        }

        var jobId = Guid.NewGuid();
        var fileName = !string.IsNullOrWhiteSpace(body.FileName) ? ep.SanitizeFileName(body.FileName) : "import.zip";
        var finalBlobPath = $"imports/{user.Id}/{finalStoryId}/{jobId}/{fileName}";

        var containerClient = ep._sas.GetContainerClient(ep._sas.DraftContainer);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var destBlobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, finalBlobPath);
        var copyOp = await destBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, cancellationToken: ct);
        await copyOp.WaitForCompletionAsync(ct);

        try
        {
            await sourceBlobClient.DeleteIfExistsAsync(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            ep._logger.LogWarning(ex, "Failed to delete temp blob after copy: {Path}", tempBlobPath);
        }

        var job = new StoryImportJob
        {
            Id = jobId,
            StoryId = finalStoryId,
            OriginalStoryId = manifestResult.StoryId,
            OwnerUserId = user.Id,
            RequestedByEmail = user.Email ?? string.Empty,
            Locale = locale ?? string.Empty,
            ZipBlobPath = finalBlobPath,
            ZipFileName = fileName,
            ZipSizeBytes = blobSize,
            QueuedAtUtc = DateTime.UtcNow,
            Status = StoryImportJobStatus.Queued,
            IncludeImages = includeImages,
            IncludeAudio = includeAudio,
            IncludeVideo = includeVideo
        };

        ep._db.StoryImportJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        ep._jobEvents.Publish(JobTypes.StoryImport, job.Id, new
        {
            jobId = job.Id,
            storyId = job.StoryId,
            originalStoryId = job.OriginalStoryId,
            status = job.Status,
            queuedAtUtc = job.QueuedAtUtc
        });

        await ep._importQueue.EnqueueAsync(job, ct);

        var response = new ImportFullStoryEnqueueResponse
        {
            JobId = job.Id,
            StoryId = job.StoryId,
            OriginalStoryId = job.OriginalStoryId,
            Status = job.Status,
            QueueName = ep._importQueueName
        };

        return TypedResults.Accepted($"/api/{locale}/stories/import-full/jobs/{job.Id}", response);
    }
}
