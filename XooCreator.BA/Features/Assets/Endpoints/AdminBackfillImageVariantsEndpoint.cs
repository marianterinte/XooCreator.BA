using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Images;

namespace XooCreator.BA.Features.Assets.Endpoints;

[Endpoint]
public sealed class AdminBackfillImageVariantsEndpoint
{
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly IImageCompressionService _imageCompression;
    private readonly IOptions<ImageCompressionOptions> _options;
    private readonly ILogger<AdminBackfillImageVariantsEndpoint> _logger;

    public AdminBackfillImageVariantsEndpoint(
        IAuth0UserService auth0,
        IBlobSasService sas,
        IImageCompressionService imageCompression,
        IOptions<ImageCompressionOptions> options,
        ILogger<AdminBackfillImageVariantsEndpoint> logger)
    {
        _auth0 = auth0;
        _sas = sas;
        _imageCompression = imageCompression;
        _options = options;
        _logger = logger;
    }

    public sealed record BackfillRequest(
        int? MaxImages = null,
        bool OverwriteExisting = false,
        string Prefix = "images/");

    public sealed record BackfillResponse
    {
        public int Scanned { get; init; }
        public int Attempted { get; init; }
        public int Succeeded { get; init; }
        public int Failed { get; init; }
        public int SkippedBecauseNotFourByFive { get; init; }
        public int SkippedDerivedVariantPath { get; init; }
        public List<string> Errors { get; init; } = new();
    }

    [Route("/api/admin/image-compression/backfill-images")]
    [Authorize]
    public static async Task<Results<Ok<BackfillResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] AdminBackfillImageVariantsEndpoint ep,
        [FromBody] BackfillRequest? request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }
        if (!ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var opt = ep._options.Value;
        // If MaxImages is explicitly set, use it. If null, process all images (no limit)
        // For backward compatibility: if request is null, use default from config
        int? limit = null;
        bool hasLimit = true;
        if (request?.MaxImages.HasValue == true)
        {
            limit = Math.Max(1, request.MaxImages.Value);
            hasLimit = true;
        }
        else if (request == null)
        {
            // Backward compatibility: if no request body, use config default
            limit = Math.Max(1, opt.ProcessBatchSize);
            hasLimit = true;
        }
        else
        {
            // MaxImages is explicitly null in request - process all images
            hasLimit = false;
        }

        var prefix = (request?.Prefix ?? "images/").Trim().TrimStart('/');
        if (!prefix.EndsWith("/")) prefix += "/";

        var overwrite = request?.OverwriteExisting ?? false;

        var resp = new BackfillResponse();
        var container = ep._sas.GetContainerClient(ep._sas.PublishedContainer);

        var scanned = 0;
        var attempted = 0;
        var succeeded = 0;
        var failed = 0;
        var skippedNot45 = 0;
        var skippedDerived = 0;
        var errors = new List<string>();

        ep._logger.LogInformation("Starting image variants backfill: prefix={Prefix} limit={Limit} overwrite={Overwrite}", prefix, hasLimit ? limit.ToString() : "unlimited", overwrite);

        await foreach (var blob in container.GetBlobsAsync(prefix: prefix, cancellationToken: ct))
        {
            scanned++;

            if (hasLimit && attempted >= limit)
            {
                break;
            }

            var name = blob.Name;
            if (IsDerivedVariantPath(name))
            {
                skippedDerived++;
                continue;
            }

            if (!IsSupportedImageFile(name))
            {
                continue;
            }

            attempted++;

            try
            {
                var (basePath, fileName) = SplitPath(name);
                var result = await ep._imageCompression.EnsureStorySizeVariantsAsync(
                    sourceBlobPath: name,
                    targetBasePath: basePath,
                    filename: fileName,
                    overwriteExisting: overwrite,
                    ct: ct);

                if (!result.Success)
                {
                    failed++;
                    errors.Add($"{name}: {result.ErrorMessage}");
                    continue;
                }

                if (result.SkippedBecauseNotFourByFive)
                {
                    skippedNot45++;
                }

                succeeded++;
            }
            catch (Exception ex)
            {
                failed++;
                errors.Add($"{name}: {ex.Message}");
            }
        }

        ep._logger.LogInformation(
            "Image variants backfill completed: scanned={Scanned} attempted={Attempted} succeeded={Succeeded} failed={Failed} skippedNot45={SkippedNot45} skippedDerived={SkippedDerived}",
            scanned, attempted, succeeded, failed, skippedNot45, skippedDerived);

        return TypedResults.Ok(resp with
        {
            Scanned = scanned,
            Attempted = attempted,
            Succeeded = succeeded,
            Failed = failed,
            SkippedBecauseNotFourByFive = skippedNot45,
            SkippedDerivedVariantPath = skippedDerived,
            Errors = errors
        });
    }

    private static bool IsSupportedImageFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".png" or ".jpg" or ".jpeg" or ".webp";
    }

    private static bool IsDerivedVariantPath(string blobPath)
    {
        // Our convention is to insert /s/ or /m/ right before the filename.
        // This method skips any blob that already contains those segments.
        return blobPath.Contains("/s/", StringComparison.OrdinalIgnoreCase) ||
               blobPath.Contains("/m/", StringComparison.OrdinalIgnoreCase);
    }

    private static (string BasePath, string FileName) SplitPath(string blobPath)
    {
        var trimmed = (blobPath ?? string.Empty).Trim().TrimStart('/');
        var idx = trimmed.LastIndexOf('/');
        if (idx < 0)
        {
            return (string.Empty, trimmed);
        }
        var basePath = trimmed.Substring(0, idx);
        var fileName = trimmed.Substring(idx + 1);
        return (basePath, fileName);
    }
}


