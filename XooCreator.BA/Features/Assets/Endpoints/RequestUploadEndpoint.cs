using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Features.Assets.DTOs;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Data;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.Assets.Endpoints;

[Endpoint]
public class RequestUploadEndpoint
{
    private readonly IBlobSasService _sas;
    private readonly IAuth0UserService _auth0;
    private readonly IConfiguration _config;
    private readonly IStoryCraftsRepository _crafts;
    private readonly XooDbContext _context;
    private readonly ILogger<RequestUploadEndpoint> _logger;

    public RequestUploadEndpoint(
        IBlobSasService sas, 
        IAuth0UserService auth0, 
        IConfiguration config,
        IStoryCraftsRepository crafts,
        XooDbContext context,
        ILogger<RequestUploadEndpoint> logger)
    {
        _sas = sas;
        _auth0 = auth0;
        _config = config;
        _crafts = crafts;
        _context = context;
        _logger = logger;
    }

    [Route("/api/assets/request-upload")]
    [Authorize]
    public static async Task<Results<Ok<RequestUploadResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] RequestUploadEndpoint ep,
        [FromBody] RequestUploadDto dto,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.StoryId) || string.IsNullOrWhiteSpace(dto.Lang) ||
            string.IsNullOrWhiteSpace(dto.Kind) || string.IsNullOrWhiteSpace(dto.FileName))
        {
            return TypedResults.BadRequest("Missing required fields.");
        }

        var contentType = string.IsNullOrWhiteSpace(dto.ContentType) ? "application/octet-stream" : dto.ContentType;

        // Validări minime (tip/extension/size)
        var ext = Path.GetExtension(dto.FileName).ToLowerInvariant();
        var allowedImage = new[] { ".png", ".jpg", ".jpeg", ".webp" };
        var allowedAudio = new[] { ".mp3", ".m4a", ".wav", ".aac", ".ogg" };
        var allowedVideo = new[] { ".mp4", ".webm" };

        long maxImage = ep._config.GetValue<long?>("Uploads:MaxImageBytes") ?? 10 * 1024 * 1024;
        long maxAudio = ep._config.GetValue<long?>("Uploads:MaxAudioBytes") ?? 50 * 1024 * 1024;
        long maxVideo = ep._config.GetValue<long?>("Uploads:MaxVideoBytes") ?? 200 * 1024 * 1024;

        // Ensure ExpectedSize is non-negative
        var expectedSize = dto.ExpectedSize < 0 ? 0 : dto.ExpectedSize;

        if (string.Equals(dto.Kind, "cover", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(dto.Kind, "tile-image", StringComparison.OrdinalIgnoreCase))
        {
            if (!allowedImage.Contains(ext)) return TypedResults.BadRequest("Unsupported image extension.");
            if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) return TypedResults.BadRequest("Invalid image content-type.");
            if (expectedSize > maxImage) return TypedResults.BadRequest("Image too large.");
        }
        else if (string.Equals(dto.Kind, "tile-audio", StringComparison.OrdinalIgnoreCase))
        {
            if (!allowedAudio.Contains(ext)) return TypedResults.BadRequest("Unsupported audio extension.");
            if (!contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)) return TypedResults.BadRequest("Invalid audio content-type.");
            if (expectedSize > maxAudio) return TypedResults.BadRequest("Audio too large.");
        }
        else if (string.Equals(dto.Kind, "video", StringComparison.OrdinalIgnoreCase))
        {
            if (!allowedVideo.Contains(ext)) return TypedResults.BadRequest("Unsupported video extension.");
            if (!contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)) return TypedResults.BadRequest("Invalid video content-type.");
            if (expectedSize > maxVideo) return TypedResults.BadRequest("Video too large.");
        }

        var assetType = dto.Kind switch
        {
            "cover" => StoryAssetPathMapper.AssetType.Image,
            "tile-image" => StoryAssetPathMapper.AssetType.Image,
            "tile-audio" => StoryAssetPathMapper.AssetType.Audio,
            "video" => StoryAssetPathMapper.AssetType.Video,
            _ => StoryAssetPathMapper.AssetType.Image
        };

        var lang = assetType == StoryAssetPathMapper.AssetType.Audio || assetType == StoryAssetPathMapper.AssetType.Video
            ? dto.Lang
            : null;
        
        // Delete old asset if it exists and update DB with new filename
        await ep.DeleteOldAssetAndUpdateDbAsync(user.Email, dto.StoryId, dto.TileId, dto.Kind, dto.FileName, lang, ct);
        
        var asset = new StoryAssetPathMapper.AssetInfo(dto.FileName, assetType, lang);
        var blobPath = StoryAssetPathMapper.BuildDraftPath(asset, user.Email, dto.StoryId);

        var putUri = await ep._sas.GetPutSasAsync(ep._sas.DraftContainer, blobPath, contentType!, TimeSpan.FromMinutes(10), ct);
        var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, blobPath);

        return TypedResults.Ok(new RequestUploadResponse
        {
            PutUrl = putUri.ToString(),
            BlobUrl = blobClient.Uri.ToString(),
            RelPath = dto.FileName,
            Container = ep._sas.DraftContainer,
            BlobPath = blobPath
        });
    }

    /// <summary>
    /// Deletes old asset from Azure Storage if it exists and updates DB with new filename
    /// </summary>
    private async Task DeleteOldAssetAndUpdateDbAsync(
        string userEmail, 
        string storyId, 
        string? tileId, 
        string kind, 
        string newFileName, 
        string? languageCode, 
        CancellationToken ct)
    {
        try
        {
            var craft = await _crafts.GetAsync(storyId, ct);
            if (craft == null)
            {
                // Story doesn't exist yet, nothing to delete
                return;
            }

            string? oldFilename = null;
            StoryAssetPathMapper.AssetType assetType;
            string? lang = null;

            if (string.Equals(kind, "cover", StringComparison.OrdinalIgnoreCase))
            {
                // Cover image
                oldFilename = craft.CoverImageUrl;
                assetType = StoryAssetPathMapper.AssetType.Image;
                lang = null;
            }
            else if (string.Equals(kind, "tile-image", StringComparison.OrdinalIgnoreCase))
            {
                // Tile image
                if (string.IsNullOrWhiteSpace(tileId))
                {
                    _logger.LogWarning("TileId is required for tile-image upload: storyId={StoryId}", storyId);
                    return;
                }

                var tile = craft.Tiles.FirstOrDefault(t => t.TileId == tileId);
                if (tile == null)
                {
                    // Tile doesn't exist yet, nothing to delete
                    return;
                }

                oldFilename = tile.ImageUrl;
                assetType = StoryAssetPathMapper.AssetType.Image;
                lang = null;
            }
            else if (string.Equals(kind, "tile-audio", StringComparison.OrdinalIgnoreCase))
            {
                // Tile audio (language-specific)
                if (string.IsNullOrWhiteSpace(tileId))
                {
                    _logger.LogWarning("TileId is required for tile-audio upload: storyId={StoryId}", storyId);
                    return;
                }

                if (string.IsNullOrWhiteSpace(languageCode))
                {
                    _logger.LogWarning("Language code is required for tile-audio upload: storyId={StoryId} tileId={TileId}", storyId, tileId);
                    return;
                }

                var tile = craft.Tiles.FirstOrDefault(t => t.TileId == tileId);
                if (tile == null)
                {
                    // Tile doesn't exist yet, nothing to delete
                    return;
                }

                var translation = tile.Translations.FirstOrDefault(t => t.LanguageCode == languageCode.ToLowerInvariant());
                if (translation == null)
                {
                    // Translation doesn't exist yet, nothing to delete
                    return;
                }

                oldFilename = translation.AudioUrl;
                assetType = StoryAssetPathMapper.AssetType.Audio;
                lang = languageCode.ToLowerInvariant();
            }
            else if (string.Equals(kind, "video", StringComparison.OrdinalIgnoreCase))
            {
                // Tile video (language-specific)
                if (string.IsNullOrWhiteSpace(tileId))
                {
                    _logger.LogWarning("TileId is required for video upload: storyId={StoryId}", storyId);
                    return;
                }

                if (string.IsNullOrWhiteSpace(languageCode))
                {
                    _logger.LogWarning("Language code is required for video upload: storyId={StoryId} tileId={TileId}", storyId, tileId);
                    return;
                }

                var tile = craft.Tiles.FirstOrDefault(t => t.TileId == tileId);
                if (tile == null)
                {
                    // Tile doesn't exist yet, nothing to delete
                    return;
                }

                var translation = tile.Translations.FirstOrDefault(t => t.LanguageCode == languageCode.ToLowerInvariant());
                if (translation == null)
                {
                    // Translation doesn't exist yet, nothing to delete
                    return;
                }

                oldFilename = translation.VideoUrl;
                assetType = StoryAssetPathMapper.AssetType.Video;
                lang = languageCode.ToLowerInvariant();
            }
            else
            {
                // Unknown kind, skip
                return;
            }

            // Build path for old asset and delete it if it exists and is different
            var newFilenameOnly = ExtractFileName(newFileName);
            if (!string.IsNullOrWhiteSpace(oldFilename) && 
                !string.Equals(oldFilename, newFilenameOnly, StringComparison.OrdinalIgnoreCase))
            {
                var oldAsset = new StoryAssetPathMapper.AssetInfo(oldFilename, assetType, lang);
                var oldBlobPath = StoryAssetPathMapper.BuildDraftPath(oldAsset, userEmail, storyId);

                var oldBlobClient = _sas.GetBlobClient(_sas.DraftContainer, oldBlobPath);
                if (await oldBlobClient.ExistsAsync(ct))
                {
                    await oldBlobClient.DeleteIfExistsAsync(cancellationToken: ct);
                    _logger.LogInformation(
                        "Deleted old asset before upload: storyId={StoryId} kind={Kind} tileId={TileId} lang={Lang} oldAsset={OldAsset} newAsset={NewAsset}",
                        storyId, kind, tileId ?? "N/A", lang ?? "N/A", oldFilename, newFileName);
                }
            }

            // Update DB with new filename immediately so next upload sees the correct value
            if (string.Equals(kind, "cover", StringComparison.OrdinalIgnoreCase))
            {
                craft.CoverImageUrl = newFilenameOnly;
                craft.UpdatedAt = DateTime.UtcNow;
            }
            else if (string.Equals(kind, "tile-image", StringComparison.OrdinalIgnoreCase))
            {
                var tile = craft.Tiles.FirstOrDefault(t => t.TileId == tileId);
                if (tile != null)
                {
                    tile.ImageUrl = newFilenameOnly;
                    tile.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (string.Equals(kind, "tile-audio", StringComparison.OrdinalIgnoreCase))
            {
                var tile = craft.Tiles.FirstOrDefault(t => t.TileId == tileId);
                if (tile != null)
                {
                    var translation = tile.Translations.FirstOrDefault(t => t.LanguageCode == languageCode!.ToLowerInvariant());
                    if (translation != null)
                    {
                        translation.AudioUrl = newFilenameOnly;
                    }
                    else
                    {
                        // Create translation if it doesn't exist
                        translation = new Data.Entities.StoryCraftTileTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftTileId = tile.Id,
                            LanguageCode = languageCode!.ToLowerInvariant(),
                            AudioUrl = newFilenameOnly
                        };
                        _context.StoryCraftTileTranslations.Add(translation);
                    }
                    tile.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (string.Equals(kind, "video", StringComparison.OrdinalIgnoreCase))
            {
                var tile = craft.Tiles.FirstOrDefault(t => t.TileId == tileId);
                if (tile != null)
                {
                    var translation = tile.Translations.FirstOrDefault(t => t.LanguageCode == languageCode!.ToLowerInvariant());
                    if (translation != null)
                    {
                        translation.VideoUrl = newFilenameOnly;
                    }
                    else
                    {
                        // Create translation if it doesn't exist
                        translation = new Data.Entities.StoryCraftTileTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftTileId = tile.Id,
                            LanguageCode = languageCode!.ToLowerInvariant(),
                            VideoUrl = newFilenameOnly
                        };
                        _context.StoryCraftTileTranslations.Add(translation);
                    }
                    tile.UpdatedAt = DateTime.UtcNow;
                }
            }

            // Save changes to DB
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation(
                "Updated DB with new asset filename: storyId={StoryId} kind={Kind} tileId={TileId} lang={Lang} newAsset={NewAsset}",
                storyId, kind, tileId ?? "N/A", lang ?? "N/A", newFileName);
        }
        catch (Exception ex)
        {
            // Log but don't fail the upload if cleanup/update fails
            _logger.LogWarning(ex,
                "Failed to delete old asset or update DB before upload: storyId={StoryId} kind={Kind} tileId={TileId} lang={Lang}",
                storyId, kind, tileId ?? "N/A", languageCode ?? "N/A");
        }
    }

    /// <summary>
    /// Extracts filename from a path. If input is already just a filename (no '/'), returns it as-is.
    /// If input contains '/', extracts the filename using Path.GetFileName().
    /// </summary>
    private static string? ExtractFileName(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        
        // If already just filename (no path separator), return as-is
        if (!path.Contains('/')) return path;
        
        // Extract filename from path
        return Path.GetFileName(path);
    }
}

