using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Jobs;
using XooCreator.BA.Infrastructure.Services.Queue;
using static XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public partial class ImportFullStoryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryEditorService _editorService;
    private readonly ILogger<ImportFullStoryEndpoint> _logger;
    private readonly TelemetryClient? _telemetryClient;
    private readonly string _importQueueName;
    private readonly IStoryImportQueue _importQueue;
    private readonly IJobEventsHub _jobEvents;
    private const long MaxZipSizeBytes = 500 * 1024 * 1024; // 500MB
    private const long MaxAssetSizeBytes = 50 * 1024 * 1024; // 50MB per asset
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".webp", // Images
        ".mp3", ".m4a", ".wav", // Audio
        ".mp4", ".webm" // Video
    };

    public ImportFullStoryEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IBlobSasService sas,
        IStoryCraftsRepository crafts,
        IStoryEditorService editorService,
        IConfiguration configuration,
        ILogger<ImportFullStoryEndpoint> logger,
        IStoryImportQueue importQueue,
        IJobEventsHub jobEvents,
        TelemetryClient? telemetryClient = null)
    {
        _db = db;
        _auth0 = auth0;
        _sas = sas;
        _crafts = crafts;
        _editorService = editorService;
        _importQueueName = configuration.GetSection("AzureStorage:Queues")?["Import"] ?? "story-import-queue";
        _logger = logger;
        _importQueue = importQueue;
        _jobEvents = jobEvents;
        _telemetryClient = telemetryClient;
    }

    public record ImportFullStoryResponse
    {
        public string StoryId { get; init; } = string.Empty;
        public bool Success { get; init; }
        public List<string> Warnings { get; init; } = new();
        public List<string> Errors { get; init; } = new();
        public int ImportedAssets { get; init; }
        public int TotalAssets { get; init; }
        public List<string> ImportedLanguages { get; init; } = new();
    }

    public record ImportFullStoryEnqueueResponse
    {
        public Guid JobId { get; init; }
        public string StoryId { get; init; } = string.Empty;
        public string OriginalStoryId { get; init; } = string.Empty;
        public string Status { get; init; } = StoryImportJobStatus.Queued;
        public string QueueName { get; init; } = string.Empty;
    }

    public record ImportJobStatusResponse
    {
        public Guid JobId { get; init; }
        public string StoryId { get; init; } = string.Empty;
        public string OriginalStoryId { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime QueuedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
        public int ImportedAssets { get; init; }
        public int TotalAssets { get; init; }
        public int ImportedLanguagesCount { get; init; }
        public string? ErrorMessage { get; init; }
        public string? WarningSummary { get; init; }
        public int DequeueCount { get; init; }
    }

    [Route("/api/{locale}/stories/import-full")]
    [Authorize]
    [DisableRequestSizeLimit] // Disable request size limit for this endpoint (allows up to 500MB as per MaxZipSizeBytes)
    public static async Task<Results<Accepted<ImportFullStoryEnqueueResponse>, BadRequest<ImportFullStoryResponse>, Conflict<ImportFullStoryResponse>>> HandlePost(
        [FromRoute] string locale,
        [FromServices] ImportFullStoryEndpoint ep,
        HttpRequest request,
        CancellationToken ct)
    {
        var errors = new List<string>();

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            errors.Add("Authentication required. Please log in to import a story.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);
        if (!isAdmin && !isCreator)
        {
            errors.Add("You do not have permission to import stories. Only users with Creator or Admin role can import stories.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        if (!request.HasFormContentType)
        {
            errors.Add("Request must be multipart/form-data");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var feature = request.HttpContext.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpMaxRequestBodySizeFeature>();
        if (feature != null && !feature.IsReadOnly)
        {
            feature.MaxRequestBodySize = null;
        }

        var form = await request.ReadFormAsync(new Microsoft.AspNetCore.Http.Features.FormOptions
        {
            MultipartBodyLengthLimit = 600 * 1024 * 1024,
            ValueLengthLimit = int.MaxValue,
            KeyLengthLimit = int.MaxValue
        }, ct);

        var file = form.Files.GetFile("file");
        if (file == null || file.Length == 0)
        {
            errors.Add("No file provided");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        // Read media options from form (default to true if not provided)
        var includeImages = true;
        var includeAudio = true;
        var includeVideo = true;
        
        if (form.TryGetValue("includeImages", out var imagesValue) && bool.TryParse(imagesValue, out var imagesBool))
        {
            includeImages = imagesBool;
        }
        if (form.TryGetValue("includeAudio", out var audioValue) && bool.TryParse(audioValue, out var audioBool))
        {
            includeAudio = audioBool;
        }
        if (form.TryGetValue("includeVideo", out var videoValue) && bool.TryParse(videoValue, out var videoBool))
        {
            includeVideo = videoBool;
        }

        if (file.Length > MaxZipSizeBytes)
        {
            errors.Add($"File size exceeds maximum allowed size of {MaxZipSizeBytes / (1024 * 1024)}MB");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("File must be a ZIP archive");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        await using var zipStream = file.OpenReadStream();
        if (!zipStream.CanSeek)
        {
            errors.Add("Unable to process file stream for import.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var manifestResult = await ep.TryReadManifestAsync(zipStream, ct);
        if (!manifestResult.Success)
        {
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = manifestResult.Errors, Warnings = manifestResult.Warnings });
        }

        zipStream.Position = 0;

        var finalStoryId = await ep.ResolveStoryIdConflictAsync(manifestResult.StoryId, user, isAdmin, ct);

        // Check for stuck jobs (Running for more than 10 minutes) and mark them as Failed
        var tenMinutesAgo = DateTime.UtcNow.AddMinutes(-10);
        var stuckJobs = await ep._db.StoryImportJobs
            .Where(j => j.StoryId == finalStoryId &&
                       j.Status == StoryImportJobStatus.Running &&
                       j.StartedAtUtc.HasValue &&
                       j.StartedAtUtc.Value <= tenMinutesAgo)
            .ToListAsync(ct);

        if (stuckJobs.Any())
        {
            foreach (var stuckJob in stuckJobs)
            {
                stuckJob.Status = StoryImportJobStatus.Failed;
                stuckJob.ErrorMessage = "Job timed out (stuck in Running status for more than 10 minutes).";
                stuckJob.CompletedAtUtc = DateTime.UtcNow;
                ep._logger.LogWarning("Marked stuck import job as Failed: jobId={JobId} storyId={StoryId} startedAt={StartedAt}",
                    stuckJob.Id, stuckJob.StoryId, stuckJob.StartedAtUtc);
            }
            await ep._db.SaveChangesAsync(ct);
        }

        // Check for active jobs (Queued or Running)
        var activeJob = await ep._db.StoryImportJobs
            .FirstOrDefaultAsync(j => j.StoryId == finalStoryId &&
                                      (j.Status == StoryImportJobStatus.Queued || j.Status == StoryImportJobStatus.Running), ct);

        if (activeJob != null)
        {
            var jobStatus = activeJob.Status == StoryImportJobStatus.Queued 
                ? "queued" 
                : "currently running";
            var queuedTime = activeJob.QueuedAtUtc.ToString("yyyy-MM-dd HH:mm:ss UTC");
            var message = $"Cannot import story '{finalStoryId}'. An import job is already {jobStatus} for this story (queued at {queuedTime}). " +
                         $"Please wait up to 10 minutes for the current import to complete, or check the import status before trying again. " +
                         $"If the import appears to be stuck, you can retry after 10 minutes.";
            return TypedResults.Conflict(new ImportFullStoryResponse 
            { 
                Success = false, 
                Errors = new List<string> { message } 
            });
        }

        var jobId = Guid.NewGuid();
        var sanitizedFileName = ep.SanitizeFileName(file.FileName);
        var blobPath = $"imports/{user.Id}/{finalStoryId}/{jobId}/{sanitizedFileName}";

        var containerClient = ep._sas.GetContainerClient(ep._sas.DraftContainer);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, blobPath);
        await blobClient.UploadAsync(zipStream, overwrite: true, cancellationToken: ct);

        var job = new StoryImportJob
        {
            Id = jobId,
            StoryId = finalStoryId,
            OriginalStoryId = manifestResult.StoryId,
            OwnerUserId = user.Id,
            RequestedByEmail = user.Email,
            Locale = locale,
            ZipBlobPath = blobPath,
            ZipFileName = sanitizedFileName,
            ZipSizeBytes = file.Length,
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
            queuedAtUtc = job.QueuedAtUtc,
            startedAtUtc = job.StartedAtUtc,
            completedAtUtc = job.CompletedAtUtc,
            importedAssets = job.ImportedAssets,
            totalAssets = job.TotalAssets,
            importedLanguagesCount = job.ImportedLanguagesCount,
            errorMessage = job.ErrorMessage,
            warningSummary = job.WarningSummary,
            dequeueCount = job.DequeueCount
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

    private async Task<ManifestReadResult> TryReadManifestAsync(Stream zipStream, CancellationToken ct)
    {
        var warnings = new List<string>();
        var errors = new List<string>();

        try
        {
            zipStream.Position = 0;
            using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);

            var manifestEntry = zip.Entries.FirstOrDefault(e =>
                e.FullName.Contains("manifest/", StringComparison.OrdinalIgnoreCase) &&
                e.FullName.EndsWith("story.json", StringComparison.OrdinalIgnoreCase));

            if (manifestEntry == null)
            {
                errors.Add("Manifest file (manifest/{storyId}/story.json or manifest/{storyId}/v{version}/story.json) not found in ZIP");
                return ManifestReadResult.Failed(errors, warnings);
            }

            string jsonContent;
            await using (var manifestStream = manifestEntry.Open())
            using (var reader = new StreamReader(manifestStream, Encoding.UTF8))
            {
                jsonContent = await reader.ReadToEndAsync(ct);
            }

            JsonDocument jsonDoc;
            try
            {
                jsonDoc = JsonDocument.Parse(jsonContent);
            }
            catch (JsonException ex)
            {
                errors.Add($"Invalid JSON in manifest: {ex.Message}");
                return ManifestReadResult.Failed(errors, warnings);
            }

            var root = jsonDoc.RootElement;
            if (!root.TryGetProperty("id", out var idElement) || string.IsNullOrWhiteSpace(idElement.GetString()))
            {
                errors.Add("Missing or empty 'id' field in manifest");
                return ManifestReadResult.Failed(errors, warnings);
            }

            var originalStoryId = idElement.GetString()!;

            var manifestPath = manifestEntry.FullName;
            var storyIdMatch = Regex.Match(manifestPath, @"manifest/([^/]+)(?:/v\d+)?/story\.json", RegexOptions.IgnoreCase);
            if (storyIdMatch.Success)
            {
                var pathStoryId = storyIdMatch.Groups[1].Value;
                if (!string.Equals(pathStoryId, originalStoryId, StringComparison.OrdinalIgnoreCase))
                {
                    warnings.Add($"StoryId in path ('{pathStoryId}') doesn't match JSON ID ('{originalStoryId}'). Using JSON ID.");
                }
            }

            if (!root.TryGetProperty("tiles", out var tilesElement) || tilesElement.ValueKind != JsonValueKind.Array || tilesElement.GetArrayLength() == 0)
            {
                errors.Add("Missing or empty 'tiles' array in manifest");
                return ManifestReadResult.Failed(errors, warnings);
            }

            return ManifestReadResult.Successful(originalStoryId, errors, warnings);
        }
        catch (Exception ex)
        {
            errors.Add($"Failed to read manifest: {ex.Message}");
            return ManifestReadResult.Failed(errors, warnings);
        }
    }

    private string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName);
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        return string.IsNullOrWhiteSpace(name) ? $"import-{DateTime.UtcNow:yyyyMMddHHmmss}.zip" : name;
    }

    private void TrackImportTelemetry(
        long durationMs,
        string outcome,
        string locale,
        string? storyId,
        Guid? userId,
        int totalAssets,
        int uploadedAssets,
        long referencedBytes,
        long uploadedBytes,
        int warningsCount,
        int errorsCount)
    {
        var referencedMb = referencedBytes / 1024d / 1024d;
        var uploadedMb = uploadedBytes / 1024d / 1024d;

        _logger.LogInformation(
            "Import full story telemetry | storyId={StoryId} locale={Locale} outcome={Outcome} durationMs={DurationMs} totalAssets={TotalAssets} uploadedAssets={UploadedAssets} referencedSizeMB={ReferencedSizeMB:F2} uploadedSizeMB={UploadedSizeMB:F2} warnings={Warnings} errors={Errors}",
            storyId ?? "(none)",
            locale,
            outcome,
            durationMs,
            totalAssets,
            uploadedAssets,
            referencedMb,
            uploadedMb,
            warningsCount,
            errorsCount);

        var properties = new Dictionary<string, string?>
        {
            ["Outcome"] = outcome,
            ["Locale"] = locale,
            ["StoryId"] = storyId,
            ["UserId"] = userId?.ToString(),
            ["Warnings"] = warningsCount.ToString(CultureInfo.InvariantCulture),
            ["Errors"] = errorsCount.ToString(CultureInfo.InvariantCulture)
        };

        TrackMetric("ImportFullStory_Duration", durationMs, properties);
        TrackMetric("ImportFullStory_AssetsCount", totalAssets, properties);
        TrackMetric("ImportFullStory_UploadedAssets", uploadedAssets, properties);

        if (referencedBytes > 0)
        {
            TrackMetric("ImportFullStory_ReferencedSizeMB", referencedMb, properties);
        }

        if (uploadedBytes > 0)
        {
            TrackMetric("ImportFullStory_TotalSizeMB", uploadedMb, properties);
        }
    }

    private void TrackMetric(string metricName, double value, IReadOnlyDictionary<string, string?> properties)
    {
        if (_telemetryClient == null)
        {
            return;
        }

        var metric = new MetricTelemetry(metricName, value);
        foreach (var kvp in properties)
        {
            if (!string.IsNullOrEmpty(kvp.Value))
            {
                metric.Properties[kvp.Key] = kvp.Value!;
            }
        }

        _telemetryClient.TrackMetric(metric);
    }

    private readonly record struct ManifestReadResult(bool Success, string StoryId, List<string> Errors, List<string> Warnings)
    {
        public static ManifestReadResult Successful(string storyId, List<string> errors, List<string> warnings)
            => new(true, storyId, errors, warnings);

        public static ManifestReadResult Failed(List<string> errors, List<string> warnings)
            => new(false, string.Empty, errors, warnings);
    }

    private async Task<string> ResolveStoryIdConflictAsync(string storyId, AlchimaliaUser user, bool isAdmin, CancellationToken ct)
    {
        var existing = await _crafts.GetAsync(storyId, ct);

        if (existing == null)
        {
            return storyId; // No conflict
        }

        // Admin can overwrite anything
        if (isAdmin)
        {
            _logger.LogInformation("Admin overwriting storyId={StoryId}", storyId);
            return storyId;
        }

        // If user is owner, allow overwrite
        if (existing.OwnerUserId == user.Id)
        {
            _logger.LogInformation("Owner overwriting storyId={StoryId}", storyId);
            return storyId;
        }

        // Generate new ID
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var newStoryId = $"{storyId}-import-{timestamp}";
        _logger.LogInformation("StoryId conflict resolved: {OriginalId} -> {NewId}", storyId, newStoryId);
        return newStoryId;
    }

    private List<(string ZipPath, AssetInfo Asset)> CollectExpectedAssets(JsonElement root, List<string> warnings, bool includeImages = true, bool includeAudio = true, bool includeVideo = true)
    {
        var assets = new List<(string, AssetInfo)>();

        // Cover image
        if (includeImages && root.TryGetProperty("coverImageUrl", out var coverElement) && coverElement.ValueKind == JsonValueKind.String)
        {
            var coverPath = coverElement.GetString();
            if (!string.IsNullOrWhiteSpace(coverPath))
            {
                var filename = ExtractFilename(coverPath);
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    assets.Add(CreateAssetEntry(coverPath, new AssetInfo(filename, AssetType.Image, null), isCoverImage: true));
                }
            }
        }

        // Tile assets
        if (root.TryGetProperty("tiles", out var tilesElement) && tilesElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var tile in tilesElement.EnumerateArray())
            {
                // Tile image
                if (includeImages && tile.TryGetProperty("imageUrl", out var imageElement) && imageElement.ValueKind == JsonValueKind.String)
                {
                    var imagePath = imageElement.GetString();
                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        var filename = ExtractFilename(imagePath);
                        if (!string.IsNullOrWhiteSpace(filename))
                        {
                            assets.Add(CreateAssetEntry(imagePath, new AssetInfo(filename, AssetType.Image, null)));
                        }
                    }
                }

                // Tile translations (audio/video)
                if (tile.TryGetProperty("translations", out var translationsElement) && translationsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var translation in translationsElement.EnumerateArray())
                    {
                        var lang = translation.TryGetProperty("lang", out var langElement)
                            ? langElement.GetString()
                            : null;
                        var normalizedLang = string.IsNullOrWhiteSpace(lang)
                            ? null
                            : lang!.Trim().ToLowerInvariant();

                        // Audio
                        if (includeAudio && translation.TryGetProperty("audioUrl", out var audioElement) && audioElement.ValueKind == JsonValueKind.String)
                        {
                            var audioPath = audioElement.GetString();
                            if (!string.IsNullOrWhiteSpace(audioPath) && !string.IsNullOrWhiteSpace(normalizedLang))
                            {
                                var filename = ExtractFilename(audioPath);
                                if (!string.IsNullOrWhiteSpace(filename))
                                {
                                    assets.Add(CreateAssetEntry(audioPath, new AssetInfo(filename, AssetType.Audio, normalizedLang)));
                                }
                            }
                        }

                        // Video
                        if (includeVideo && translation.TryGetProperty("videoUrl", out var videoElement) && videoElement.ValueKind == JsonValueKind.String)
                        {
                            var videoPath = videoElement.GetString();
                            if (!string.IsNullOrWhiteSpace(videoPath) && !string.IsNullOrWhiteSpace(normalizedLang))
                            {
                                var filename = ExtractFilename(videoPath);
                                if (!string.IsNullOrWhiteSpace(filename))
                                {
                                    assets.Add(CreateAssetEntry(videoPath, new AssetInfo(filename, AssetType.Video, normalizedLang)));
                                }
                            }
                        }
                    }
                }
            }
        }

        return assets;
    }

    private long CalculateReferencedAssetBytes(
        ZipArchive zip,
        List<(string ZipPath, AssetInfo Asset)> expectedAssets)
    {
        long total = 0;
        foreach (var (zipPath, _) in expectedAssets)
        {
            var normalizedZipPath = NormalizeZipPath(zipPath);
            var entry = zip.Entries.FirstOrDefault(e => ContainsPath(e, normalizedZipPath));
            if (entry != null)
            {
                total += entry.Length;
            }
        }

        return total;
    }

    private (string ZipPath, AssetInfo Asset) CreateAssetEntry(string? manifestPath, AssetInfo asset, bool isCoverImage = false)
    {
        var normalizedPath = NormalizeZipPath(manifestPath);
        if (string.IsNullOrWhiteSpace(normalizedPath) || !normalizedPath.Contains('/'))
        {
            normalizedPath = BuildDefaultZipPath(asset, isCoverImage);
        }

        return (normalizedPath, asset);
    }

    private static string NormalizeZipPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        var normalized = path
            .Replace('\\', '/')
            .TrimStart('.', '/')
            .Trim();

        while (normalized.Contains("//", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("//", "/", StringComparison.Ordinal);
        }

        return normalized;
    }

    private string BuildDefaultZipPath(AssetInfo asset, bool isCoverImage)
    {
        var mediaType = asset.Type switch
        {
            AssetType.Image => "images",
            AssetType.Audio => "audio",
            AssetType.Video => "video",
            _ => "images"
        };

        if (asset.Type == AssetType.Image)
        {
            return isCoverImage
                ? $"media/{mediaType}/cover/{asset.Filename}"
                : $"media/{mediaType}/tiles/{asset.Filename}";
        }

        if (!string.IsNullOrWhiteSpace(asset.Lang))
        {
            return $"media/{mediaType}/{asset.Lang}/{asset.Filename}";
        }

        return $"media/{mediaType}/{asset.Filename}";
    }

    private string ExtractFilename(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;

        // Remove "media/" prefix if present
        var cleanPath = path.StartsWith("media/", StringComparison.OrdinalIgnoreCase)
            ? path.Substring(6)
            : path;

        // Extract filename (last part after /)
        var parts = cleanPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[^1] : cleanPath;
    }

    private async Task<AssetUploadSummary> UploadAssetsFromZipAsync(
        ZipArchive zip,
        List<(string ZipPath, AssetInfo Asset)> expectedAssets,
        string userEmail,
        string storyId,
        List<string> warnings,
        List<string> errors,
        List<string> uploadedBlobPaths,
        CancellationToken ct)
    {
        var uploadedCount = 0;
        var uploadedBytes = 0L;
        var containerClient = _sas.GetContainerClient(_sas.DraftContainer);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        foreach (var (zipPath, asset) in expectedAssets)
        {
            try
            {
                var normalizedZipPath = NormalizeZipPath(zipPath);

                // Find asset in ZIP using the full normalized path (exact match only)
                var zipEntry = zip.Entries.FirstOrDefault(e =>
                    ContainsPath(e, normalizedZipPath));

                if (zipEntry == null)
                {
                    var expectedPath = string.IsNullOrWhiteSpace(normalizedZipPath) ? asset.Filename : normalizedZipPath;
                    warnings.Add($"Asset '{asset.Filename}' referenced in JSON but not found in ZIP (expected '{expectedPath}')");
                    continue;
                }

                // Validate extension
                var extension = Path.GetExtension(asset.Filename);
                if (!AllowedExtensions.Contains(extension))
                {
                    warnings.Add($"Asset '{asset.Filename}' has unsupported extension '{extension}'");
                    continue;
                }

                // Validate size
                if (zipEntry.Length > MaxAssetSizeBytes)
                {
                    warnings.Add($"Asset '{asset.Filename}' exceeds maximum size of {MaxAssetSizeBytes / (1024 * 1024)}MB");
                    continue;
                }

                // Build blob path
                var blobPath = BuildDraftPath(asset, userEmail, storyId);

                // Extract content
                await using var entryStream = zipEntry.Open();
                using var memoryStream = new MemoryStream();
                await entryStream.CopyToAsync(memoryStream, ct);
                memoryStream.Position = 0;

                // Determine content type
                var contentType = extension.ToLowerInvariant() switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".webp" => "image/webp",
                    ".mp3" => "audio/mpeg",
                    ".m4a" => "audio/mp4",
                    ".wav" => "audio/wav",
                    ".mp4" => "video/mp4",
                    ".webm" => "video/webm",
                    _ => "application/octet-stream"
                };

                // Upload to blob storage
                var blobClient = _sas.GetBlobClient(_sas.DraftContainer, blobPath);
                await blobClient.UploadAsync(memoryStream, overwrite: true, cancellationToken: ct);

                // Set content type
                var headers = new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType };
                await blobClient.SetHttpHeadersAsync(headers, cancellationToken: ct);

                uploadedBlobPaths.Add(blobPath);
                uploadedCount++;
                uploadedBytes += memoryStream.Length;

                _logger.LogDebug("Uploaded asset: {BlobPath} ({Size} bytes)", blobPath, memoryStream.Length);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to upload asset: {Filename}", asset.Filename);
                warnings.Add($"Failed to upload asset '{asset.Filename}': {ex.Message}");
            }
        }

        return new AssetUploadSummary(uploadedCount, uploadedBytes);
    }

    private readonly record struct AssetUploadSummary(int UploadedAssets, long UploadedBytes);

    private static bool ContainsPath(ZipArchiveEntry e, string normalizedZipPath)
    {
        var path = NormalizeZipPath(e.FullName);
        var result= string.Equals(path, normalizedZipPath, StringComparison.OrdinalIgnoreCase) ||
            path.Contains(normalizedZipPath);
        ;
        return result;
    }

    private async Task RollbackAssetsAsync(List<string> blobPaths, CancellationToken ct)
    {
        foreach (var blobPath in blobPaths)
        {
            try
            {
                var blobClient = _sas.GetBlobClient(_sas.DraftContainer, blobPath);
                await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
                _logger.LogDebug("Rolled back asset: {BlobPath}", blobPath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to rollback asset: {BlobPath}", blobPath);
            }
        }
    }

    private List<string> ExtractLanguages(JsonElement root)
    {
        var languages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // From translations
        if (root.TryGetProperty("translations", out var translationsElement) && translationsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var translation in translationsElement.EnumerateArray())
            {
                if (translation.TryGetProperty("lang", out var langElement))
                {
                    var lang = langElement.GetString();
                    if (!string.IsNullOrWhiteSpace(lang))
                    {
                        languages.Add(lang);
                    }
                }
            }
        }

        // From tile translations
        if (root.TryGetProperty("tiles", out var tilesElement) && tilesElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var tile in tilesElement.EnumerateArray())
            {
                if (tile.TryGetProperty("translations", out var tileTranslationsElement) && tileTranslationsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var translation in tileTranslationsElement.EnumerateArray())
                    {
                        if (translation.TryGetProperty("lang", out var langElement))
                        {
                            var lang = langElement.GetString();
                            if (!string.IsNullOrWhiteSpace(lang))
                            {
                                languages.Add(lang);
                            }
                        }
                    }
                }
            }
        }

        return languages.ToList();
    }

    private async Task CreateStoryCraftFromJsonAsync(
        JsonElement root,
        Guid ownerUserId,
        string storyId,
        List<string> warnings,
        CancellationToken ct)
    {
        // Check if craft already exists (for overwrite scenario)
        var existing = await _crafts.GetAsync(storyId, ct);
        if (existing != null && existing.OwnerUserId == ownerUserId)
        {
            // Delete existing craft and related entities
            _db.StoryCrafts.Remove(existing);
            await _db.SaveChangesAsync(ct);
        }

        // Extract basic fields
        var title = root.TryGetProperty("title", out var titleElement) ? titleElement.GetString() ?? string.Empty : string.Empty;
        var summary = root.TryGetProperty("summary", out var summaryElement) ? summaryElement.GetString() : null;
        var storyType = root.TryGetProperty("storyType", out var typeElement) && typeElement.ValueKind == JsonValueKind.Number
            ? (StoryType)typeElement.GetInt32()
            : StoryType.Indie;
        var coverImageUrl = root.TryGetProperty("coverImageUrl", out var coverElement) ? ExtractFilename(coverElement.GetString() ?? "") : null;
        var storyTopic = root.TryGetProperty("storyTopic", out var topicElement) ? topicElement.GetString() : null;
        var authorName = root.TryGetProperty("authorName", out var authorElement) ? authorElement.GetString() : null;
        var classicAuthorId = root.TryGetProperty("classicAuthorId", out var classicAuthorElement) && classicAuthorElement.ValueKind == JsonValueKind.String
            ? Guid.TryParse(classicAuthorElement.GetString(), out var guid) ? guid : (Guid?)null
            : null;
        var priceInCredits = root.TryGetProperty("priceInCredits", out var priceElement) && priceElement.ValueKind == JsonValueKind.Number
            ? priceElement.GetDouble()
            : 0.0;
        var baseVersion = root.TryGetProperty("version", out var versionElement) && versionElement.ValueKind == JsonValueKind.Number
            ? versionElement.GetInt32()
            : 0;
        var isEvaluative = root.TryGetProperty("isEvaluative", out var isEvaluativeElement) && isEvaluativeElement.ValueKind == JsonValueKind.True
            ? true
            : false;

        // Create StoryCraft
        var craft = new StoryCraft
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            OwnerUserId = ownerUserId,
            Status = StoryStatus.Draft.ToDb(),
            CoverImageUrl = coverImageUrl,
            StoryTopic = storyTopic,
            AuthorName = authorName,
            ClassicAuthorId = classicAuthorId,
            StoryType = storyType,
            PriceInCredits = priceInCredits,
            BaseVersion = baseVersion,
            IsEvaluative = isEvaluative,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add translations
        if (root.TryGetProperty("translations", out var translationsElement) && translationsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var translation in translationsElement.EnumerateArray())
            {
                var lang = translation.TryGetProperty("lang", out var langElement) ? langElement.GetString() : null;
                var transTitle = translation.TryGetProperty("title", out var transTitleElement) ? transTitleElement.GetString() : null;
                var transSummary = translation.TryGetProperty("summary", out var transSummaryElement) ? transSummaryElement.GetString() : null;

                if (!string.IsNullOrWhiteSpace(lang))
                {
                    craft.Translations.Add(new StoryCraftTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryCraftId = craft.Id,
                        LanguageCode = lang,
                        Title = transTitle ?? title,
                        Summary = transSummary ?? summary
                    });
                }
            }
        }

        // Add tiles
        if (root.TryGetProperty("tiles", out var tilesElement) && tilesElement.ValueKind == JsonValueKind.Array)
        {
            var sortOrder = 0;
            foreach (var tile in tilesElement.EnumerateArray())
            {
                var tileId = tile.TryGetProperty("id", out var tileIdElement) ? tileIdElement.GetString() ?? $"tile-{sortOrder}" : $"tile-{sortOrder}";
                var tileType = tile.TryGetProperty("type", out var tileTypeElement) ? tileTypeElement.GetString() ?? "page" : "page";
                var tileImageUrl = tile.TryGetProperty("imageUrl", out var tileImageElement) ? ExtractFilename(tileImageElement.GetString() ?? "") : null;

                var craftTile = new StoryCraftTile
                {
                    Id = Guid.NewGuid(),
                    StoryCraftId = craft.Id,
                    TileId = tileId,
                    Type = tileType,
                    SortOrder = sortOrder++,
                    ImageUrl = tileImageUrl
                };

                // Add tile translations
                if (tile.TryGetProperty("translations", out var tileTranslationsElement) && tileTranslationsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var translation in tileTranslationsElement.EnumerateArray())
                    {
                        var lang = translation.TryGetProperty("lang", out var langElement) ? langElement.GetString() : null;
                        var caption = translation.TryGetProperty("caption", out var captionElement) ? captionElement.GetString() : null;
                        var text = translation.TryGetProperty("text", out var textElement) ? textElement.GetString() : null;
                        var question = translation.TryGetProperty("question", out var questionElement) ? questionElement.GetString() : null;
                        var audioUrl = translation.TryGetProperty("audioUrl", out var audioElement) ? ExtractFilename(audioElement.GetString() ?? "") : null;
                        var videoUrl = translation.TryGetProperty("videoUrl", out var videoElement) ? ExtractFilename(videoElement.GetString() ?? "") : null;

                        if (!string.IsNullOrWhiteSpace(lang))
                        {
                            craftTile.Translations.Add(new StoryCraftTileTranslation
                            {
                                Id = Guid.NewGuid(),
                                StoryCraftTileId = craftTile.Id,
                                LanguageCode = lang,
                                Caption = caption,
                                Text = text,
                                Question = question,
                                AudioUrl = audioUrl,
                                VideoUrl = videoUrl
                            });
                        }
                    }
                }

                // Add answers if present
                if (tile.TryGetProperty("answers", out var answersElement) && answersElement.ValueKind == JsonValueKind.Array)
                {
                    var answerSortOrder = 0;
                    foreach (var answer in answersElement.EnumerateArray())
                    {
                        var answerId = answer.TryGetProperty("id", out var answerIdElement) ? answerIdElement.GetString() ?? $"answer-{answerSortOrder}" : $"answer-{answerSortOrder}";

                        var isCorrect = answer.TryGetProperty("isCorrect", out var isCorrectElement) && isCorrectElement.ValueKind == JsonValueKind.True
                            ? true
                            : false;

                        var craftAnswer = new StoryCraftAnswer
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftTileId = craftTile.Id,
                            AnswerId = answerId,
                            IsCorrect = isCorrect,
                            SortOrder = answerSortOrder++
                        };

                        // Add answer translations
                        if (answer.TryGetProperty("translations", out var answerTranslationsElement) && answerTranslationsElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var translation in answerTranslationsElement.EnumerateArray())
                            {
                                var lang = translation.TryGetProperty("lang", out var langElement) ? langElement.GetString() : null;
                                var answerText = translation.TryGetProperty("text", out var textElement) ? textElement.GetString() : null;

                                if (!string.IsNullOrWhiteSpace(lang))
                                {
                                    craftAnswer.Translations.Add(new StoryCraftAnswerTranslation
                                    {
                                        Id = Guid.NewGuid(),
                                        StoryCraftAnswerId = craftAnswer.Id,
                                        LanguageCode = lang,
                                        Text = answerText
                                    });
                                }
                            }
                        }

                        // Add tokens if present
                        if (answer.TryGetProperty("tokens", out var tokensElement) && tokensElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var token in tokensElement.EnumerateArray())
                            {
                                var tokenType = token.TryGetProperty("type", out var tokenTypeElement) ? tokenTypeElement.GetString() : null;
                                var tokenValue = token.TryGetProperty("value", out var tokenValueElement) ? tokenValueElement.GetString() : null;
                                var tokenQuantity = token.TryGetProperty("quantity", out var quantityElement) && quantityElement.ValueKind == JsonValueKind.Number
                                    ? quantityElement.GetInt32()
                                    : 0;

                                if (!string.IsNullOrWhiteSpace(tokenType) && !string.IsNullOrWhiteSpace(tokenValue))
                                {
                                    craftAnswer.Tokens.Add(new StoryCraftAnswerToken
                                    {
                                        Id = Guid.NewGuid(),
                                        StoryCraftAnswerId = craftAnswer.Id,
                                        Type = tokenType,
                                        Value = tokenValue,
                                        Quantity = tokenQuantity
                                    });
                                }
                            }
                        }

                        craftTile.Answers.Add(craftAnswer);
                    }
                }

                craft.Tiles.Add(craftTile);
            }
        }

        // Save using repository
        await _crafts.SaveAsync(craft, ct);

        // Extract and set topic IDs
        var topicIds = new List<string>();
        if (root.TryGetProperty("topicIds", out var topicIdsElement) && topicIdsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var topicIdElement in topicIdsElement.EnumerateArray())
            {
                var topicId = topicIdElement.GetString();
                if (!string.IsNullOrWhiteSpace(topicId))
                {
                    topicIds.Add(topicId);
                }
            }
        }
        await UpdateTopicsAsync(craft, topicIds, ct);

        // Extract and set age group IDs
        var ageGroupIds = new List<string>();
        if (root.TryGetProperty("ageGroupIds", out var ageGroupIdsElement) && ageGroupIdsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var ageGroupIdElement in ageGroupIdsElement.EnumerateArray())
            {
                var ageGroupId = ageGroupIdElement.GetString();
                if (!string.IsNullOrWhiteSpace(ageGroupId))
                {
                    ageGroupIds.Add(ageGroupId);
                }
            }
        }
        await UpdateAgeGroupsAsync(craft, ageGroupIds, ct);

        // Save again to persist topics and age groups
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Created StoryCraft from import: storyId={StoryId} topics={TopicsCount} ageGroups={AgeGroupsCount}",
            storyId, topicIds.Count, ageGroupIds.Count);
    }

    private async Task UpdateTopicsAsync(StoryCraft craft, List<string> topicIds, CancellationToken ct)
    {
        if (topicIds == null || topicIds.Count == 0)
        {
            return;
        }

        // Get topic entities by TopicId
        var topics = await _db.StoryTopics
            .Where(t => topicIds.Contains(t.TopicId))
            .ToListAsync(ct);

        // Add new topics
        foreach (var topic in topics)
        {
            _db.StoryCraftTopics.Add(new StoryCraftTopic
            {
                StoryCraftId = craft.Id,
                StoryTopicId = topic.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (topics.Count < topicIds.Count)
        {
            var foundTopicIds = topics.Select(t => t.TopicId).ToList();
            var missingTopicIds = topicIds.Except(foundTopicIds).ToList();
            _logger.LogWarning("Some topic IDs not found in database: {MissingTopicIds}", string.Join(", ", missingTopicIds));
        }
    }

    private async Task UpdateAgeGroupsAsync(StoryCraft craft, List<string> ageGroupIds, CancellationToken ct)
    {
        if (ageGroupIds == null || ageGroupIds.Count == 0)
        {
            return;
        }

        // Get age group entities by AgeGroupId
        var ageGroups = await _db.StoryAgeGroups
            .Where(ag => ageGroupIds.Contains(ag.AgeGroupId))
            .ToListAsync(ct);

        // Add new age groups
        foreach (var ageGroup in ageGroups)
        {
            _db.StoryCraftAgeGroups.Add(new StoryCraftAgeGroup
            {
                StoryCraftId = craft.Id,
                StoryAgeGroupId = ageGroup.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (ageGroups.Count < ageGroupIds.Count)
        {
            var foundAgeGroupIds = ageGroups.Select(ag => ag.AgeGroupId).ToList();
            var missingAgeGroupIds = ageGroupIds.Except(foundAgeGroupIds).ToList();
            _logger.LogWarning("Some age group IDs not found in database: {MissingAgeGroupIds}", string.Join(", ", missingAgeGroupIds));
        }
    }

    [Route("/api/stories/import-jobs/{jobId:guid}")]
    [Authorize]
    public static async Task<Results<Ok<ImportJobStatusResponse>, NotFound, BadRequest<ImportFullStoryResponse>>> HandleGet(
        [FromRoute] Guid jobId,
        [FromServices] ImportFullStoryEndpoint ep,
        CancellationToken ct)
    {
        var errors = new List<string>();
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            errors.Add("Authentication required. Please log in to check import job status.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Creator) && !ep._auth0.HasRole(user, Data.Enums.UserRole.Admin))
        {
            errors.Add("You do not have permission to check import job status. Only users with Creator or Admin role can access this information.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var job = await ep._db.StoryImportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);

        if (job == null)
        {
            return TypedResults.NotFound();
        }

        // Check if user owns the job or is admin
        if (job.OwnerUserId != user.Id && !ep._auth0.HasRole(user, Data.Enums.UserRole.Admin))
        {
            errors.Clear();
            errors.Add($"You do not have permission to access this import job. This job belongs to another user.");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var response = new ImportJobStatusResponse
        {
            JobId = job.Id,
            StoryId = job.StoryId,
            OriginalStoryId = job.OriginalStoryId,
            Status = job.Status,
            QueuedAtUtc = job.QueuedAtUtc,
            StartedAtUtc = job.StartedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc,
            ImportedAssets = job.ImportedAssets,
            TotalAssets = job.TotalAssets,
            ImportedLanguagesCount = job.ImportedLanguagesCount,
            ErrorMessage = job.ErrorMessage,
            WarningSummary = job.WarningSummary,
            DequeueCount = job.DequeueCount
        };

        return TypedResults.Ok(response);
    }
}

