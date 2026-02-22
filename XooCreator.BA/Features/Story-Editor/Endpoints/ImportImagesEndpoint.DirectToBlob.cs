using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

/// <summary>
/// Direct-to-blob upload: request-upload returns SAS URL; client uploads to blob; confirm-upload enqueues job.
/// </summary>
public partial class ImportImagesEndpoint
{
    private const int MaxBatchFiles = 1000;
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".webp" };

    public record ImportImagesRequestUploadRequest(string FileName, long? ExpectedSize);

    public record ImportImagesRequestUploadResponse(string PutUrl, Guid JobId, string BlobPath);

    public record ImportImagesConfirmUploadRequest(Guid JobId, string BlobPath, long? Size);

    public record ImportImagesBatchUploadFileRequest(string ClientFileId, string FileName, long? ExpectedSize, string? ContentType);
    public record ImportImagesBatchRequestUploadRequest(List<ImportImagesBatchUploadFileRequest> Files);
    public record ImportImagesBatchUploadFileResponse(string ClientFileId, string FileName, string BlobPath, string PutUrl);
    public record ImportImagesBatchRequestUploadResponse(Guid JobId, Guid UploadId, List<ImportImagesBatchUploadFileResponse> Files);
    public record ImportImagesBatchConfirmUploadRequest(Guid JobId, Guid UploadId, List<ImageImportOverride>? Overrides);

    [Route("/api/{locale}/stories/{storyId}/import-images/request-upload")]
    [Authorize]
    public static async Task<Results<Ok<ImportImagesRequestUploadResponse>, BadRequest<ImportImagesResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] ImportImagesRequestUploadRequest body,
        [FromServices] ImportImagesEndpoint ep,
        [FromServices] IDirectUploadRateLimitService rateLimit,
        CancellationToken ct)
    {
        var errors = new List<string>();

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (!rateLimit.TryAcquire(user.Id, ct))
            return TypedResults.Problem("Too many request-upload requests. Try again later.", statusCode: 429);

        if (!ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("Image import request-upload forbidden: userId={UserId} storyId={StoryId} not admin", user.Id, storyId);
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(body?.FileName))
        {
            errors.Add("FileName is required.");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        if (!body.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("File must be a ZIP archive.");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        if (body.ExpectedSize.HasValue && body.ExpectedSize.Value > ep._maxZipSizeBytes)
        {
            errors.Add($"File size exceeds maximum allowed size of {ep._maxZipSizeBytes / (1024 * 1024)}MB");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
            return TypedResults.NotFound();

        var pageOrQuizTypes = new[] { "page", "quiz", "dialog" };
        var pageTiles = craft.Tiles
            .Where(t => pageOrQuizTypes.Contains(t.Type, StringComparer.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder)
            .ToList();

        if (pageTiles.Count == 0)
        {
            errors.Add("Story has no page, quiz, or dialog tiles");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        string emailToUse = user.Email ?? string.Empty;
        if (craft.OwnerUserId != Guid.Empty && craft.OwnerUserId != user.Id)
        {
            var ownerEmail = await ep._db.AlchimaliaUsers
                .AsNoTracking()
                .Where(u => u.Id == craft.OwnerUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(ct);
            if (!string.IsNullOrWhiteSpace(ownerEmail))
            {
                emailToUse = ownerEmail;
                ep._logger.LogInformation("Admin request-upload for another user: storyId={StoryId} ownerEmail={OwnerEmail}", storyId, emailToUse);
            }
        }

        var jobId = Guid.NewGuid();
        var blobPath = $"draft/image-import/{jobId}.zip";

        var job = new StoryImageImportJob
        {
            Id = jobId,
            StoryId = storyId,
            OwnerUserId = craft.OwnerUserId,
            RequestedByEmail = user.Email ?? string.Empty,
            OwnerEmail = emailToUse,
            Locale = (locale ?? string.Empty).Trim().ToLowerInvariant(),
            ZipBlobPath = blobPath,
            Status = StoryImageImportJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        ep._db.StoryImageImportJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        var putUrl = await ep._sas.GetPutSasAsync(
            ep._sas.DraftContainer,
            blobPath,
            "application/zip",
            TimeSpan.FromMinutes(ep._sasValidityMinutes),
            ct);

        return TypedResults.Ok(new ImportImagesRequestUploadResponse(putUrl.ToString(), jobId, blobPath));
    }

    [Route("/api/{locale}/stories/{storyId}/import-images/request-upload-batch")]
    [Authorize]
    public static async Task<Results<Ok<ImportImagesBatchRequestUploadResponse>, BadRequest<ImportImagesResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] ImportImagesBatchRequestUploadRequest body,
        [FromServices] ImportImagesEndpoint ep,
        [FromServices] IDirectUploadRateLimitService rateLimit,
        CancellationToken ct)
    {
        var errors = new List<string>();
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();
        if (!rateLimit.TryAcquire(user.Id, ct))
            return TypedResults.Problem("Too many request-upload requests. Try again later.", statusCode: 429);
        if (!ep._auth0.HasRole(user, UserRole.Admin))
            return TypedResults.Forbid();
        if (!ep._enableBatchDirectUpload)
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = new List<string> { "Batch media import is disabled." } });

        var files = body?.Files ?? new List<ImportImagesBatchUploadFileRequest>();
        if (files.Count == 0)
        {
            errors.Add("At least one file is required.");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }
        if (files.Count > MaxBatchFiles)
        {
            errors.Add($"Too many files ({files.Count}). Maximum is {MaxBatchFiles}.");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
            return TypedResults.NotFound();

        var pageOrQuizTypes = new[] { "page", "quiz", "dialog" };
        var pageTiles = craft.Tiles.Where(t => pageOrQuizTypes.Contains(t.Type, StringComparer.OrdinalIgnoreCase)).OrderBy(t => t.SortOrder).ToList();
        if (pageTiles.Count == 0)
        {
            errors.Add("Story has no page, quiz, or dialog tiles");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        foreach (var file in files)
        {
            if (string.IsNullOrWhiteSpace(file.ClientFileId) || string.IsNullOrWhiteSpace(file.FileName))
            {
                errors.Add("Each file must include clientFileId and fileName.");
                continue;
            }
            var ext = Path.GetExtension(file.FileName);
            if (!AllowedImageExtensions.Contains(ext))
                errors.Add($"Unsupported image format: {file.FileName}");
            if (file.ExpectedSize.HasValue && file.ExpectedSize.Value > ep._maxZipSizeBytes)
                errors.Add($"File size exceeds limit for {file.FileName}");
        }
        if (errors.Count > 0)
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });

        string emailToUse = user.Email ?? string.Empty;
        if (craft.OwnerUserId != Guid.Empty && craft.OwnerUserId != user.Id)
        {
            var ownerEmail = await ep._db.AlchimaliaUsers.AsNoTracking().Where(u => u.Id == craft.OwnerUserId).Select(u => u.Email).FirstOrDefaultAsync(ct);
            if (!string.IsNullOrWhiteSpace(ownerEmail))
                emailToUse = ownerEmail;
        }

        var jobId = Guid.NewGuid();
        var uploadId = Guid.NewGuid();
        var stagingPrefix = $"imports/{user.Id}/temp/image-import/{uploadId}";
        var stagedFiles = new List<StagedMediaFile>();
        var responseFiles = new List<ImportImagesBatchUploadFileResponse>();
        foreach (var file in files)
        {
            var safeFileName = Path.GetFileName(file.FileName);
            var blobPath = $"{stagingPrefix}/assets/{file.ClientFileId}_{safeFileName}";
            var ext = Path.GetExtension(safeFileName).ToLowerInvariant();
            var contentType = ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
            var putUrl = await ep._sas.GetPutSasAsync(ep._sas.DraftContainer, blobPath, contentType, TimeSpan.FromMinutes(ep._sasValidityMinutes), ct);
            responseFiles.Add(new ImportImagesBatchUploadFileResponse(file.ClientFileId, safeFileName, blobPath, putUrl.ToString()));
            stagedFiles.Add(new StagedMediaFile(file.ClientFileId, safeFileName, blobPath, file.ExpectedSize, file.ContentType));
        }

        var job = new StoryImageImportJob
        {
            Id = jobId,
            StoryId = storyId,
            OwnerUserId = craft.OwnerUserId,
            RequestedByEmail = user.Email ?? string.Empty,
            OwnerEmail = emailToUse,
            Locale = (locale ?? string.Empty).Trim().ToLowerInvariant(),
            ZipBlobPath = null,
            StagingPrefix = stagingPrefix,
            BatchMappingJson = JsonSerializer.Serialize(new ImageBatchMappingPayload(stagedFiles, null)),
            Status = StoryImageImportJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };
        ep._db.StoryImageImportJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        return TypedResults.Ok(new ImportImagesBatchRequestUploadResponse(jobId, uploadId, responseFiles));
    }

    [Route("/api/{locale}/stories/{storyId}/import-images/confirm-upload")]
    [Authorize]
    public static async Task<Results<Accepted<ImageImportJobResponse>, BadRequest<ImportImagesResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] ImportImagesConfirmUploadRequest body,
        [FromServices] ImportImagesEndpoint ep,
        CancellationToken ct)
    {
        var errors = new List<string>();

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(body?.BlobPath) || body.JobId == Guid.Empty)
        {
            errors.Add("JobId and BlobPath are required.");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        var job = await ep._db.StoryImageImportJobs.FirstOrDefaultAsync(j => j.Id == body.JobId, ct);
        if (job == null)
            return TypedResults.NotFound();

        if (job.StoryId != storyId)
        {
            errors.Add("Job does not belong to this story.");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        if (job.Status != StoryImageImportJobStatus.Queued)
        {
            errors.Add($"Job is not in Queued status (current: {job.Status}).");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        if (!string.Equals(job.ZipBlobPath, body.BlobPath, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("BlobPath does not match job.");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isRequester = string.Equals(job.RequestedByEmail, user.Email, StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && !isRequester)
        {
            ep._logger.LogWarning("Confirm-upload forbidden: userId={UserId} jobId={JobId} not requester or admin", user.Id, body.JobId);
            return TypedResults.Forbid();
        }

        var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, body.BlobPath);
        if (!await blobClient.ExistsAsync(ct))
        {
            errors.Add("ZIP file not found in storage. Upload may have failed or expired.");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        var props = await blobClient.GetPropertiesAsync(cancellationToken: ct);
        var blobSize = props.Value.ContentLength;
        if (blobSize > ep._maxZipSizeBytes)
        {
            try { await blobClient.DeleteIfExistsAsync(cancellationToken: ct); } catch { /* best effort */ }
            errors.Add($"Uploaded file size ({blobSize / (1024 * 1024)}MB) exceeds maximum allowed ({ep._maxZipSizeBytes / (1024 * 1024)}MB).");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        ep._jobEvents.Publish(JobTypes.StoryImageImport, job.Id, new
        {
            jobId = job.Id,
            storyId = job.StoryId,
            status = job.Status,
            queuedAtUtc = job.QueuedAtUtc
        });

        await ep._importQueue.EnqueueAsync(job, ct);

        return TypedResults.Accepted($"/api/stories/{storyId}/image-import-jobs/{job.Id}", new ImageImportJobResponse(job.Id));
    }

    [Route("/api/{locale}/stories/{storyId}/import-images/confirm-upload-batch")]
    [Authorize]
    public static async Task<Results<Accepted<ImageImportJobResponse>, BadRequest<ImportImagesResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] ImportImagesBatchConfirmUploadRequest body,
        [FromServices] ImportImagesEndpoint ep,
        CancellationToken ct)
    {
        var errors = new List<string>();
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (body.JobId == Guid.Empty || body.UploadId == Guid.Empty)
        {
            errors.Add("JobId and UploadId are required.");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        var job = await ep._db.StoryImageImportJobs.FirstOrDefaultAsync(j => j.Id == body.JobId, ct);
        if (job == null)
            return TypedResults.NotFound();
        if (job.StoryId != storyId)
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = new List<string> { "Job does not belong to this story." } });
        if (job.Status != StoryImageImportJobStatus.Queued)
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = new List<string> { $"Job is not in Queued status (current: {job.Status})." } });
        if (string.IsNullOrWhiteSpace(job.StagingPrefix) || !job.StagingPrefix.Contains(body.UploadId.ToString(), StringComparison.OrdinalIgnoreCase))
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = new List<string> { "UploadId does not match job staging data." } });

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isRequester = string.Equals(job.RequestedByEmail, user.Email, StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && !isRequester)
            return TypedResults.Forbid();

        var staged = JsonSerializer.Deserialize<ImageBatchMappingPayload>(job.BatchMappingJson ?? string.Empty);
        var stagedFiles = staged?.Files ?? new List<StagedMediaFile>();
        if (stagedFiles.Count == 0)
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = new List<string> { "No staged files were registered for this job." } });

        foreach (var file in stagedFiles)
        {
            var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, file.BlobPath);
            if (!await blobClient.ExistsAsync(ct))
                errors.Add($"Missing uploaded file: {file.FileName}");
        }
        if (errors.Count > 0)
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });

        var mapping = new ImageBatchMappingPayload(stagedFiles, body.Overrides ?? new List<ImageImportOverride>());
        job.BatchMappingJson = JsonSerializer.Serialize(mapping);

        ep._jobEvents.Publish(JobTypes.StoryImageImport, job.Id, new
        {
            jobId = job.Id,
            storyId = job.StoryId,
            status = job.Status,
            queuedAtUtc = job.QueuedAtUtc
        });
        await ep._db.SaveChangesAsync(ct);
        await ep._importQueue.EnqueueAsync(job, ct);

        return TypedResults.Accepted($"/api/stories/{storyId}/image-import-jobs/{job.Id}", new ImageImportJobResponse(job.Id));
    }
}
