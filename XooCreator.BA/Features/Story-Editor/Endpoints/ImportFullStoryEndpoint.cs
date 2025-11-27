using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using static XooCreator.BA.Features.StoryEditor.Mappers.StoryAssetPathMapper;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ImportFullStoryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryEditorService _editorService;
    private readonly ILogger<ImportFullStoryEndpoint> _logger;
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
        ILogger<ImportFullStoryEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _sas = sas;
        _crafts = crafts;
        _editorService = editorService;
        _logger = logger;
    }

    public record ImportFullStoryResponse
    {
        public string StoryId { get; init; } = string.Empty;
        public bool Success { get; init; }
        public List<string> Warnings { get; init; } = new();
        public List<string> Errors { get; init; } = new();
        public int ImportedAssets { get; init; }
        public List<string> ImportedLanguages { get; init; } = new();
    }

    [Route("/api/{locale}/stories/import-full")]
    [Authorize]
    public static async Task<Results<Ok<ImportFullStoryResponse>, BadRequest<ImportFullStoryResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] ImportFullStoryEndpoint ep,
        HttpRequest request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Allow Creator or Admin to import
        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);

        if (!isAdmin && !isCreator)
        {
            return TypedResults.Forbid();
        }

        var warnings = new List<string>();
        var errors = new List<string>();

        // Read file from form
        if (!request.HasFormContentType)
        {
            errors.Add("Request must be multipart/form-data");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
        }

        var form = await request.ReadFormAsync(ct);
        var file = form.Files.GetFile("file");

        // Validate file
        if (file == null || file.Length == 0)
        {
            errors.Add("No file provided");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
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

        // Track uploaded assets for rollback
        var uploadedBlobPaths = new List<string>();

        try
        {
            // Extract and validate ZIP
            using var zipStream = file.OpenReadStream();
            using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

            // Find manifest file
            var manifestEntry = zip.Entries.FirstOrDefault(e => 
                e.FullName.Contains("manifest/", StringComparison.OrdinalIgnoreCase) && 
                e.FullName.EndsWith("story.json", StringComparison.OrdinalIgnoreCase));

            if (manifestEntry == null)
            {
                errors.Add("Manifest file (manifest/{storyId}/story.json) not found in ZIP");
                return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
            }

            // Extract storyId from manifest path
            var manifestPath = manifestEntry.FullName;
            var storyIdMatch = Regex.Match(manifestPath, @"manifest/([^/]+)/story\.json");
            if (!storyIdMatch.Success)
            {
                errors.Add("Could not extract storyId from manifest path");
                return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
            }

            var originalStoryId = storyIdMatch.Groups[1].Value;

            // Read and parse JSON
            string jsonContent;
            await using (var manifestStream = manifestEntry.Open())
            using (var reader = new StreamReader(manifestStream, Encoding.UTF8))
            {
                jsonContent = await reader.ReadToEndAsync(ct);
            }

            // Parse JSON
            JsonDocument? jsonDoc;
            try
            {
                jsonDoc = JsonDocument.Parse(jsonContent);
            }
            catch (JsonException ex)
            {
                errors.Add($"Invalid JSON in manifest: {ex.Message}");
                return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
            }

            var root = jsonDoc.RootElement;

            // Validate required fields
            if (!root.TryGetProperty("id", out var idElement) || string.IsNullOrWhiteSpace(idElement.GetString()))
            {
                errors.Add("Missing or empty 'id' field in manifest");
                return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
            }

            if (!root.TryGetProperty("tiles", out var tilesElement) || tilesElement.ValueKind != JsonValueKind.Array || tilesElement.GetArrayLength() == 0)
            {
                errors.Add("Missing or empty 'tiles' array in manifest");
                return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors });
            }

            // Handle storyId conflict
            var finalStoryId = await ep.ResolveStoryIdConflictAsync(originalStoryId, user, isAdmin, ct);
            if (finalStoryId != originalStoryId)
            {
                warnings.Add($"StoryId '{originalStoryId}' already exists. Using '{finalStoryId}' instead.");
            }

            // Collect all assets from JSON
            var expectedAssets = ep.CollectExpectedAssets(root, warnings);

            // Upload assets from ZIP
            var uploadedAssets = await ep.UploadAssetsFromZipAsync(zip, expectedAssets, user.Email, finalStoryId, warnings, errors, uploadedBlobPaths, ct);

            // If critical errors occurred during upload, rollback
            if (errors.Any(e => e.Contains("critical", StringComparison.OrdinalIgnoreCase)))
            {
                await ep.RollbackAssetsAsync(uploadedBlobPaths, ct);
                return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors, Warnings = warnings });
            }

            // Extract languages
            var importedLanguages = ep.ExtractLanguages(root);

            // Create StoryCraft from JSON
            await ep.CreateStoryCraftFromJsonAsync(root, user.Id, finalStoryId, warnings, ct);

            ep._logger.LogInformation("Import full story successful: storyId={StoryId} userId={UserId} assets={AssetsCount}", 
                finalStoryId, user.Id, uploadedAssets);

            return TypedResults.Ok(new ImportFullStoryResponse
            {
                StoryId = finalStoryId,
                Success = true,
                Warnings = warnings,
                Errors = errors,
                ImportedAssets = uploadedAssets,
                ImportedLanguages = importedLanguages
            });
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Import full story failed: userId={UserId}", user.Id);
            
            // Rollback on any exception
            await ep.RollbackAssetsAsync(uploadedBlobPaths, ct);

            errors.Add($"Import failed: {ex.Message}");
            return TypedResults.BadRequest(new ImportFullStoryResponse { Success = false, Errors = errors, Warnings = warnings });
        }
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

    private List<(string ZipPath, AssetInfo Asset)> CollectExpectedAssets(JsonElement root, List<string> warnings)
    {
        var assets = new List<(string, AssetInfo)>();

        // Cover image
        if (root.TryGetProperty("coverImageUrl", out var coverElement) && coverElement.ValueKind == JsonValueKind.String)
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
                if (tile.TryGetProperty("imageUrl", out var imageElement) && imageElement.ValueKind == JsonValueKind.String)
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
                        if (translation.TryGetProperty("audioUrl", out var audioElement) && audioElement.ValueKind == JsonValueKind.String)
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
                        if (translation.TryGetProperty("videoUrl", out var videoElement) && videoElement.ValueKind == JsonValueKind.String)
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

    private async Task<int> UploadAssetsFromZipAsync(
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
        var containerClient = _sas.GetContainerClient(_sas.DraftContainer);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        foreach (var (zipPath, asset) in expectedAssets)
        {
            try
            {
                var normalizedZipPath = NormalizeZipPath(zipPath);

                // Find asset in ZIP using the full normalized path (exact match only)
                var zipEntry = zip.Entries.FirstOrDefault(e => 
                    string.Equals(NormalizeZipPath(e.FullName), normalizedZipPath, StringComparison.OrdinalIgnoreCase));

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

                _logger.LogDebug("Uploaded asset: {BlobPath} ({Size} bytes)", blobPath, memoryStream.Length);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to upload asset: {Filename}", asset.Filename);
                warnings.Add($"Failed to upload asset '{asset.Filename}': {ex.Message}");
            }
        }

        return uploadedCount;
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
                        
                        var craftAnswer = new StoryCraftAnswer
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftTileId = craftTile.Id,
                            AnswerId = answerId,
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
        _logger.LogInformation("Created StoryCraft from import: storyId={StoryId}", storyId);
    }
}

