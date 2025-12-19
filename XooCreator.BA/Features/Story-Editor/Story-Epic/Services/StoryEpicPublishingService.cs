using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage.Blobs.Specialized;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryEpicPublishingService : IStoryEpicPublishingService
{
    private readonly XooDbContext _context;
    private readonly IBlobSasService _blobSas;
    private readonly ILogger<StoryEpicPublishingService> _logger;

    public StoryEpicPublishingService(
        XooDbContext context,
        IBlobSasService blobSas,
        ILogger<StoryEpicPublishingService> logger)
    {
        _context = context;
        _blobSas = blobSas;
        _logger = logger;
    }

    /// <summary>
    /// Collects assets from StoryEpicCraft (for publishing from craft to definition)
    /// </summary>
    private List<EpicAssetInfo> CollectEpicAssetsFromCraft(StoryEpicCraft craft)
    {
        var assets = new List<EpicAssetInfo>();

        // Cover image
        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var extension = Path.GetExtension(craft.CoverImageUrl);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".png"; // Default extension
            }

            assets.Add(new EpicAssetInfo
            {
                Type = EpicAssetType.Cover,
                DraftPath = NormalizeBlobPath(craft.CoverImageUrl),
                PublishedPath = $"images/epics/{craft.Id}/cover{extension}"
            });
        }

        // Reward images
        foreach (var storyNode in craft.StoryNodes)
        {
            if (string.IsNullOrWhiteSpace(storyNode.RewardImageUrl)) continue;

            var extension = Path.GetExtension(storyNode.RewardImageUrl);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".png"; // Default extension
            }

            assets.Add(new EpicAssetInfo
            {
                Type = EpicAssetType.Reward,
                StoryId = storyNode.StoryId,
                DraftPath = NormalizeBlobPath(storyNode.RewardImageUrl),
                PublishedPath = $"images/epics/{craft.Id}/stories/{storyNode.StoryId}/reward{extension}"
            });
        }

        return assets;
    }

    public async Task<AssetCopyResult> CopyEpicAssetsAsync(List<EpicAssetInfo> assets, string ownerEmail, string epicId, CancellationToken ct)
    {
        foreach (var asset in assets)
        {
            // Verifică dacă asset-ul este deja publicat
            if (IsAlreadyPublished(asset.DraftPath))
            {
                _logger.LogDebug("Asset already published: {Path}", asset.DraftPath);
                continue;
            }

            // Verifică existența în draft container
            var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, asset.DraftPath);
            if (!await sourceClient.ExistsAsync(ct))
            {
                _logger.LogWarning("Draft asset not found: {Path}", asset.DraftPath);
                return AssetCopyResult.AssetNotFound(asset.DraftPath, epicId);
            }

            // Copiază în published container
            var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, asset.PublishedPath);

            _logger.LogInformation("Copying epic asset: {Source} → {Destination}", asset.DraftPath, asset.PublishedPath);

            try
            {
                var sourceSas = sourceClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
                var copyOperation = await destinationClient.StartCopyFromUriAsync(sourceSas, cancellationToken: ct);
                await copyOperation.WaitForCompletionAsync(cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to copy asset: {Path}", asset.DraftPath);
                return AssetCopyResult.CopyFailed(asset.DraftPath, ex.Message);
            }
        }

        return AssetCopyResult.Success();
    }

    private async Task<string> PublishRegionImageAsync(string epicId, string regionId, string draftPath, CancellationToken ct)
    {
        var sourceClient = _blobSas.GetBlobClient(_blobSas.DraftContainer, draftPath);
        if (!await sourceClient.ExistsAsync(ct))
        {
            throw new InvalidOperationException($"Draft image '{draftPath}' does not exist.");
        }

        var fileName = Path.GetFileName(draftPath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"{regionId}-background.png";
        }

        var destinationPath = $"images/epics/{epicId}/regions/{regionId}/{fileName}";
        var destinationClient = _blobSas.GetBlobClient(_blobSas.PublishedContainer, destinationPath);

        _logger.LogInformation("Copying epic region image from {Source} to {Destination}", draftPath, destinationPath);

        var sasUri = sourceClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(10));
        var operation = await destinationClient.StartCopyFromUriAsync(sasUri, cancellationToken: ct);
        await operation.WaitForCompletionAsync(cancellationToken: ct);

        return destinationPath;
    }

    private static string NormalizeBlobPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;

        var trimmed = path.Trim();
        if (trimmed.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(trimmed);
            trimmed = uri.AbsolutePath.TrimStart('/');
        }

        return trimmed.TrimStart('/');
    }

    private static bool IsAlreadyPublished(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return path.StartsWith("images/", StringComparison.OrdinalIgnoreCase);
    }

    public async Task PublishFromCraftAsync(StoryEpicCraft craft, string requestedByEmail, string langTag, bool forceFull, CancellationToken ct = default)
    {
        // Load craft with all related data
        craft = await _context.StoryEpicCrafts
            .Include(c => c.Regions)
            .Include(c => c.StoryNodes)
            .Include(c => c.UnlockRules)
            .Include(c => c.Translations)
            .Include(c => c.HeroReferences)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == craft.Id, ct) ?? craft;

        // Validate craft status
        if (craft.Status != "approved")
        {
            throw new InvalidOperationException($"Epic craft must be approved before publishing (current status: {craft.Status})");
        }

        // Load or create StoryEpicDefinition
        var definition = await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.UnlockRules)
            .Include(d => d.Translations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == craft.Id, ct);

        var isNew = definition == null;
        if (isNew)
        {
            definition = new StoryEpicDefinition
            {
                Id = craft.Id,
                Name = craft.Name,
                Description = craft.Description,
                OwnerUserId = craft.OwnerUserId,
                Status = "published",
                CoverImageUrl = craft.CoverImageUrl,
                IsDefault = craft.IsDefault,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PublishedAtUtc = DateTime.UtcNow,
                Version = 1,
                BaseVersion = 0,
                LastPublishedVersion = craft.LastDraftVersion
            };
            _context.StoryEpicDefinitions.Add(definition);
        }
        else
        {
            // Update existing definition
            // Save current version as BaseVersion BEFORE incrementing
            definition.BaseVersion = definition.Version;
            definition.Version = definition.Version <= 0 ? 1 : definition.Version + 1;
            
            definition.Name = craft.Name;
            definition.Description = craft.Description;
            definition.Status = "published";
            definition.CoverImageUrl = craft.CoverImageUrl;
            definition.IsDefault = craft.IsDefault;
            definition.UpdatedAt = DateTime.UtcNow;
            definition.PublishedAtUtc = DateTime.UtcNow;
            definition.LastPublishedVersion = craft.LastDraftVersion;
        }

        // Collect and copy assets from craft
        var assets = CollectEpicAssetsFromCraft(craft);
        var copyResult = await CopyEpicAssetsAsync(assets, requestedByEmail, craft.Id, ct);
        if (copyResult.HasError)
        {
            throw new InvalidOperationException(copyResult.ErrorMessage);
        }

        // Remove existing content before adding new content
        // Always remove to avoid duplication when re-publishing
        // This ensures clean state before copying new content from craft
        // Remove existing regions
        _context.StoryEpicDefinitionRegions.RemoveRange(definition.Regions);
        definition.Regions.Clear();

        // Remove existing story nodes
        _context.StoryEpicDefinitionStoryNodes.RemoveRange(definition.StoryNodes);
        definition.StoryNodes.Clear();

        // Remove existing unlock rules
        _context.StoryEpicDefinitionUnlockRules.RemoveRange(definition.UnlockRules);
        definition.UnlockRules.Clear();

        // Remove existing translations
        _context.StoryEpicDefinitionTranslations.RemoveRange(definition.Translations);
        definition.Translations.Clear();

        // Remove existing hero references
        var existingHeroReferences = await _context.StoryEpicHeroReferences
            .Where(h => h.EpicId == definition.Id)
            .ToListAsync(ct);
        if (existingHeroReferences.Count > 0)
        {
            _context.StoryEpicHeroReferences.RemoveRange(existingHeroReferences);
        }

        // Update definition with published asset paths
        var coverAsset = assets.FirstOrDefault(a => a.Type == EpicAssetType.Cover);
        if (coverAsset != null)
        {
            definition.CoverImageUrl = coverAsset.PublishedPath;
        }

        // Copy Regions (and publish region images if they're draft assets)
        foreach (var craftRegion in craft.Regions)
        {
            var publishedImageUrl = craftRegion.ImageUrl;
            
            // If region has an image URL and it's a draft asset (not already published), publish it
            if (!string.IsNullOrWhiteSpace(craftRegion.ImageUrl) && !IsAlreadyPublished(craftRegion.ImageUrl))
            {
                try
                {
                    publishedImageUrl = await PublishRegionImageAsync(craft.Id, craftRegion.RegionId, craftRegion.ImageUrl, ct);
                    _logger.LogInformation("Published region image: regionId={RegionId} from={DraftPath} to={PublishedPath}", 
                        craftRegion.RegionId, craftRegion.ImageUrl, publishedImageUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to publish region image for regionId={RegionId}, using draft path", craftRegion.RegionId);
                    // Continue with draft path if publish fails (non-blocking)
                }
            }
            
            definition.Regions.Add(new StoryEpicDefinitionRegion
            {
                RegionId = craftRegion.RegionId,
                Label = craftRegion.Label,
                ImageUrl = publishedImageUrl, // Use published path if copied, otherwise original draft path
                SortOrder = craftRegion.SortOrder,
                IsLocked = craftRegion.IsLocked,
                IsStartupRegion = craftRegion.IsStartupRegion,
                X = craftRegion.X,
                Y = craftRegion.Y
            });
        }

        // Copy StoryNodes
        foreach (var craftNode in craft.StoryNodes)
        {
            // Find the corresponding reward asset to get published path
            var rewardAsset = assets.FirstOrDefault(a => a.Type == EpicAssetType.Reward && a.StoryId == craftNode.StoryId);
            var rewardImageUrl = rewardAsset?.PublishedPath ?? craftNode.RewardImageUrl;

            definition.StoryNodes.Add(new StoryEpicDefinitionStoryNode
            {
                StoryId = craftNode.StoryId,
                RegionId = craftNode.RegionId,
                RewardImageUrl = rewardImageUrl, // Use published path if asset was copied
                SortOrder = craftNode.SortOrder,
                X = craftNode.X,
                Y = craftNode.Y
            });
        }

        // Copy UnlockRules
        foreach (var craftRule in craft.UnlockRules)
        {
            definition.UnlockRules.Add(new StoryEpicDefinitionUnlockRule
            {
                Type = craftRule.Type,
                FromId = craftRule.FromId,
                ToRegionId = craftRule.ToRegionId,
                RequiredStoriesCsv = craftRule.RequiredStoriesCsv,
                MinCount = craftRule.MinCount,
                StoryId = craftRule.StoryId,
                SortOrder = craftRule.SortOrder
            });
        }

        // Copy Translations
        foreach (var craftTranslation in craft.Translations)
        {
            definition.Translations.Add(new StoryEpicDefinitionTranslation
            {
                StoryEpicDefinitionId = definition.Id,
                LanguageCode = craftTranslation.LanguageCode,
                Name = craftTranslation.Name,
                Description = craftTranslation.Description
            });
        }

        // Copy Hero References from StoryEpicCraftHeroReference (craft) to StoryEpicHeroReference (definition)
        foreach (var craftHeroRef in craft.HeroReferences)
        {
            _context.StoryEpicHeroReferences.Add(new StoryEpicHeroReference
            {
                EpicId = definition.Id,
                HeroId = craftHeroRef.HeroId,
                StoryId = craftHeroRef.StoryId
            });
        }

        // Copy unlocked heroes from StoryCrafts in this epic
        // For each story node, get the corresponding StoryCraft and copy its unlocked heroes to StoryDefinition
        foreach (var craftNode in craft.StoryNodes)
        {
            var storyCraft = await _context.StoryCrafts
                .Include(sc => sc.UnlockedHeroes)
                .FirstOrDefaultAsync(sc => sc.StoryId == craftNode.StoryId, ct);

            if (storyCraft != null && storyCraft.UnlockedHeroes != null && storyCraft.UnlockedHeroes.Count > 0)
            {
                // Get or create StoryDefinition for this story
                var storyDefinition = await _context.StoryDefinitions
                    .FirstOrDefaultAsync(sd => sd.StoryId == craftNode.StoryId, ct);

                if (storyDefinition != null)
                {
                    // Remove existing unlocked heroes for this story definition
                    var existingUnlockedHeroes = await _context.StoryDefinitionUnlockedHeroes
                        .Where(h => h.StoryDefinitionId == storyDefinition.Id)
                        .ToListAsync(ct);
                    if (existingUnlockedHeroes.Count > 0)
                    {
                        _context.StoryDefinitionUnlockedHeroes.RemoveRange(existingUnlockedHeroes);
                    }

                    // Copy unlocked heroes from StoryCraft to StoryDefinition
                    foreach (var craftUnlockedHero in storyCraft.UnlockedHeroes)
                    {
                        _context.StoryDefinitionUnlockedHeroes.Add(new StoryDefinitionUnlockedHero
                        {
                            StoryDefinitionId = storyDefinition.Id,
                            HeroId = craftUnlockedHero.HeroId,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    _logger.LogInformation(
                        "Copied {Count} unlocked heroes from StoryCraft to StoryDefinition: storyId={StoryId}",
                        storyCraft.UnlockedHeroes.Count, craftNode.StoryId);
                }
            }
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Published StoryEpicCraft to StoryEpicDefinition: epicId={EpicId} version={Version} draftVersion={DraftVersion} isNew={IsNew}",
            craft.Id, definition.Version, craft.LastDraftVersion, isNew);
    }
}
