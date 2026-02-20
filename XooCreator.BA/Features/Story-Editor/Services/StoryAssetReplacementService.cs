using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Extensions;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services.Blob;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryAssetReplacementService
{
    /// <summary>
    /// Deletes old asset from Azure Storage if it exists and updates DB with new filename
    /// </summary>
    Task ReplaceAssetAsync(
        string userEmail,
        string storyId,
        string? tileId,
        string kind,
        string newFileName,
        string? languageCode,
        CancellationToken ct);
}

/// <summary>
/// Service for handling asset replacement during upload.
/// Deletes old assets from Azure Storage and updates the database immediately.
/// </summary>
public class StoryAssetReplacementService : IStoryAssetReplacementService
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly XooDbContext _context;
    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryAssetReplacementService> _logger;

    public StoryAssetReplacementService(
        IStoryCraftsRepository crafts,
        XooDbContext context,
        IBlobSasService sas,
        ILogger<StoryAssetReplacementService> logger)
    {
        _crafts = crafts;
        _context = context;
        _sas = sas;
        _logger = logger;
    }

    public async Task ReplaceAssetAsync(
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
            var newFilenameOnly = newFileName.GetFilenameOnly();
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
                        translation = new StoryCraftTileTranslation
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
                        translation = new StoryCraftTileTranslation
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

}

