using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

public partial class ImportFullStoryEndpoint
{
    /// <summary>Process import when assets were uploaded from client (staging blobs). No ZIP on server.</summary>
    internal async Task<ImportJobResult> ProcessImportJobFromStagingAsync(StoryImportJob job, CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var warnings = new List<string>();
        var errors = new List<string>();
        var uploadedBlobPaths = new List<string>();
        int totalAssets = 0;
        int uploadedAssets = 0;
        long referencedBytes = 0;
        long uploadedBytes = 0;

        if (string.IsNullOrEmpty(job.ManifestBlobPath) || string.IsNullOrEmpty(job.StagingPrefix))
        {
            errors.Add("Staging manifest or prefix missing.");
            return ImportJobResult.Failed(errors, warnings, 0, 0, 0, 0, new List<string>());
        }

        try
        {
            var manifestBlobClient = _sas.GetBlobClient(_sas.DraftContainer, job.ManifestBlobPath);
            if (!await manifestBlobClient.ExistsAsync(ct))
            {
                errors.Add("Manifest blob not found.");
                return ImportJobResult.Failed(errors, warnings, 0, 0, 0, 0, new List<string>());
            }

            string jsonContent;
            var download = await manifestBlobClient.DownloadStreamingAsync(cancellationToken: ct);
            using (var sr = new StreamReader(download.Value.Content, Encoding.UTF8))
            {
                jsonContent = await sr.ReadToEndAsync(ct);
            }

            JsonDocument jsonDoc;
            try
            {
                jsonDoc = JsonDocument.Parse(jsonContent);
            }
            catch (JsonException ex)
            {
                errors.Add($"Invalid manifest JSON: {ex.Message}");
                return ImportJobResult.Failed(errors, warnings, 0, 0, 0, 0, new List<string>());
            }

            using (jsonDoc)
            {
                var root = jsonDoc.RootElement;
                if (!root.TryGetProperty("tiles", out var tilesElement) || tilesElement.ValueKind != JsonValueKind.Array || tilesElement.GetArrayLength() == 0)
                {
                    errors.Add("Missing or empty 'tiles' array in manifest");
                    return ImportJobResult.Failed(errors, warnings, 0, 0, 0, 0, new List<string>());
                }

                var expectedAssets = CollectExpectedAssets(root, warnings, job.IncludeImages, job.IncludeAudio, job.IncludeVideo);
                totalAssets = expectedAssets.Count;

                var uploadSummary = await UploadAssetsFromStagingAsync(
                    job.StagingPrefix!,
                    expectedAssets,
                    job.RequestedByEmail,
                    job.StoryId,
                    warnings,
                    errors,
                    uploadedBlobPaths,
                    ct);

                uploadedAssets = uploadSummary.UploadedAssets;
                uploadedBytes = uploadSummary.UploadedBytes;

                if (errors.Any(e => e.Contains("critical", StringComparison.OrdinalIgnoreCase)))
                {
                    await RollbackAssetsAsync(uploadedBlobPaths, ct);
                    return ImportJobResult.Failed(errors, warnings, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, new List<string>());
                }

                var importedLanguages = ExtractLanguages(root);
                await CreateStoryCraftFromJsonAsync(root, job.OwnerUserId, job.StoryId, warnings, ct);

                _logger.LogInformation("Import full story from staging succeeded: jobId={JobId} storyId={StoryId} assets={AssetsCount}",
                    job.Id, job.StoryId, uploadedAssets);

                TrackImportTelemetry(
                    stopwatch.ElapsedMilliseconds,
                    "Success",
                    job.Locale,
                    job.StoryId,
                    job.OwnerUserId,
                    totalAssets,
                    uploadedAssets,
                    referencedBytes,
                    uploadedBytes,
                    warnings.Count,
                    errors.Count);

                await DeleteStagingBlobsAsync(job.StagingPrefix!, job.ManifestBlobPath!, ct);

                return ImportJobResult.Successful(totalAssets, uploadedAssets, referencedBytes, uploadedBytes, importedLanguages, warnings, errors);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import job from staging failed: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);
            errors.Add($"Import failed: {ex.Message}");
            TrackImportTelemetry(stopwatch.ElapsedMilliseconds, "Exception", job.Locale, job.StoryId, job.OwnerUserId, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, warnings.Count, errors.Count);
            return ImportJobResult.Failed(errors, warnings, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, new List<string>());
        }
    }

    private async Task DeleteStagingBlobsAsync(string stagingPrefix, string manifestBlobPath, CancellationToken ct)
    {
        try
        {
            var manifestClient = _sas.GetBlobClient(_sas.DraftContainer, manifestBlobPath);
            await manifestClient.DeleteIfExistsAsync(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete staging manifest: {Path}", manifestBlobPath);
        }

        var containerClient = _sas.GetContainerClient(_sas.DraftContainer);
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: stagingPrefix + "/", cancellationToken: ct))
        {
            if (string.IsNullOrEmpty(blobItem.Name)) continue;
            try
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete staging blob: {Path}", blobItem.Name);
            }
        }
    }

    internal async Task<ImportJobResult> ProcessImportJobAsync(StoryImportJob job, Stream zipStream, CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        var warnings = new List<string>();
        var errors = new List<string>();
        var uploadedBlobPaths = new List<string>();
        int totalAssets = 0;
        int uploadedAssets = 0;
        long referencedBytes = 0;
        long uploadedBytes = 0;

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
                return ImportJobResult.Failed(errors, warnings, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, new List<string>());
            }

            string jsonContent;
            await using (var manifestStream = manifestEntry.Open())
            using (var reader = new StreamReader(manifestStream, Encoding.UTF8))
            {
                jsonContent = await reader.ReadToEndAsync(ct);
            }

            // Resolve dialogRef: load dialogs/*.json from ZIP and inline into manifest so CreateStoryCraft sees full dialogNodes
            var manifestDir = manifestEntry.FullName.Replace('\\', '/');
            var lastSlash = manifestDir.LastIndexOf('/');
            var manifestDirPrefix = lastSlash >= 0 ? manifestDir[..(lastSlash + 1)] : string.Empty;
            jsonContent = InflateManifestDialogRefs(zip, manifestDirPrefix, jsonContent);

            JsonDocument jsonDoc;
            try
            {
                jsonDoc = JsonDocument.Parse(jsonContent);
            }
            catch (JsonException ex)
            {
                errors.Add($"Invalid JSON in manifest: {ex.Message}");
                return ImportJobResult.Failed(errors, warnings, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, new List<string>());
            }

            var root = jsonDoc.RootElement;
            if (!root.TryGetProperty("tiles", out var tilesElement) || tilesElement.ValueKind != JsonValueKind.Array || tilesElement.GetArrayLength() == 0)
            {
                errors.Add("Missing or empty 'tiles' array in manifest");
                return ImportJobResult.Failed(errors, warnings, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, new List<string>());
            }

            var expectedAssets = CollectExpectedAssets(root, warnings, job.IncludeImages, job.IncludeAudio, job.IncludeVideo);
            totalAssets = expectedAssets.Count;
            referencedBytes = CalculateReferencedAssetBytes(zip, expectedAssets);

            var uploadSummary = await UploadAssetsFromZipAsync(
                zip,
                expectedAssets,
                job.RequestedByEmail,
                job.StoryId,
                warnings,
                errors,
                uploadedBlobPaths,
                ct);

            uploadedAssets = uploadSummary.UploadedAssets;
            uploadedBytes = uploadSummary.UploadedBytes;

            if (errors.Any(e => e.Contains("critical", StringComparison.OrdinalIgnoreCase)))
            {
                await RollbackAssetsAsync(uploadedBlobPaths, ct);
                return ImportJobResult.Failed(errors, warnings, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, new List<string>());
            }

            var importedLanguages = ExtractLanguages(root);

            await CreateStoryCraftFromJsonAsync(root, job.OwnerUserId, job.StoryId, warnings, ct);

            _logger.LogInformation("Import full story job succeeded: jobId={JobId} storyId={StoryId} assets={AssetsCount}",
                job.Id, job.StoryId, uploadedAssets);

            TrackImportTelemetry(
                stopwatch.ElapsedMilliseconds,
                "Success",
                job.Locale,
                job.StoryId,
                job.OwnerUserId,
                totalAssets,
                uploadedAssets,
                referencedBytes,
                uploadedBytes,
                warnings.Count,
                errors.Count);

            return ImportJobResult.Successful(totalAssets, uploadedAssets, referencedBytes, uploadedBytes, importedLanguages, warnings, errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import job failed: jobId={JobId} storyId={StoryId}", job.Id, job.StoryId);
            errors.Add($"Import failed: {ex.Message}");

            TrackImportTelemetry(
                stopwatch.ElapsedMilliseconds,
                "Exception",
                job.Locale,
                job.StoryId,
                job.OwnerUserId,
                totalAssets,
                uploadedAssets,
                referencedBytes,
                uploadedBytes,
                warnings.Count,
                errors.Count);

            return ImportJobResult.Failed(errors, warnings, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, new List<string>());
        }
    }

    internal readonly record struct ImportJobResult(
        bool Success,
        int TotalAssets,
        int UploadedAssets,
        long ReferencedBytes,
        long UploadedBytes,
        IReadOnlyList<string> ImportedLanguages,
        IReadOnlyList<string> Warnings,
        IReadOnlyList<string> Errors)
    {
        public static ImportJobResult Successful(
            int totalAssets,
            int uploadedAssets,
            long referencedBytes,
            long uploadedBytes,
            IReadOnlyList<string> importedLanguages,
            IReadOnlyList<string> warnings,
            IReadOnlyList<string> errors)
            => new(true, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, importedLanguages, warnings, errors);

        public static ImportJobResult Failed(
            IReadOnlyList<string> errors,
            IReadOnlyList<string> warnings,
            int totalAssets,
            int uploadedAssets,
            long referencedBytes,
            long uploadedBytes,
            IReadOnlyList<string> importedLanguages)
            => new(false, totalAssets, uploadedAssets, referencedBytes, uploadedBytes, importedLanguages, warnings, errors);
    }
}

