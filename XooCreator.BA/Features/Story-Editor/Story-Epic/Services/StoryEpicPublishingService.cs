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

    public async Task<ValidationResult> ValidatePublishAsync(DbStoryEpic epic, Guid ownerUserId, bool isAdmin, CancellationToken ct = default)
    {
        // 1. Ownership check
        if (epic.OwnerUserId != ownerUserId)
        {
            return ValidationResult.Unauthorized();
        }

        // 2. Status check - epic trebuie să fie approved (după workflow de review)
        var currentStatus = StoryStatusExtensions.FromDb(epic.Status);
        if (currentStatus != StoryStatus.Approved && !isAdmin)
        {
            return ValidationResult.InvalidStatus("Epic must be approved before publishing. Please submit for review first.");
        }

        // 3. Minimum requirements - Check if epic has regions
        // Check both StoryEpicRegions (old architecture) and StoryEpicRegionReferences (new architecture)
        var hasRegionsOld = await _context.StoryEpicRegions
            .AnyAsync(r => r.EpicId == epic.Id, ct);
        var hasRegionsNew = await _context.StoryEpicRegionReferences
            .AnyAsync(r => r.EpicId == epic.Id, ct);

        if (!hasRegionsOld && !hasRegionsNew)
        {
            return ValidationResult.NoRegions();
        }

        if (!epic.StoryNodes.Any())
        {
            return ValidationResult.NoStories();
        }

        // 4. Check all referenced regions are published
        // Check both StoryEpicRegions (old architecture) and StoryEpicRegionReferences (new architecture)
        var unpublishedRegions = new List<StoryRegion>();
        
        // Check old architecture (StoryEpicRegions - regions stored directly in epic)
        // Note: In old architecture, regions are just IDs, so we can't check their status here
        // This check is mainly for the new architecture
        
        // Check new architecture (StoryEpicRegionReferences - references to independent StoryRegion entities)
        var regionReferences = await _context.StoryEpicRegionReferences
            .Where(r => r.EpicId == epic.Id)
            .Include(r => r.Region)
            .ToListAsync(ct);

        var unpublishedRegionsNew = regionReferences
            .Where(r => r.Region.Status != "published")
            .Select(r => r.Region)
            .ToList();
        
        unpublishedRegions.AddRange(unpublishedRegionsNew);

        if (unpublishedRegions.Any())
        {
            return ValidationResult.UnpublishedRegions(unpublishedRegions);
        }

        // 5. Check all referenced heroes are published
        var heroReferences = await _context.StoryEpicHeroReferences
            .Where(h => h.EpicId == epic.Id)
            .Include(h => h.Hero)
            .ToListAsync(ct);

        var unpublishedHeroes = heroReferences
            .Where(h => h.Hero.Status != "published")
            .Select(h => h.Hero)
            .ToList();

        if (unpublishedHeroes.Any())
        {
            return ValidationResult.UnpublishedHeroes(unpublishedHeroes);
        }

        // 6. Check all referenced stories are published
        var storyIds = epic.StoryNodes.Select(s => s.StoryId).ToList();
        var stories = await _context.StoryDefinitions
            .Where(s => storyIds.Contains(s.StoryId) && s.Status == StoryStatus.Published)
            .ToListAsync(ct);

        var unpublishedStories = storyIds.Except(stories.Select(s => s.StoryId)).ToList();
        if (unpublishedStories.Any())
        {
            return ValidationResult.UnpublishedStories(unpublishedStories);
        }

        return ValidationResult.Success();
    }

    public List<EpicAssetInfo> CollectEpicAssets(DbStoryEpic epic)
    {
        var assets = new List<EpicAssetInfo>();

        // Cover image
        if (!string.IsNullOrWhiteSpace(epic.CoverImageUrl))
        {
            var extension = Path.GetExtension(epic.CoverImageUrl);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".png"; // Default extension
            }

            assets.Add(new EpicAssetInfo
            {
                Type = EpicAssetType.Cover,
                DraftPath = NormalizeBlobPath(epic.CoverImageUrl),
                PublishedPath = $"images/epics/{epic.Id}/cover{extension}"
            });
        }

        // Reward images
        foreach (var storyNode in epic.StoryNodes)
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
                PublishedPath = $"images/epics/{epic.Id}/stories/{storyNode.StoryId}/reward{extension}"
            });
        }

        return assets;
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

    public void UpdateEpicAfterPublish(DbStoryEpic epic, List<EpicAssetInfo> assets)
    {
        // Versioning logic (similar cu StoryDefinition)
        var isNewPublish = epic.Status != "published" || epic.Version == 0;
        if (isNewPublish)
        {
            epic.Version = 1;
            epic.BaseVersion = 0;
        }
        else
        {
            // Re-publish: incrementează versiunea
            epic.BaseVersion = epic.Version;
            epic.Version = epic.Version <= 0 ? 1 : epic.Version + 1;
        }

        epic.Status = StoryStatus.Published.ToDb();
        epic.PublishedAtUtc = DateTime.UtcNow;
        epic.UpdatedAt = DateTime.UtcNow;

        // Actualizează cover image path
        var coverAsset = assets.FirstOrDefault(a => a.Type == EpicAssetType.Cover);
        if (coverAsset != null)
        {
            epic.CoverImageUrl = coverAsset.PublishedPath;
        }

        // Actualizează reward image paths
        foreach (var rewardAsset in assets.Where(a => a.Type == EpicAssetType.Reward))
        {
            var storyNode = epic.StoryNodes.FirstOrDefault(s => s.StoryId == rewardAsset.StoryId);
            if (storyNode != null)
            {
                storyNode.RewardImageUrl = rewardAsset.PublishedPath;
                storyNode.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    public async Task<DateTime?> PublishAsync(Guid ownerUserId, string epicId, bool isAdmin, CancellationToken ct = default)
    {
        // Load epic with all relations
        var epic = await _context.StoryEpics
            .Include(e => e.StoryNodes)
            .FirstOrDefaultAsync(e => e.Id == epicId, ct);

        if (epic == null)
        {
            throw new InvalidOperationException($"Epic '{epicId}' was not found.");
        }

        // Validate pre-publish
        var validationResult = await ValidatePublishAsync(epic, ownerUserId, isAdmin, ct);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(validationResult.ErrorMessage);
        }

        // Get owner email for asset paths
        var owner = await _context.AlchimaliaUsers
            .Where(u => u.Id == ownerUserId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrWhiteSpace(owner))
        {
            throw new InvalidOperationException($"Owner email not found for user {ownerUserId}");
        }

        // Collect assets
        var assets = CollectEpicAssets(epic);

        // Copy assets
        var copyResult = await CopyEpicAssetsAsync(assets, owner, epicId, ct);
        if (copyResult.HasError)
        {
            throw new InvalidOperationException(copyResult.ErrorMessage);
        }

        // Update epic status and metadata
        UpdateEpicAfterPublish(epic, assets);

        // Save changes
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Epic published successfully: epicId={EpicId} version={Version}", epicId, epic.Version);

        return epic.PublishedAtUtc;
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

        // Update definition with published asset paths
        var coverAsset = assets.FirstOrDefault(a => a.Type == EpicAssetType.Cover);
        if (coverAsset != null)
        {
            definition.CoverImageUrl = coverAsset.PublishedPath;
        }

        // Copy Regions
        foreach (var craftRegion in craft.Regions)
        {
            definition.Regions.Add(new StoryEpicDefinitionRegion
            {
                RegionId = craftRegion.RegionId,
                Label = craftRegion.Label,
                ImageUrl = craftRegion.ImageUrl,
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

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Published StoryEpicCraft to StoryEpicDefinition: epicId={EpicId} version={Version} draftVersion={DraftVersion} isNew={IsNew}",
            craft.Id, definition.Version, craft.LastDraftVersion, isNew);
    }
}
