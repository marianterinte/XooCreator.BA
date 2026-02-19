using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public partial class ImportImagesEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryImageImportQueue _importQueue;
    private readonly IJobEventsHub _jobEvents;
    private readonly ILogger<ImportImagesEndpoint> _logger;

    private const long DefaultMaxZipSizeBytes = 1024L * 1024 * 1024; // 1GB
    private const long DefaultMultipartBodyLengthLimit = 1024L * 1024 * 1024; // 1GB
    private readonly long _maxZipSizeBytes;
    private readonly long _multipartBodyLengthLimit;

    public ImportImagesEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IBlobSasService sas,
        IStoryCraftsRepository crafts,
        IStoryImageImportQueue importQueue,
        IJobEventsHub jobEvents,
        ILogger<ImportImagesEndpoint> logger,
        IConfiguration configuration)
    {
        _db = db;
        _auth0 = auth0;
        _sas = sas;
        _crafts = crafts;
        _importQueue = importQueue;
        _jobEvents = jobEvents;
        _logger = logger;
        _maxZipSizeBytes = configuration.GetValue<long?>("StoryEditor:ImportImages:MaxZipSizeBytes") ?? DefaultMaxZipSizeBytes;
        _multipartBodyLengthLimit = configuration.GetValue<long?>("StoryEditor:ImportImages:MultipartBodyLengthLimit") ?? DefaultMultipartBodyLengthLimit;
    }

    public record ImageImportJobResponse(Guid JobId);

    public record ImportImagesResponse
    {
        public bool Success { get; init; }
        public List<string> Errors { get; init; } = new();
        public List<string> Warnings { get; init; } = new();
        public int ImportedCount { get; init; }
        public int TotalPages { get; init; }
    }

    [Route("/api/{locale}/stories/{storyId}/import-images")]
    [Authorize]
    [DisableRequestSizeLimit]
    public static async Task<Results<Accepted<ImageImportJobResponse>, BadRequest<ImportImagesResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ImportImagesEndpoint ep,
        HttpRequest request,
        CancellationToken ct)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isAdmin)
        {
            ep._logger.LogWarning("Image import forbidden: userId={UserId} storyId={StoryId} not admin", user.Id, storyId);
            return TypedResults.Forbid();
        }

        if (!request.HasFormContentType)
        {
            errors.Add("Request must be multipart/form-data");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        var form = await request.ReadFormAsync(new Microsoft.AspNetCore.Http.Features.FormOptions
        {
            MultipartBodyLengthLimit = ep._multipartBodyLengthLimit,
            ValueLengthLimit = int.MaxValue,
            KeyLengthLimit = int.MaxValue
        }, ct);

        var file = form.Files.GetFile("file");
        if (file == null || file.Length == 0)
        {
            errors.Add("No file provided");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        if (file.Length > ep._maxZipSizeBytes)
        {
            errors.Add($"File size exceeds maximum allowed size of {ep._maxZipSizeBytes / (1024 * 1024)}MB");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("File must be a ZIP archive");
            return TypedResults.BadRequest(new ImportImagesResponse { Success = false, Errors = errors });
        }

        // Get story craft
        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
        {
            return TypedResults.NotFound();
        }

        // Get page and quiz tiles ordered by SortOrder (quiz treated as pages for images)
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

        // Determine which email to use for asset path (owner or current user)
        string emailToUse = user.Email ?? string.Empty;
        if (isAdmin && craft.OwnerUserId != Guid.Empty && craft.OwnerUserId != user.Id)
        {
            var ownerEmail = await ep._db.AlchimaliaUsers
                .AsNoTracking()
                .Where(u => u.Id == craft.OwnerUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(ct);
            if (!string.IsNullOrWhiteSpace(ownerEmail))
            {
                emailToUse = ownerEmail;
                ep._logger.LogInformation("Admin importing images for another user: storyId={StoryId} ownerEmail={OwnerEmail}", storyId, emailToUse);
            }
        }

        var normalizedLocale = locale?.ToLowerInvariant() ?? locale;

        // Create job and upload ZIP to blob, then enqueue
        var jobId = Guid.NewGuid();
        var zipBlobPath = $"draft/image-import/{jobId}.zip";

        var job = new StoryImageImportJob
        {
            Id = jobId,
            StoryId = storyId,
            OwnerUserId = craft.OwnerUserId,
            RequestedByEmail = user.Email ?? string.Empty,
            OwnerEmail = emailToUse,
            Locale = normalizedLocale,
            ZipBlobPath = zipBlobPath,
            Status = StoryImageImportJobStatus.Queued,
            QueuedAtUtc = DateTime.UtcNow
        };

        ep._db.StoryImageImportJobs.Add(job);
        await ep._db.SaveChangesAsync(ct);

        // Upload ZIP to blob
        await using var zipStream = file.OpenReadStream();
        var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, zipBlobPath);
        await blobClient.UploadAsync(zipStream, overwrite: true, cancellationToken: ct);

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
}
