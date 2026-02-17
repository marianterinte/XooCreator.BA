using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage.Blobs.Specialized;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Infrastructure.Services.Images;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryEpicPublishingService : IStoryEpicPublishingService
{
    private readonly XooDbContext _context;
    private readonly IBlobSasService _blobSas;
    private readonly IImageCompressionService _imageCompression;
    private readonly IEpicPublishChangeLogService _changeLogService;
    private readonly IEpicAssetLinkService _assetLinkService;
    private readonly ILogger<StoryEpicPublishingService> _logger;

    public StoryEpicPublishingService(
        XooDbContext context,
        IBlobSasService blobSas,
        IImageCompressionService imageCompression,
        IEpicPublishChangeLogService changeLogService,
        IEpicAssetLinkService assetLinkService,
        ILogger<StoryEpicPublishingService> logger)
    {
        _context = context;
        _blobSas = blobSas;
        _imageCompression = imageCompression;
        _changeLogService = changeLogService;
        _assetLinkService = assetLinkService;
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

                // Generate s/m variants for images (non-blocking).
                if (asset.PublishedPath.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var (basePath, fileName) = SplitPath(asset.PublishedPath);
                        await _imageCompression.EnsureStorySizeVariantsAsync(
                            sourceBlobPath: asset.PublishedPath,
                            targetBasePath: basePath,
                            filename: fileName,
                            overwriteExisting: true,
                            ct: ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to generate epic image variants: path={Path}", asset.PublishedPath);
                    }
                }
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

        // Generate s/m variants for region images (non-blocking).
        try
        {
            var (basePath, fileName2) = SplitPath(destinationPath);
            await _imageCompression.EnsureStorySizeVariantsAsync(
                sourceBlobPath: destinationPath,
                targetBasePath: basePath,
                filename: fileName2,
                overwriteExisting: true,
                ct: ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate region image variants: path={Path}", destinationPath);
        }

        return destinationPath;
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

    /// <summary>
    /// Validates that all regions and stories referenced in unlock rules exist in the craft.
    /// Throws <see cref="InvalidOperationException"/> with a clear message if any reference is invalid.
    /// </summary>
    private static void ValidateUnlockRulesReferences(StoryEpicCraft craft)
    {
        var regionIds = new HashSet<string>(craft.Regions.Select(r => r.RegionId), StringComparer.OrdinalIgnoreCase);
        var storyIds = new HashSet<string>(craft.StoryNodes.Select(n => n.StoryId), StringComparer.OrdinalIgnoreCase);
        var invalidRefs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var rule in craft.UnlockRules)
        {
            // FromId can be a region or story (source of the unlock)
            if (!string.IsNullOrWhiteSpace(rule.FromId) && !regionIds.Contains(rule.FromId) && !storyIds.Contains(rule.FromId))
            {
                invalidRefs.Add(rule.FromId);
            }

            if (!string.IsNullOrWhiteSpace(rule.ToRegionId) && !regionIds.Contains(rule.ToRegionId))
            {
                invalidRefs.Add($"region:{rule.ToRegionId}");
            }

            if (!string.IsNullOrWhiteSpace(rule.ToStoryId) && !storyIds.Contains(rule.ToStoryId))
            {
                invalidRefs.Add($"story:{rule.ToStoryId}");
            }

            if (!string.IsNullOrWhiteSpace(rule.RequiredStoriesCsv))
            {
                foreach (var id in rule.RequiredStoriesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (!string.IsNullOrWhiteSpace(id) && !storyIds.Contains(id))
                    {
                        invalidRefs.Add($"story:{id}");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(rule.StoryId) && !storyIds.Contains(rule.StoryId))
            {
                invalidRefs.Add($"story:{rule.StoryId}");
            }
        }

        if (invalidRefs.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            "Unlock rules reference regions or stories that are not in this epic. Invalid: " +
            string.Join(", ", invalidRefs.OrderBy(x => x)) +
            ". Ensure every region and story used in unlock rules exists in the epic tree.");
    }

    public async Task PublishFromCraftAsync(StoryEpicCraft craft, string requestedByEmail, string langTag, bool forceFull, bool isAdmin = false, CancellationToken ct = default)
    {
        // Load craft with all related data
        craft = await _context.StoryEpicCrafts
            .Include(c => c.Regions)
            .Include(c => c.StoryNodes)
            .Include(c => c.UnlockRules)
            .Include(c => c.Translations)
            .Include(c => c.HeroReferences)
            .Include(c => c.Topics).ThenInclude(t => t.StoryTopic)
            .Include(c => c.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .Include(c => c.CoAuthors)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == craft.Id, ct) ?? craft;

        // Validate craft status - admin can publish from draft, changes_requested, or approved
        if (!isAdmin && craft.Status != "approved")
        {
            throw new InvalidOperationException($"Epic craft must be approved before publishing (current status: {craft.Status})");
        }
        if (isAdmin && !(craft.Status == "approved" || craft.Status == "draft" || craft.Status == "changes_requested"))
        {
            throw new InvalidOperationException($"Admin cannot publish epic in status '{craft.Status}'. Expected Draft, ChangesRequested, or Approved.");
        }

        // Validate unlock rules: all referenced regions and stories must exist in this epic
        ValidateUnlockRulesReferences(craft);

        // Load or create StoryEpicDefinition
        var definition = await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.UnlockRules)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .Include(d => d.CoAuthors)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == craft.Id, ct);

        var isNew = definition == null;
        var requiresFullPublish = forceFull || isNew;
        List<EpicPublishChangeLog>? pendingLogs = null;

        if (!requiresFullPublish && definition != null)
        {
            pendingLogs = await _context.EpicPublishChangeLogs
                .Where(x => x.EpicId == craft.Id && x.DraftVersion > definition.LastPublishedVersion)
                .OrderBy(x => x.DraftVersion)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync(ct);

            if (pendingLogs.Count == 0)
            {
                requiresFullPublish = true;
            }
        }

        if (!requiresFullPublish && definition != null && pendingLogs != null)
        {
            var deltaApplied = await TryApplyDeltaPublishAsync(definition, craft, pendingLogs, requestedByEmail, langTag, ct);
            if (deltaApplied)
            {
                await CleanupChangeLogsAsync(craft.Id, craft.LastDraftVersion, ct);
                await CleanupCraftAsync(craft, ct);
                _logger.LogInformation(
                    "Published StoryEpicCraft to StoryEpicDefinition (delta): epicId={EpicId} version={Version} draftVersion={DraftVersion}",
                    craft.Id, definition.Version, craft.LastDraftVersion);
                return;
            }

            requiresFullPublish = true;
        }

        await ApplyFullPublishAsync(definition, craft, requestedByEmail, langTag, ct);
        await CleanupChangeLogsAsync(craft.Id, craft.LastDraftVersion, ct);
        await CleanupCraftAsync(craft, ct);
    }

    private async Task ApplyFullPublishAsync(StoryEpicDefinition? existingDefinition, StoryEpicCraft craft, string requestedByEmail, string langTag, CancellationToken ct)
    {
        var isNew = existingDefinition == null;
        var definition = existingDefinition;

        // Sync assets first
        await _assetLinkService.SyncAllAssetsAsync(craft, requestedByEmail, ct);

        // Get published cover image URL from asset link
        string? publishedCoverImageUrl = null;
        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var normalizedPath = NormalizeBlobPath(craft.CoverImageUrl);
            var assetLink = await _context.EpicAssetLinks
                .Where(x => x.EpicId == craft.Id && x.DraftPath == normalizedPath && x.AssetType == "Cover")
                .OrderByDescending(x => x.DraftVersion)
                .FirstOrDefaultAsync(ct);
            
            if (assetLink != null && !string.IsNullOrWhiteSpace(assetLink.PublishedPath))
            {
                publishedCoverImageUrl = assetLink.PublishedPath;
            }
            else if (IsAlreadyPublished(normalizedPath))
            {
                publishedCoverImageUrl = normalizedPath;
            }
        }

        // Use first translation for Name and Description, prefer ro-ro as default (or fallback to craft.Name/Description)
        // This ensures marketplace shows the correct language when new translations are added
        var defaultTranslation = craft.Translations.FirstOrDefault(t => t.LanguageCode == "ro-ro")
            ?? craft.Translations.OrderBy(t => t.LanguageCode).FirstOrDefault();
        var defaultName = defaultTranslation?.Name ?? craft.Name;
        var defaultDescription = defaultTranslation?.Description ?? craft.Description;

        if (isNew)
        {
            definition = new StoryEpicDefinition
            {
                Id = craft.Id,
                Name = defaultName,
                Description = defaultDescription,
                OwnerUserId = craft.OwnerUserId,
                Status = "published",
                IsActive = true,
                CoverImageUrl = publishedCoverImageUrl,
                IsDefault = craft.IsDefault,
                AudioLanguages = craft.AudioLanguages ?? new List<string>(),
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
            definition.BaseVersion = definition.Version;
            definition.Version = definition.Version <= 0 ? 1 : definition.Version + 1;
            definition.Name = defaultName;
            definition.Description = defaultDescription;
            definition.Status = "published";
            definition.IsActive = true;
            definition.CoverImageUrl = publishedCoverImageUrl;
            definition.IsDefault = craft.IsDefault;
            definition.AudioLanguages = craft.AudioLanguages ?? new List<string>();
            definition.UpdatedAt = DateTime.UtcNow;
            definition.PublishedAtUtc = DateTime.UtcNow;
            definition.LastPublishedVersion = craft.LastDraftVersion;

            // Remove existing content
            _context.StoryEpicDefinitionRegions.RemoveRange(definition.Regions);
            definition.Regions.Clear();
            _context.StoryEpicDefinitionStoryNodes.RemoveRange(definition.StoryNodes);
            definition.StoryNodes.Clear();
            _context.StoryEpicDefinitionUnlockRules.RemoveRange(definition.UnlockRules);
            definition.UnlockRules.Clear();
            _context.StoryEpicDefinitionTranslations.RemoveRange(definition.Translations);
            definition.Translations.Clear();
            _context.Set<StoryEpicDefinitionTopic>().RemoveRange(definition.Topics);
            definition.Topics.Clear();
            _context.Set<StoryEpicDefinitionAgeGroup>().RemoveRange(definition.AgeGroups);
            definition.AgeGroups.Clear();
            _context.StoryEpicDefinitionCoAuthors.RemoveRange(definition.CoAuthors);
            definition.CoAuthors.Clear();

            var existingHeroReferences = await _context.StoryEpicHeroReferences
                .Where(h => h.EpicId == definition.Id)
                .ToListAsync(ct);
            if (existingHeroReferences.Count > 0)
            {
                _context.StoryEpicHeroReferences.RemoveRange(existingHeroReferences);
            }
        }

        // Copy Regions (and publish region images if they're draft assets)
        foreach (var craftRegion in craft.Regions)
        {
            var publishedImageUrl = craftRegion.ImageUrl;
            
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
                }
            }
            
            definition.Regions.Add(new StoryEpicDefinitionRegion
            {
                RegionId = craftRegion.RegionId,
                Label = craftRegion.Label,
                ImageUrl = publishedImageUrl,
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
            var rewardImageUrl = craftNode.RewardImageUrl;
            if (!string.IsNullOrWhiteSpace(craftNode.RewardImageUrl))
            {
                var normalizedPath = NormalizeBlobPath(craftNode.RewardImageUrl);
                var assetLink = await _context.EpicAssetLinks
                    .Where(x => x.EpicId == craft.Id && x.DraftPath == normalizedPath && x.EntityId == $"__reward_image__{craftNode.StoryId}")
                    .OrderByDescending(x => x.DraftVersion)
                    .FirstOrDefaultAsync(ct);
                
                if (assetLink != null && !string.IsNullOrWhiteSpace(assetLink.PublishedPath))
                {
                    rewardImageUrl = assetLink.PublishedPath;
                }
                else if (IsAlreadyPublished(normalizedPath))
                {
                    rewardImageUrl = normalizedPath;
                }
            }

            definition.StoryNodes.Add(new StoryEpicDefinitionStoryNode
            {
                StoryId = craftNode.StoryId,
                RegionId = craftNode.RegionId,
                RewardImageUrl = rewardImageUrl,
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
                ToStoryId = craftRule.ToStoryId,
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

        // Copy Hero References
        foreach (var craftHeroRef in craft.HeroReferences)
        {
            _context.StoryEpicHeroReferences.Add(new StoryEpicHeroReference
            {
                EpicId = definition.Id,
                HeroId = craftHeroRef.HeroId,
                StoryId = craftHeroRef.StoryId
            });
        }

        // Copy Topics
        foreach (var craftTopic in craft.Topics)
        {
            definition.Topics.Add(new StoryEpicDefinitionTopic
            {
                StoryEpicDefinitionId = definition.Id,
                StoryTopicId = craftTopic.StoryTopicId,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Copy Age Groups
        foreach (var craftAgeGroup in craft.AgeGroups)
        {
            definition.AgeGroups.Add(new StoryEpicDefinitionAgeGroup
            {
                StoryEpicDefinitionId = definition.Id,
                StoryAgeGroupId = craftAgeGroup.StoryAgeGroupId,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Copy Co-authors
        var sortOrder = 0;
        foreach (var craftCoAuthor in craft.CoAuthors.OrderBy(ca => ca.SortOrder))
        {
            definition.CoAuthors.Add(new StoryEpicDefinitionCoAuthor
            {
                Id = Guid.NewGuid(),
                StoryEpicDefinitionId = definition.Id,
                UserId = craftCoAuthor.UserId,
                DisplayName = craftCoAuthor.DisplayName,
                SortOrder = sortOrder++
            });
        }

        // Copy unlocked heroes from StoryCrafts in this epic
        foreach (var craftNode in craft.StoryNodes)
        {
            var storyCraft = await _context.StoryCrafts
                .Include(sc => sc.UnlockedHeroes)
                .FirstOrDefaultAsync(sc => sc.StoryId == craftNode.StoryId, ct);

            if (storyCraft != null && storyCraft.UnlockedHeroes != null && storyCraft.UnlockedHeroes.Count > 0)
            {
                var storyDefinition = await _context.StoryDefinitions
                    .FirstOrDefaultAsync(sd => sd.StoryId == craftNode.StoryId, ct);

                if (storyDefinition != null)
                {
                    var existingUnlockedHeroes = await _context.StoryDefinitionUnlockedHeroes
                        .Where(h => h.StoryDefinitionId == storyDefinition.Id)
                        .ToListAsync(ct);
                    if (existingUnlockedHeroes.Count > 0)
                    {
                        _context.StoryDefinitionUnlockedHeroes.RemoveRange(existingUnlockedHeroes);
                    }

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

    private async Task<bool> TryApplyDeltaPublishAsync(
        StoryEpicDefinition definition,
        StoryEpicCraft craft,
        IReadOnlyCollection<EpicPublishChangeLog> changeLogs,
        string ownerEmail,
        string langTag,
        CancellationToken ct)
    {
        if (changeLogs.Count == 0)
        {
            return false;
        }

        var headerChanged = changeLogs.Any(l =>
            string.Equals(l.EntityType, "Header", StringComparison.OrdinalIgnoreCase));

        var regionReferenceChanges = changeLogs
            .Where(l => string.Equals(l.EntityType, "RegionReference", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(l.EntityId))
            .GroupBy(l => l.EntityId!)
            .Select(g => g.OrderBy(l => l.DraftVersion).ThenBy(l => l.CreatedAt).Last())
            .ToList();

        var storyNodeChanges = changeLogs
            .Where(l => string.Equals(l.EntityType, "StoryNode", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(l.EntityId))
            .GroupBy(l => l.EntityId!)
            .Select(g => g.OrderBy(l => l.DraftVersion).ThenBy(l => l.CreatedAt).Last())
            .ToList();

        var unlockRuleChanges = changeLogs
            .Where(l => string.Equals(l.EntityType, "UnlockRule", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(l.EntityId))
            .GroupBy(l => l.EntityId!)
            .Select(g => g.OrderBy(l => l.DraftVersion).ThenBy(l => l.CreatedAt).Last())
            .ToList();

        var translationChanges = changeLogs
            .Where(l => string.Equals(l.EntityType, "Translation", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(l.EntityId))
            .GroupBy(l => l.EntityId!)
            .Select(g => g.OrderBy(l => l.DraftVersion).ThenBy(l => l.CreatedAt).Last())
            .ToList();

        var heroReferenceChanges = changeLogs
            .Where(l => string.Equals(l.EntityType, "HeroReference", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(l.EntityId))
            .GroupBy(l => l.EntityId!)
            .Select(g => g.OrderBy(l => l.DraftVersion).ThenBy(l => l.CreatedAt).Last())
            .ToList();

        if (!headerChanged && regionReferenceChanges.Count == 0 && storyNodeChanges.Count == 0 &&
            unlockRuleChanges.Count == 0 && translationChanges.Count == 0 && heroReferenceChanges.Count == 0)
        {
            return false;
        }

        if (headerChanged)
        {
            await ApplyDefinitionMetadataDeltaAsync(definition, craft, ownerEmail, ct);
        }

        foreach (var change in regionReferenceChanges)
        {
            var applied = await ApplyRegionReferenceChangeAsync(definition, craft, change, ownerEmail, ct);
            if (!applied) return false;
        }

        foreach (var change in storyNodeChanges)
        {
            var applied = await ApplyStoryNodeChangeAsync(definition, craft, change, ownerEmail, ct);
            if (!applied) return false;
        }

        foreach (var change in unlockRuleChanges)
        {
            var applied = await ApplyUnlockRuleChangeAsync(definition, craft, change, ct);
            if (!applied) return false;
        }

        foreach (var change in translationChanges)
        {
            var applied = await ApplyTranslationChangeAsync(definition, craft, change, ct);
            if (!applied) return false;
        }

        foreach (var change in heroReferenceChanges)
        {
            var applied = await ApplyHeroReferenceChangeAsync(definition, craft, change, ct);
            if (!applied) return false;
        }

        definition.LastPublishedVersion = craft.LastDraftVersion;
        definition.Version = definition.Version <= 0 ? 1 : definition.Version + 1;
        definition.Status = "published";
        definition.IsActive = true;
        definition.UpdatedAt = DateTime.UtcNow;
        definition.PublishedAtUtc = DateTime.UtcNow;

        return true;
    }

    private async Task ApplyDefinitionMetadataDeltaAsync(StoryEpicDefinition definition, StoryEpicCraft craft, string ownerEmail, CancellationToken ct)
    {
        // Use first translation for Name and Description, prefer ro-ro as default (or fallback to craft.Name/Description)
        // This ensures marketplace shows the correct language when new translations are added
        var defaultTranslation = craft.Translations.FirstOrDefault(t => t.LanguageCode == "ro-ro")
            ?? craft.Translations.OrderBy(t => t.LanguageCode).FirstOrDefault();
        definition.Name = defaultTranslation?.Name ?? craft.Name;
        definition.Description = defaultTranslation?.Description ?? craft.Description;
        definition.IsDefault = craft.IsDefault;
        definition.AudioLanguages = craft.AudioLanguages ?? new List<string>();
        definition.UpdatedAt = DateTime.UtcNow;

        await _assetLinkService.SyncCoverImageAsync(craft, ownerEmail, ct);

        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var normalizedPath = NormalizeBlobPath(craft.CoverImageUrl);
            // Find the asset link for the current draft version to ensure we get the correct published path
            // This is important when doing replacement (same filename, new version)
            var assetLink = await _context.EpicAssetLinks
                .Where(x => x.EpicId == craft.Id && x.DraftPath == normalizedPath && x.AssetType == "Cover")
                .OrderByDescending(x => x.DraftVersion) // Get the latest version
                .FirstOrDefaultAsync(ct);
            
            if (assetLink != null && !string.IsNullOrWhiteSpace(assetLink.PublishedPath))
            {
                definition.CoverImageUrl = assetLink.PublishedPath;
            }
            else if (IsAlreadyPublished(normalizedPath))
            {
                definition.CoverImageUrl = normalizedPath;
            }
        }
        else
        {
            definition.CoverImageUrl = null;
            await _assetLinkService.RemoveCoverImageAsync(craft.Id, ct);
        }

        SyncDefinitionTopics(definition, craft);
        SyncDefinitionAgeGroups(definition, craft);
        SyncDefinitionCoAuthors(definition, craft);
    }

    private void SyncDefinitionTopics(StoryEpicDefinition definition, StoryEpicCraft craft)
    {
        _context.Set<StoryEpicDefinitionTopic>().RemoveRange(definition.Topics);
        definition.Topics.Clear();

        foreach (var craftTopic in craft.Topics)
        {
            definition.Topics.Add(new StoryEpicDefinitionTopic
            {
                StoryEpicDefinitionId = definition.Id,
                StoryTopicId = craftTopic.StoryTopicId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private void SyncDefinitionAgeGroups(StoryEpicDefinition definition, StoryEpicCraft craft)
    {
        _context.Set<StoryEpicDefinitionAgeGroup>().RemoveRange(definition.AgeGroups);
        definition.AgeGroups.Clear();

        foreach (var craftAgeGroup in craft.AgeGroups)
        {
            definition.AgeGroups.Add(new StoryEpicDefinitionAgeGroup
            {
                StoryEpicDefinitionId = definition.Id,
                StoryAgeGroupId = craftAgeGroup.StoryAgeGroupId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private void SyncDefinitionCoAuthors(StoryEpicDefinition definition, StoryEpicCraft craft)
    {
        _context.StoryEpicDefinitionCoAuthors.RemoveRange(definition.CoAuthors);
        definition.CoAuthors.Clear();

        var sortOrder = 0;
        foreach (var craftCoAuthor in craft.CoAuthors.OrderBy(ca => ca.SortOrder))
        {
            definition.CoAuthors.Add(new StoryEpicDefinitionCoAuthor
            {
                Id = Guid.NewGuid(),
                StoryEpicDefinitionId = definition.Id,
                UserId = craftCoAuthor.UserId,
                DisplayName = craftCoAuthor.DisplayName,
                SortOrder = sortOrder++
            });
        }
    }

    private async Task<bool> ApplyRegionReferenceChangeAsync(StoryEpicDefinition definition, StoryEpicCraft craft, EpicPublishChangeLog change, string ownerEmail, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            return true;
        }

        var regionId = change.EntityId;

        if (string.Equals(change.ChangeType, "Removed", StringComparison.OrdinalIgnoreCase))
        {
            var existingRegion = definition.Regions.FirstOrDefault(r => r.RegionId == regionId);
            if (existingRegion != null)
            {
                _context.StoryEpicDefinitionRegions.Remove(existingRegion);
                definition.Regions.Remove(existingRegion);
            }
            return true;
        }

        var craftRegion = craft.Regions.FirstOrDefault(r => r.RegionId == regionId);
        if (craftRegion == null)
        {
            _logger.LogWarning("Delta publish failed: epicId={EpicId} regionId={RegionId} missing in craft.", definition.Id, regionId);
            return false;
        }

        var publishedImageUrl = craftRegion.ImageUrl;
        if (!string.IsNullOrWhiteSpace(craftRegion.ImageUrl) && !IsAlreadyPublished(craftRegion.ImageUrl))
        {
            try
            {
                publishedImageUrl = await PublishRegionImageAsync(craft.Id, craftRegion.RegionId, craftRegion.ImageUrl, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to publish region image for regionId={RegionId}", craftRegion.RegionId);
            }
        }

        var existingRegion2 = definition.Regions.FirstOrDefault(r => r.RegionId == regionId);
        if (existingRegion2 != null)
        {
            existingRegion2.Label = craftRegion.Label;
            existingRegion2.ImageUrl = publishedImageUrl;
            existingRegion2.SortOrder = craftRegion.SortOrder;
            existingRegion2.IsLocked = craftRegion.IsLocked;
            existingRegion2.IsStartupRegion = craftRegion.IsStartupRegion;
            existingRegion2.X = craftRegion.X;
            existingRegion2.Y = craftRegion.Y;
        }
        else
        {
            definition.Regions.Add(new StoryEpicDefinitionRegion
            {
                RegionId = craftRegion.RegionId,
                Label = craftRegion.Label,
                ImageUrl = publishedImageUrl,
                SortOrder = craftRegion.SortOrder,
                IsLocked = craftRegion.IsLocked,
                IsStartupRegion = craftRegion.IsStartupRegion,
                X = craftRegion.X,
                Y = craftRegion.Y
            });
        }

        return true;
    }

    private async Task<bool> ApplyStoryNodeChangeAsync(StoryEpicDefinition definition, StoryEpicCraft craft, EpicPublishChangeLog change, string ownerEmail, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            return true;
        }

        var storyId = change.EntityId;

        if (string.Equals(change.ChangeType, "Removed", StringComparison.OrdinalIgnoreCase))
        {
            var existingNode = definition.StoryNodes.FirstOrDefault(n => n.StoryId == storyId);
            if (existingNode != null)
            {
                _context.StoryEpicDefinitionStoryNodes.Remove(existingNode);
                definition.StoryNodes.Remove(existingNode);
            }
            return true;
        }

        var craftNode = craft.StoryNodes.FirstOrDefault(n => n.StoryId == storyId);
        if (craftNode == null)
        {
            _logger.LogWarning("Delta publish failed: epicId={EpicId} storyId={StoryId} missing in craft.", definition.Id, storyId);
            return false;
        }

        var rewardImageUrl = craftNode.RewardImageUrl;
        if (!string.IsNullOrWhiteSpace(craftNode.RewardImageUrl))
        {
            await _assetLinkService.SyncRewardImageAsync(craft, storyId, ownerEmail, ct);
            
            var normalizedPath = NormalizeBlobPath(craftNode.RewardImageUrl);
            var assetLink = await _context.EpicAssetLinks
                .Where(x => x.EpicId == craft.Id && x.DraftPath == normalizedPath && x.EntityId == $"__reward_image__{storyId}")
                .OrderByDescending(x => x.DraftVersion)
                .FirstOrDefaultAsync(ct);
            
            if (assetLink != null && !string.IsNullOrWhiteSpace(assetLink.PublishedPath))
            {
                rewardImageUrl = assetLink.PublishedPath;
            }
            else if (IsAlreadyPublished(normalizedPath))
            {
                rewardImageUrl = normalizedPath;
            }
        }

        var existingNode2 = definition.StoryNodes.FirstOrDefault(n => n.StoryId == storyId);
        if (existingNode2 != null)
        {
            existingNode2.RegionId = craftNode.RegionId;
            existingNode2.RewardImageUrl = rewardImageUrl;
            existingNode2.SortOrder = craftNode.SortOrder;
            existingNode2.X = craftNode.X;
            existingNode2.Y = craftNode.Y;
        }
        else
        {
            definition.StoryNodes.Add(new StoryEpicDefinitionStoryNode
            {
                StoryId = craftNode.StoryId,
                RegionId = craftNode.RegionId,
                RewardImageUrl = rewardImageUrl,
                SortOrder = craftNode.SortOrder,
                X = craftNode.X,
                Y = craftNode.Y
            });
        }

        return true;
    }

    private async Task<bool> ApplyUnlockRuleChangeAsync(StoryEpicDefinition definition, StoryEpicCraft craft, EpicPublishChangeLog change, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            return true;
        }

        var ruleId = change.EntityId;
        var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(change.PayloadJson ?? "{}");

        if (string.Equals(change.ChangeType, "Removed", StringComparison.OrdinalIgnoreCase))
        {
            var existingRule = definition.UnlockRules.FirstOrDefault(r => 
                $"{r.Type}:{r.FromId}:{r.ToRegionId}:{r.ToStoryId}" == ruleId);
            if (existingRule != null)
            {
                _context.StoryEpicDefinitionUnlockRules.Remove(existingRule);
                definition.UnlockRules.Remove(existingRule);
            }
            return true;
        }

        var craftRule = craft.UnlockRules.FirstOrDefault(r => 
            $"{r.Type}:{r.FromId}:{r.ToRegionId}:{r.ToStoryId}" == ruleId);
        if (craftRule == null)
        {
            _logger.LogWarning("Delta publish failed: epicId={EpicId} ruleId={RuleId} missing in craft.", definition.Id, ruleId);
            return false;
        }

        var existingRule2 = definition.UnlockRules.FirstOrDefault(r => 
            $"{r.Type}:{r.FromId}:{r.ToRegionId}:{r.ToStoryId}" == ruleId);
        if (existingRule2 != null)
        {
            existingRule2.Type = craftRule.Type;
            existingRule2.FromId = craftRule.FromId;
            existingRule2.ToRegionId = craftRule.ToRegionId;
            existingRule2.ToStoryId = craftRule.ToStoryId;
            existingRule2.RequiredStoriesCsv = craftRule.RequiredStoriesCsv;
            existingRule2.MinCount = craftRule.MinCount;
            existingRule2.StoryId = craftRule.StoryId;
            existingRule2.SortOrder = craftRule.SortOrder;
        }
        else
        {
            definition.UnlockRules.Add(new StoryEpicDefinitionUnlockRule
            {
                Type = craftRule.Type,
                FromId = craftRule.FromId,
                ToRegionId = craftRule.ToRegionId,
                ToStoryId = craftRule.ToStoryId,
                RequiredStoriesCsv = craftRule.RequiredStoriesCsv,
                MinCount = craftRule.MinCount,
                StoryId = craftRule.StoryId,
                SortOrder = craftRule.SortOrder
            });
        }

        return true;
    }

    private async Task<bool> ApplyTranslationChangeAsync(StoryEpicDefinition definition, StoryEpicCraft craft, EpicPublishChangeLog change, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            return true;
        }

        var languageCode = change.EntityId;

        if (string.Equals(change.ChangeType, "Removed", StringComparison.OrdinalIgnoreCase))
        {
            var existingTranslation = await _context.StoryEpicDefinitionTranslations
                .FirstOrDefaultAsync(t => t.StoryEpicDefinitionId == definition.Id && t.LanguageCode == languageCode, ct);
            
            if (existingTranslation != null)
            {
                _context.StoryEpicDefinitionTranslations.Remove(existingTranslation);
            }
            return true;
        }

        var craftTranslation = craft.Translations.FirstOrDefault(t => 
            string.Equals(t.LanguageCode, languageCode, StringComparison.OrdinalIgnoreCase));
        
        if (craftTranslation == null)
        {
            _logger.LogWarning("Delta publish failed: epicId={EpicId} languageCode={LanguageCode} missing in craft.", definition.Id, languageCode);
            return false;
        }

        var existingTranslation2 = await _context.StoryEpicDefinitionTranslations
            .FirstOrDefaultAsync(t => t.StoryEpicDefinitionId == definition.Id && t.LanguageCode == languageCode, ct);
        
        if (existingTranslation2 != null)
        {
            existingTranslation2.Name = craftTranslation.Name;
            existingTranslation2.Description = craftTranslation.Description;
        }
        else
        {
            definition.Translations.Add(new StoryEpicDefinitionTranslation
            {
                StoryEpicDefinitionId = definition.Id,
                LanguageCode = craftTranslation.LanguageCode,
                Name = craftTranslation.Name,
                Description = craftTranslation.Description
            });
        }

        return true;
    }

    private async Task<bool> ApplyHeroReferenceChangeAsync(StoryEpicDefinition definition, StoryEpicCraft craft, EpicPublishChangeLog change, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(change.EntityId))
        {
            return true;
        }

        var heroId = change.EntityId;

        if (string.Equals(change.ChangeType, "Removed", StringComparison.OrdinalIgnoreCase))
        {
            var existingHeroRef = await _context.StoryEpicHeroReferences
                .FirstOrDefaultAsync(h => h.EpicId == definition.Id && h.HeroId == heroId, ct);
            
            if (existingHeroRef != null)
            {
                _context.StoryEpicHeroReferences.Remove(existingHeroRef);
            }
            return true;
        }

        var craftHeroRef = craft.HeroReferences.FirstOrDefault(h => h.HeroId == heroId);
        if (craftHeroRef == null)
        {
            _logger.LogWarning("Delta publish failed: epicId={EpicId} heroId={HeroId} missing in craft.", definition.Id, heroId);
            return false;
        }

        var existingHeroRef2 = await _context.StoryEpicHeroReferences
            .FirstOrDefaultAsync(h => h.EpicId == definition.Id && h.HeroId == heroId, ct);
        
        if (existingHeroRef2 != null)
        {
            existingHeroRef2.StoryId = craftHeroRef.StoryId;
        }
        else
        {
            _context.StoryEpicHeroReferences.Add(new StoryEpicHeroReference
            {
                EpicId = definition.Id,
                HeroId = craftHeroRef.HeroId,
                StoryId = craftHeroRef.StoryId
            });
        }

        return true;
    }

    private async Task CleanupChangeLogsAsync(string epicId, int lastDraftVersion, CancellationToken ct)
    {
        if (lastDraftVersion <= 0)
        {
            return;
        }

        await _context.EpicPublishChangeLogs
            .Where(x => x.EpicId == epicId && x.DraftVersion <= lastDraftVersion)
            .ExecuteDeleteAsync(ct);
    }

    private async Task CleanupCraftAsync(StoryEpicCraft craft, CancellationToken ct)
    {
        try
        {
            var craftToDelete = await _context.StoryEpicCrafts
                .FirstOrDefaultAsync(c => c.Id == craft.Id, ct);
            
            if (craftToDelete != null)
            {
                _context.StoryEpicCrafts.Remove(craftToDelete);
                await _context.SaveChangesAsync(ct);
                _logger.LogInformation("Epic published and craft cleaned up: epicId={EpicId}", craft.Id);
            }
        }
        catch (Exception cleanupEx)
        {
            _logger.LogWarning(cleanupEx, "Failed to cleanup epic craft after publish: epicId={EpicId}, but publish succeeded", craft.Id);
        }
    }
}
