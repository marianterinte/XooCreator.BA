using System.Text;
using System.Text.Json;
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
using XooCreator.BA.Features.System.Services;
using static XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

/// <summary>
/// Client-side ZIP flow: browser unzips, uploads manifest + assets to staging; server prepares SAS URLs and confirms from staging.
/// </summary>
public partial class ImportFullStoryEndpoint
{
    /// <summary>Request for prepare-from-manifest. Manifest is the full story.json object (id, tiles, coverImageUrl, etc.).</summary>
    public record PrepareFromManifestRequest(JsonElement Manifest, bool IncludeImages = true, bool IncludeAudio = true, bool IncludeVideo = true);

    /// <summary>One put URL per asset path (path = normalized path in ZIP, used as key by client).</summary>
    public record AssetPutUrl(string Path, string PutUrl);

    public record PrepareFromManifestResponse(Guid UploadId, IReadOnlyList<AssetPutUrl> AssetPutUrls);

    public record ConfirmFromAssetsRequest(Guid UploadId, bool IncludeImages = true, bool IncludeAudio = true, bool IncludeVideo = true);

    [Route("/api/{locale}/stories/import-full/prepare-from-manifest")]
    [Authorize]
    public static async Task<Results<Ok<PrepareFromManifestResponse>, BadRequest<ImportFullStoryResponse>, UnauthorizedHttpResult, ForbidHttpResult, ProblemHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] PrepareFromManifestRequest body,
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
            ep._logger.LogWarning("Prepare-from-manifest forbidden: userId={UserId} not creator or admin", user.Id);
            return TypedResults.Forbid();
        }

        var root = body.Manifest;
        var hasId = root.TryGetProperty("id", out var idEl) && !string.IsNullOrWhiteSpace(idEl.GetString());
        var hasStoryId = root.TryGetProperty("storyId", out var storyIdEl) && !string.IsNullOrWhiteSpace(storyIdEl.GetString());
        if (!hasId && !hasStoryId)
        {
            errors.Add("Missing or empty 'id'/'storyId' in manifest.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        if (!root.TryGetProperty("tiles", out var tilesEl) || tilesEl.ValueKind != JsonValueKind.Array || tilesEl.GetArrayLength() == 0)
        {
            errors.Add("Missing or empty 'tiles' array in manifest.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var warnings = new List<string>();
        var expectedAssets = ep.CollectExpectedAssets(root, warnings, body.IncludeImages, body.IncludeAudio, body.IncludeVideo);
        if (expectedAssets.Count == 0)
        {
            errors.Add("No assets to upload (check includeImages, includeAudio, includeVideo).");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        const int maxAssets = 2000;
        if (expectedAssets.Count > maxAssets)
        {
            errors.Add($"Too many assets ({expectedAssets.Count}). Maximum is {maxAssets}.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var uploadId = Guid.NewGuid();
        var stagingPrefix = $"imports/{user.Id}/temp/{uploadId}";
        var manifestBlobPath = $"{stagingPrefix}/manifest.json";

        var manifestBytes = Encoding.UTF8.GetBytes(body.Manifest.GetRawText());
        var manifestBlobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, manifestBlobPath);
        await manifestBlobClient.UploadAsync(new BinaryData(manifestBytes), overwrite: true, cancellationToken: ct);
        var headers = new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = "application/json" };
        await manifestBlobClient.SetHttpHeadersAsync(headers, cancellationToken: ct);

        var assetPutUrls = new List<AssetPutUrl>();
        var ttl = TimeSpan.FromMinutes(ep._sasValidityMinutes);

        foreach (var (zipPath, asset) in expectedAssets)
        {
            var normalizedPath = NormalizeZipPath(zipPath);
            if (string.IsNullOrWhiteSpace(normalizedPath))
                normalizedPath = ep.BuildDefaultZipPath(asset, false);

            var extension = Path.GetExtension(asset.Filename).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                continue;

            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".mp3" => "audio/mpeg",
                ".m4a" => "audio/mp4",
                ".wav" => "audio/wav",
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                _ => "application/octet-stream"
            };

            var assetBlobPath = $"{stagingPrefix}/assets/{normalizedPath}";
            var putUrl = await ep._sas.GetPutSasAsync(ep._sas.DraftContainer, assetBlobPath, contentType, ttl, ct);
            assetPutUrls.Add(new AssetPutUrl(normalizedPath, putUrl.ToString()));
        }

        return TypedResults.Ok(new PrepareFromManifestResponse(uploadId, assetPutUrls));
    }

    [Route("/api/{locale}/stories/import-full/confirm-from-assets")]
    [Authorize]
    public static async Task<IResult> HandlePost(
        [FromRoute] string locale,
        [FromBody] ConfirmFromAssetsRequest body,
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
            ep._logger.LogWarning("Confirm-from-assets forbidden: userId={UserId} not creator or admin", user.Id);
            return TypedResults.Forbid();
        }

        if (await ep._maintenanceService.IsStoryCreatorDisabledAsync(ct))
            return StoryCreatorMaintenanceResult.Unavailable();

        if (body.UploadId == Guid.Empty)
        {
            errors.Add("UploadId is required.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var stagingPrefix = $"imports/{user.Id}/temp/{body.UploadId}";
        var manifestBlobPath = $"{stagingPrefix}/manifest.json";

        var manifestBlobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, manifestBlobPath);
        if (!await manifestBlobClient.ExistsAsync(ct))
        {
            errors.Add("Manifest not found. Complete prepare-from-manifest and upload assets first.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        string manifestJson;
        var download = await manifestBlobClient.DownloadStreamingAsync(cancellationToken: ct);
        using (var sr = new StreamReader(download.Value.Content, Encoding.UTF8))
        {
            manifestJson = await sr.ReadToEndAsync(ct);
        }

        JsonDocument manifestDoc;
        try
        {
            manifestDoc = JsonDocument.Parse(manifestJson);
        }
        catch (JsonException ex)
        {
            errors.Add($"Invalid manifest JSON: {ex.Message}");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        using (manifestDoc)
        {
            var root = manifestDoc.RootElement;
            string? originalStoryId = null;
            if (root.TryGetProperty("id", out var idEl) && !string.IsNullOrWhiteSpace(idEl.GetString()))
            {
                originalStoryId = idEl.GetString();
            }
            else if (root.TryGetProperty("storyId", out var sidEl) && !string.IsNullOrWhiteSpace(sidEl.GetString()))
            {
                originalStoryId = sidEl.GetString();
            }

            if (string.IsNullOrWhiteSpace(originalStoryId))
            {
                errors.Add("Missing or empty 'id'/'storyId' in manifest.");
                return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
            }

            var safeOriginalStoryId = originalStoryId!;
            var finalStoryId = await ep.ResolveStoryIdConflictAsync(safeOriginalStoryId, user, isAdmin, ct);

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
                return TypedResults.Conflict(new ImportFullStoryResponse { Success = false, Errors = new List<string> { message } });
            }

            var jobId = Guid.NewGuid();
            var job = new StoryImportJob
            {
                Id = jobId,
                StoryId = finalStoryId,
                OriginalStoryId = safeOriginalStoryId,
                OwnerUserId = user.Id,
                RequestedByEmail = user.Email ?? string.Empty,
                Locale = locale ?? string.Empty,
                ZipBlobPath = null,
                StagingPrefix = stagingPrefix,
                ManifestBlobPath = manifestBlobPath,
                ZipFileName = string.Empty,
                ZipSizeBytes = 0,
                QueuedAtUtc = DateTime.UtcNow,
                Status = StoryImportJobStatus.Queued,
                IncludeImages = body.IncludeImages,
                IncludeAudio = body.IncludeAudio,
                IncludeVideo = body.IncludeVideo
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
}
