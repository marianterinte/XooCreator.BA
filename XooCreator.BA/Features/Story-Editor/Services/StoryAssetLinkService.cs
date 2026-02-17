using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryAssetLinkService
{
    Task SyncCoverAsync(StoryCraft craft, string ownerEmail, CancellationToken ct);
    Task SyncTileAssetsAsync(StoryCraft craft, StoryCraftTile tile, string ownerEmail, CancellationToken ct);
    Task RemoveTileAssetsAsync(string storyId, string tileId, CancellationToken ct);
    Task RemoveCoverAsync(string storyId, CancellationToken ct);
}

public class StoryAssetLinkService : IStoryAssetLinkService
{
    private const string CoverEntityId = "__cover__";

    private readonly XooDbContext _db;
    private readonly IStoryPublishAssetService _assetService;
    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryAssetLinkService> _logger;

    public StoryAssetLinkService(
        XooDbContext db,
        IStoryPublishAssetService assetService,
        IBlobSasService sas,
        ILogger<StoryAssetLinkService> logger)
    {
        _db = db;
        _assetService = assetService;
        _sas = sas;
        _logger = logger;
    }

    public async Task SyncCoverAsync(StoryCraft craft, string ownerEmail, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            await RemoveCoverAsync(craft.StoryId, ct);
            return;
        }

        var coverType = StoryAssetPathMapper.GetCoverAssetType(craft.CoverImageUrl);
        var asset = new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, coverType, null);
        var assets = new List<StoryAssetPathMapper.AssetInfo> { asset };
        var copyResult = await _assetService.CopyAssetsToPublishedAsync(assets, ownerEmail, craft.StoryId, ct);
        if (copyResult.HasError)
        {
            _logger.LogWarning("Failed to copy cover asset for storyId={StoryId}", craft.StoryId);
            return;
        }

        await UpsertLinkAsync(
            craft.StoryId,
            craft.LastDraftVersion,
            CoverEntityId,
            asset,
            ownerEmail,
            ct);
    }

    public async Task SyncTileAssetsAsync(StoryCraft craft, StoryCraftTile tile, string ownerEmail, CancellationToken ct)
    {
        var assets = CollectTileAssets(tile);
        if (assets.Count == 0)
        {
            await RemoveTileAssetsAsync(craft.StoryId, tile.TileId, ct);
            return;
        }

        var copyResult = await _assetService.CopyAssetsToPublishedAsync(assets, ownerEmail, craft.StoryId, ct);
        if (copyResult.HasError)
        {
            _logger.LogWarning("Failed to copy assets for tileId={TileId} storyId={StoryId}", tile.TileId, craft.StoryId);
            return;
        }

        foreach (var asset in assets)
        {
            await UpsertLinkAsync(
                craft.StoryId,
                craft.LastDraftVersion,
                tile.TileId,
                asset,
                ownerEmail,
                ct);
        }
    }

    public async Task RemoveTileAssetsAsync(string storyId, string tileId, CancellationToken ct)
    {
        var links = await _db.StoryAssetLinks
            .Where(x => x.StoryId == storyId && x.EntityId == tileId)
            .ToListAsync(ct);

        if (links.Count == 0)
        {
            return;
        }

        foreach (var link in links)
        {
            await DeletePublishedAssetAsync(link.PublishedPath, ct);
        }

        _db.StoryAssetLinks.RemoveRange(links);
    }

    public async Task RemoveCoverAsync(string storyId, CancellationToken ct)
    {
        var links = await _db.StoryAssetLinks
            .Where(x => x.StoryId == storyId && x.EntityId == CoverEntityId)
            .ToListAsync(ct);

        if (links.Count == 0)
        {
            return;
        }

        foreach (var link in links)
        {
            await DeletePublishedAssetAsync(link.PublishedPath, ct);
        }

        _db.StoryAssetLinks.RemoveRange(links);
    }

    private async Task UpsertLinkAsync(
        string storyId,
        int draftVersion,
        string entityId,
        StoryAssetPathMapper.AssetInfo asset,
        string ownerEmail,
        CancellationToken ct)
    {
        var language = asset.Lang?.ToLowerInvariant();
        var draftPath = StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, storyId);
        var publishedPath = StoryAssetPathMapper.BuildPublishedPath(asset, ownerEmail, storyId);
        var hash = ComputeAssetHash(asset, draftVersion);

        // First check if there's already an entity in the EF Core context with the same DraftPath
        // This prevents duplicate key violations when multiple calls happen before SaveChangesAsync
        var existingInContext = _db.ChangeTracker.Entries<StoryAssetLink>()
            .FirstOrDefault(e => e.Entity.DraftPath == draftPath && 
                                (e.State == Microsoft.EntityFrameworkCore.EntityState.Added || 
                                 e.State == Microsoft.EntityFrameworkCore.EntityState.Modified ||
                                 e.State == Microsoft.EntityFrameworkCore.EntityState.Unchanged));

        if (existingInContext != null)
        {
            // Update the existing entity in context
            var existing = existingInContext.Entity;
            existing.StoryId = storyId;
            existing.DraftVersion = draftVersion;
            existing.LanguageCode = language;
            existing.AssetType = asset.Type.ToString();
            existing.EntityId = entityId;
            existing.PublishedPath = publishedPath;
            existing.ContentHash = hash;
            existing.LastSyncedAt = DateTime.UtcNow;
            existing.UpdatedAt = DateTime.UtcNow;
            
            if (existingInContext.State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
            {
                existingInContext.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            return;
        }

        // Check by DraftPath in database to avoid duplicate key violations
        var existing2 = await _db.StoryAssetLinks
            .FirstOrDefaultAsync(x => x.DraftPath == draftPath, ct);

        if (existing2 == null)
        {
            _db.StoryAssetLinks.Add(new StoryAssetLink
            {
                Id = Guid.NewGuid(),
                StoryId = storyId,
                DraftVersion = draftVersion,
                LanguageCode = language,
                AssetType = asset.Type.ToString(),
                EntityId = entityId,
                DraftPath = draftPath,
                PublishedPath = publishedPath,
                ContentHash = hash,
                LastSyncedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            // Update existing link - may have different StoryId/EntityId if asset was moved/reused
            existing2.StoryId = storyId;
            existing2.DraftVersion = draftVersion;
            existing2.LanguageCode = language;
            existing2.AssetType = asset.Type.ToString();
            existing2.EntityId = entityId;
            existing2.PublishedPath = publishedPath;
            existing2.ContentHash = hash;
            existing2.LastSyncedAt = DateTime.UtcNow;
            existing2.UpdatedAt = DateTime.UtcNow;
        }
    }

    private async Task DeletePublishedAssetAsync(string? publishedPath, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(publishedPath))
        {
            return;
        }

        try
        {
            var client = _sas.GetBlobClient(_sas.PublishedContainer, publishedPath);
            await client.DeleteIfExistsAsync(cancellationToken: ct);

            // Also delete derived s/m variants for images.
            // We intentionally do NOT generate variants for non-4:5 images, so these might not exist.
            if (publishedPath.StartsWith("images/", StringComparison.OrdinalIgnoreCase) &&
                !publishedPath.Contains("/s/", StringComparison.OrdinalIgnoreCase) &&
                !publishedPath.Contains("/m/", StringComparison.OrdinalIgnoreCase))
            {
                var (basePath, fileName) = SplitPath(publishedPath);
                if (!string.IsNullOrWhiteSpace(basePath) && !string.IsNullOrWhiteSpace(fileName))
                {
                    var small = $"{basePath.TrimEnd('/')}/s/{fileName}";
                    var medium = $"{basePath.TrimEnd('/')}/m/{fileName}";
                    await _sas.GetBlobClient(_sas.PublishedContainer, small).DeleteIfExistsAsync(cancellationToken: ct);
                    await _sas.GetBlobClient(_sas.PublishedContainer, medium).DeleteIfExistsAsync(cancellationToken: ct);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete published asset path={Path}", publishedPath);
        }
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

    private static string ComputeAssetHash(StoryAssetPathMapper.AssetInfo asset, int draftVersion)
    {
        var raw = $"{asset.Type}|{asset.Filename}|{asset.Lang}|v{draftVersion}";
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(raw)));
    }

    private List<StoryAssetPathMapper.AssetInfo> CollectTileAssets(StoryCraftTile tile)
    {
        var assets = new List<StoryAssetPathMapper.AssetInfo>();
        if (!string.IsNullOrWhiteSpace(tile.ImageUrl))
        {
            assets.Add(new StoryAssetPathMapper.AssetInfo(tile.ImageUrl, StoryAssetPathMapper.AssetType.Image, null));
        }

        _logger.LogDebug("Collecting assets for tileId={TileId}: translations count={TranslationsCount}", 
            tile.TileId, tile.Translations.Count);

        foreach (var translation in tile.Translations)
        {
            var lang = translation.LanguageCode.ToLowerInvariant();
            _logger.LogDebug("Processing translation: tileId={TileId} lang={Lang} audioUrl={AudioUrl} videoUrl={VideoUrl}",
                tile.TileId, lang, translation.AudioUrl ?? "(null)", translation.VideoUrl ?? "(null)");
                
            if (!string.IsNullOrWhiteSpace(translation.AudioUrl))
            {
                assets.Add(new StoryAssetPathMapper.AssetInfo(translation.AudioUrl, StoryAssetPathMapper.AssetType.Audio, lang));
            }
            if (!string.IsNullOrWhiteSpace(translation.VideoUrl))
            {
                assets.Add(new StoryAssetPathMapper.AssetInfo(translation.VideoUrl, StoryAssetPathMapper.AssetType.Video, lang));
            }
        }

        // Dialog tile: collect audio from node and edge translations
        if (string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase) && tile.DialogTile != null)
        {
            foreach (var node in tile.DialogTile.Nodes)
            {
                foreach (var nodeTr in node.Translations)
                {
                    if (!string.IsNullOrWhiteSpace(nodeTr.AudioUrl))
                    {
                        var nodeLang = nodeTr.LanguageCode.ToLowerInvariant();
                        assets.Add(new StoryAssetPathMapper.AssetInfo(nodeTr.AudioUrl, StoryAssetPathMapper.AssetType.Audio, nodeLang));
                    }
                }
                // Option audio not used: only node (replica) audio
            }
        }

        var result = assets
            .GroupBy(a => $"{a.Type}|{a.Filename}|{a.Lang}")
            .Select(g => g.First())
            .ToList();
            
        _logger.LogInformation("Collected {AssetCount} assets for tileId={TileId}: {Assets}",
            result.Count, tile.TileId, 
            string.Join(", ", result.Select(a => $"{a.Type}/{a.Lang ?? "common"}/{a.Filename}")));

        return result;
    }
}

