using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryEpicService : IStoryEpicService
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContext;
    private readonly IEpicPublishedAssetCleanupService _assetCleanup;
    private readonly IEpicPublishChangeLogService _changeLogService;
    private readonly ILogger<StoryEpicService> _logger;

    public StoryEpicService(
        XooDbContext context, 
        IUserContextService userContext, 
        IEpicPublishedAssetCleanupService assetCleanup,
        IEpicPublishChangeLogService changeLogService,
        ILogger<StoryEpicService> logger)
    {
        _context = context;
        _userContext = userContext;
        _assetCleanup = assetCleanup;
        _changeLogService = changeLogService;
        _logger = logger;
    }

    public async Task EnsureEpicAsync(Guid ownerUserId, string epicId, string name, CancellationToken ct = default)
    {
        // Check if craft exists
        var craftExists = await _context.StoryEpicCrafts.AnyAsync(c => c.Id == epicId, ct);
        if (!craftExists)
        {
            // Create new StoryEpicCraft
            var craft = new StoryEpicCraft
            {
                Id = epicId,
                Name = name,
                OwnerUserId = ownerUserId,
                Status = "draft",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.StoryEpicCrafts.Add(craft);
            
            // Create default translation (ro-ro) with the provided name
            var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
            craft.Translations.Add(new StoryEpicCraftTranslation
            {
                StoryEpicCraftId = craft.Id,
                LanguageCode = locale,
                Name = name
            });
            
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task SaveEpicAsync(Guid ownerUserId, string epicId, StoryEpicDto dto, bool isAdmin = false, CancellationToken ct = default)
    {
        // Load or create StoryEpicCraft (always work with draft)
        var craft = await _context.StoryEpicCrafts
            .Include(c => c.Regions)
            .Include(c => c.StoryNodes)
            .Include(c => c.UnlockRules)
            .Include(c => c.Translations)
            .Include(c => c.HeroReferences)
            .Include(c => c.Topics).ThenInclude(t => t.StoryTopic)
            .Include(c => c.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == epicId, ct);
        
        // Check ownership only if user is not admin
        if (craft != null && craft.OwnerUserId != ownerUserId && !isAdmin)
        {
            throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
        }

        // Create new craft if it doesn't exist
        if (craft == null)
        {
            // Use first translation name or dto.Name as fallback
            var defaultName = dto.Translations.FirstOrDefault()?.Name ?? dto.Name;
            craft = new StoryEpicCraft
            {
                Id = epicId,
                Name = defaultName,
                Description = dto.Description,
                OwnerUserId = ownerUserId,
                Status = dto.Status ?? "draft",
                CoverImageUrl = dto.CoverImageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.StoryEpicCrafts.Add(craft);
            
            // Create translations if provided
            if (dto.Translations.Any())
            {
                foreach (var translationDto in dto.Translations)
                {
                    craft.Translations.Add(new StoryEpicCraftTranslation
                    {
                        StoryEpicCraftId = craft.Id,
                        LanguageCode = translationDto.LanguageCode.ToLowerInvariant(),
                        Name = translationDto.Name,
                        Description = translationDto.Description
                    });
                }
            }
            else
            {
                // Create default translation (ro-ro) with the provided name
                var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
                craft.Translations.Add(new StoryEpicCraftTranslation
                {
                    StoryEpicCraftId = craft.Id,
                    LanguageCode = locale,
                    Name = defaultName,
                    Description = dto.Description
                });
            }
            await _context.SaveChangesAsync(ct);
            
            // Reload with collections initialized
            craft = await _context.StoryEpicCrafts
                .Include(c => c.Regions)
                .Include(c => c.StoryNodes)
                .Include(c => c.UnlockRules)
                .Include(c => c.Translations)
                .Include(c => c.Topics).ThenInclude(t => t.StoryTopic)
                .Include(c => c.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == epicId, ct);
            
            if (craft == null)
            {
                throw new InvalidOperationException($"Failed to create epic '{epicId}'");
            }
        }

        // Determine language code for change tracking (use first translation or default)
        const string defaultLang = "ro-ro";
        var langForTracking = dto.Translations?.FirstOrDefault()?.LanguageCode ?? defaultLang;
        
        // Capture snapshot before changes
        var snapshotBeforeChanges = _changeLogService.CaptureSnapshot(craft, langForTracking);

        // Update basic properties
        craft.CoverImageUrl = dto.CoverImageUrl;
        craft.Status = dto.Status ?? craft.Status;
        craft.UpdatedAt = DateTime.UtcNow;
        // Note: LastDraftVersion will be incremented in AppendChangesAsync

        // Update translations
        await UpdateCraftTranslationsAsync(craft, dto.Translations, ct);

        // Update regions
        await UpdateCraftRegionsAsync(craft, dto.Regions, ct);

        // Update story nodes
        await UpdateCraftStoryNodesAsync(craft, dto.Stories, ct);

        // Update unlock rules
        await UpdateCraftUnlockRulesAsync(craft, dto.Rules, ct);

        // Update hero references
        await UpdateCraftHeroReferencesAsync(craft, dto.Heroes, ct);

        // Update topic references
        await UpdateCraftTopicsAsync(craft, dto.TopicIds, ct);

        // Update age group references
        await UpdateCraftAgeGroupsAsync(craft, dto.AgeGroupIds, ct);

        // Save changes
        await _context.SaveChangesAsync(ct);

        // Append changes to change log after saving
        await _changeLogService.AppendChangesAsync(craft, snapshotBeforeChanges, langForTracking, ownerUserId, ct);
    }

    public async Task<StoryEpicDto?> GetEpicAsync(string epicId, CancellationToken ct = default)
    {
        // Try to get draft (StoryEpicCraft) first
        var craft = await _context.StoryEpicCrafts
            .Include(c => c.Regions)
            .Include(c => c.StoryNodes)
            .Include(c => c.UnlockRules)
            .Include(c => c.Translations)
            .Include(c => c.HeroReferences)
            .Include(c => c.Topics).ThenInclude(t => t.StoryTopic)
            .Include(c => c.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == epicId, ct);

        if (craft != null)
        {
            var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
            return MapCraftToDto(craft, locale);
        }

        // If no draft, try to get published (StoryEpicDefinition)
        var definition = await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.UnlockRules)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == epicId, ct);

        if (definition == null) return null;

        var locale2 = _userContext.GetRequestLocaleOrDefault("ro-ro");
        var heroes2 = await _context.StoryEpicHeroReferences
            .Where(h => h.EpicId == epicId)
            .ToListAsync(ct);
        return await MapDefinitionToDtoAsync(definition, locale2, heroes2, ct);
    }

    public async Task<StoryEpicStateDto?> GetEpicStateAsync(string epicId, CancellationToken ct = default)
    {
        var epic = await GetEpicAsync(epicId, ct);
        if (epic == null) return null;

        var preview = await GeneratePreviewAsync(epic, ct);

        return new StoryEpicStateDto
        {
            Epic = epic,
            Preview = preview
        };
    }

    public async Task<StoryEpicStateDto?> GetPublishedEpicStateAsync(string epicId, CancellationToken ct = default)
    {
        // Only get published epic (no draft fallback) - used for play mode
        var epic = await GetPublishedEpicAsync(epicId, ct);
        if (epic == null) return null;

        var preview = await GeneratePreviewAsync(epic, ct);

        return new StoryEpicStateDto
        {
            Epic = epic,
            Preview = preview
        };
    }

    public async Task<List<StoryEpicListItemDto>> ListEpicsByOwnerAsync(Guid ownerUserId, Guid? currentUserId = null, CancellationToken ct = default)
    {
        var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
        
        // Get both crafts (drafts) and definitions (published) for this owner
        var crafts = await _context.StoryEpicCrafts
            .Include(c => c.Regions)
            .Include(c => c.StoryNodes)
            .Include(c => c.Translations)
            .Where(c => c.OwnerUserId == ownerUserId)
            .ToListAsync(ct);
        
        var definitions = await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.Translations)
            .Where(d => d.OwnerUserId == ownerUserId)
            .ToListAsync(ct);
        
        // Get unique owner IDs for email lookup
        var uniqueOwnerIds = crafts.Select(c => c.OwnerUserId)
            .Concat(definitions.Select(d => d.OwnerUserId))
            .Distinct()
            .ToList();
        
        var ownerEmailMap = await _context.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => uniqueOwnerIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? "", ct);
        
        var result = new List<StoryEpicListItemDto>();
        
        // Add crafts (drafts)
        foreach (var craft in crafts)
        {
            var isOwnedByCurrentUser = currentUserId.HasValue && craft.OwnerUserId == currentUserId.Value;
            var isAssignedToCurrentUser = currentUserId.HasValue && 
                                          craft.AssignedReviewerUserId.HasValue && 
                                          craft.AssignedReviewerUserId.Value == currentUserId.Value;
            
            var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
            var name = translation?.Name ?? craft.Translations.FirstOrDefault()?.Name ?? craft.Name;
            var description = translation?.Description ?? craft.Translations.FirstOrDefault()?.Description ?? craft.Description;
            
            var ownerEmail = ownerEmailMap.TryGetValue(craft.OwnerUserId, out var email) ? email : "";
            
            result.Add(new StoryEpicListItemDto
            {
                Id = craft.Id,
                Name = name,
                Description = description,
                CoverImageUrl = craft.CoverImageUrl,
                Status = craft.Status,
                CreatedAt = craft.CreatedAt,
                UpdatedAt = craft.UpdatedAt,
                PublishedAtUtc = null, // Crafts are not published
                StoryCount = craft.StoryNodes.Count,
                RegionCount = craft.Regions.Count,
                AssignedReviewerUserId = craft.AssignedReviewerUserId,
                IsAssignedToCurrentUser = isAssignedToCurrentUser,
                IsOwnedByCurrentUser = isOwnedByCurrentUser,
                OwnerEmail = ownerEmail
            });
        }
        
        // Add definitions (published) - always include published epics, even if draft exists
        // This allows published epics to remain visible when "new version" creates a draft
        foreach (var definition in definitions)
        {
            var isOwnedByCurrentUser = currentUserId.HasValue && definition.OwnerUserId == currentUserId.Value;
            
            var translation = definition.Translations.FirstOrDefault(t => t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
            var name = translation?.Name ?? definition.Translations.FirstOrDefault()?.Name ?? definition.Name;
            var description = translation?.Description ?? definition.Translations.FirstOrDefault()?.Description ?? definition.Description;
            
            var ownerEmail = ownerEmailMap.TryGetValue(definition.OwnerUserId, out var email) ? email : "";
            
            result.Add(new StoryEpicListItemDto
            {
                Id = definition.Id,
                Name = name,
                Description = description,
                CoverImageUrl = definition.CoverImageUrl,
                Status = definition.Status,
                CreatedAt = definition.CreatedAt,
                UpdatedAt = definition.UpdatedAt,
                PublishedAtUtc = definition.PublishedAtUtc,
                StoryCount = definition.StoryNodes.Count,
                RegionCount = definition.Regions.Count,
                AssignedReviewerUserId = null, // Definitions don't have reviewers
                IsAssignedToCurrentUser = false,
                IsOwnedByCurrentUser = isOwnedByCurrentUser,
                OwnerEmail = ownerEmail
            });
        }
        
        return result.OrderByDescending(e => e.UpdatedAt).ToList();
    }

    public async Task<List<StoryEpicListItemDto>> ListAllEpicsAsync(Guid currentUserId, CancellationToken ct = default)
    {
        var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
        
        // Get all crafts (drafts) and definitions (published) - no owner filter
        var crafts = await _context.StoryEpicCrafts
            .Include(c => c.Regions)
            .Include(c => c.StoryNodes)
            .Include(c => c.Translations)
            .ToListAsync(ct);
        
        var definitions = await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.Translations)
            .ToListAsync(ct);
        
        // Get unique owner IDs for email lookup
        var uniqueOwnerIds = crafts.Select(c => c.OwnerUserId)
            .Concat(definitions.Select(d => d.OwnerUserId))
            .Distinct()
            .ToList();
        
        var ownerEmailMap = await _context.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => uniqueOwnerIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? "", ct);
        
        var result = new List<StoryEpicListItemDto>();
        
        // Add crafts (drafts)
        foreach (var craft in crafts)
        {
            var isOwnedByCurrentUser = craft.OwnerUserId == currentUserId;
            var isAssignedToCurrentUser = craft.AssignedReviewerUserId.HasValue && 
                                          craft.AssignedReviewerUserId.Value == currentUserId;
            
            var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
            var name = translation?.Name ?? craft.Translations.FirstOrDefault()?.Name ?? craft.Name;
            var description = translation?.Description ?? craft.Translations.FirstOrDefault()?.Description ?? craft.Description;
            
            var ownerEmail = ownerEmailMap.TryGetValue(craft.OwnerUserId, out var email) ? email : "";
            
            result.Add(new StoryEpicListItemDto
            {
                Id = craft.Id,
                Name = name,
                Description = description,
                CoverImageUrl = craft.CoverImageUrl,
                Status = craft.Status,
                CreatedAt = craft.CreatedAt,
                UpdatedAt = craft.UpdatedAt,
                PublishedAtUtc = null, // Crafts are not published
                StoryCount = craft.StoryNodes.Count,
                RegionCount = craft.Regions.Count,
                AssignedReviewerUserId = craft.AssignedReviewerUserId,
                IsAssignedToCurrentUser = isAssignedToCurrentUser,
                IsOwnedByCurrentUser = isOwnedByCurrentUser,
                OwnerEmail = ownerEmail
            });
        }
        
        // Add definitions (published) - always include published epics, even if draft exists
        // This allows published epics to remain visible when "new version" creates a draft
        foreach (var definition in definitions)
        {
            var isOwnedByCurrentUser = definition.OwnerUserId == currentUserId;
            
            var translation = definition.Translations.FirstOrDefault(t => t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
            var name = translation?.Name ?? definition.Translations.FirstOrDefault()?.Name ?? definition.Name;
            var description = translation?.Description ?? definition.Translations.FirstOrDefault()?.Description ?? definition.Description;
            
            var ownerEmail = ownerEmailMap.TryGetValue(definition.OwnerUserId, out var email) ? email : "";
            
            result.Add(new StoryEpicListItemDto
            {
                Id = definition.Id,
                Name = name,
                Description = description,
                CoverImageUrl = definition.CoverImageUrl,
                Status = definition.Status,
                CreatedAt = definition.CreatedAt,
                UpdatedAt = definition.UpdatedAt,
                PublishedAtUtc = definition.PublishedAtUtc,
                StoryCount = definition.StoryNodes.Count,
                RegionCount = definition.Regions.Count,
                AssignedReviewerUserId = null, // Definitions don't have reviewers
                IsAssignedToCurrentUser = false,
                IsOwnedByCurrentUser = isOwnedByCurrentUser,
                OwnerEmail = ownerEmail
            });
        }
        
        return result.OrderByDescending(e => e.UpdatedAt).ToList();
    }

    /// <summary>
    /// Deletes only the draft (StoryEpicCraft). Published version is never touched; use Retract/Unpublish for that.
    /// </summary>
    public async Task DeleteEpicDraftAsync(Guid requestingUserId, string epicId, bool allowAdminOverride = false, CancellationToken ct = default)
    {
        var craft = await _context.StoryEpicCrafts.FirstOrDefaultAsync(c => c.Id == epicId, ct);
        if (craft == null)
        {
            throw new InvalidOperationException($"Draft for epic '{epicId}' not found.");
        }

        if (!allowAdminOverride && craft.OwnerUserId != requestingUserId)
        {
            throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
        }

        _context.StoryEpicCrafts.Remove(craft);
        await _context.SaveChangesAsync(ct);
    }

    /// <summary>Same as DeleteEpicDraftAsync. Kept for backward compatibility.</summary>
    public async Task DeleteEpicAsync(Guid requestingUserId, string epicId, bool allowAdminOverride = false, CancellationToken ct = default)
    {
        await DeleteEpicDraftAsync(requestingUserId, epicId, allowAdminOverride, ct);
    }

    public async Task<int> CreateVersionFromPublishedAsync(Guid ownerUserId, string epicId, CancellationToken ct = default)
    {
        // Load published StoryEpicDefinition with all related data
        var definition = await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.UnlockRules)
            .Include(d => d.Translations)
            .Include(d => d.Topics)
            .Include(d => d.AgeGroups)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == epicId, ct);
        
        // Load hero references for the definition
        var heroReferences = await _context.StoryEpicHeroReferences
            .Where(h => h.EpicId == epicId)
            .ToListAsync(ct);

        if (definition == null)
        {
            throw new InvalidOperationException($"Published epic '{epicId}' not found");
        }

        // Verify ownership
        if (definition.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
        }

        // Check if epic is published
        if (definition.Status != "published")
        {
            throw new InvalidOperationException($"Epic '{epicId}' is not published (status: {definition.Status})");
        }

        // Check if draft already exists (use AsNoTracking to avoid tracking conflicts)
        var existingCraft = await _context.StoryEpicCrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == epicId, ct);
        
        if (existingCraft != null)
        {
            if (existingCraft.Status != "published")
            {
                throw new InvalidOperationException("A draft already exists for this epic. Please edit or publish it first.");
            }
            
            // If status is "published", this is a leftover craft that should have been deleted
            // Delete it now before creating the new version
            _logger.LogWarning("Found leftover published craft for epicId={EpicId}, deleting it before creating new version", epicId);
            var craftToDelete = await _context.StoryEpicCrafts.FirstOrDefaultAsync(c => c.Id == epicId, ct);
            if (craftToDelete != null)
            {
                _context.StoryEpicCrafts.Remove(craftToDelete);
                await _context.SaveChangesAsync(ct);
            }
        }

        // Create new StoryEpicCraft from StoryEpicDefinition
        var craft = new StoryEpicCraft
        {
            Id = definition.Id,
            Name = definition.Name,
            Description = definition.Description,
            OwnerUserId = definition.OwnerUserId,
            Status = "draft",
            CoverImageUrl = definition.CoverImageUrl,
            IsDefault = definition.IsDefault,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BaseVersion = definition.Version,
            LastDraftVersion = 0
        };

        // Copy Regions
        foreach (var defRegion in definition.Regions)
        {
            craft.Regions.Add(new StoryEpicCraftRegion
            {
                RegionId = defRegion.RegionId,
                Label = defRegion.Label,
                ImageUrl = defRegion.ImageUrl,
                SortOrder = defRegion.SortOrder,
                IsLocked = defRegion.IsLocked,
                IsStartupRegion = defRegion.IsStartupRegion,
                X = defRegion.X,
                Y = defRegion.Y
            });
        }

        // Copy StoryNodes
        foreach (var defNode in definition.StoryNodes)
        {
            craft.StoryNodes.Add(new StoryEpicCraftStoryNode
            {
                StoryId = defNode.StoryId,
                RegionId = defNode.RegionId,
                RewardImageUrl = defNode.RewardImageUrl,
                SortOrder = defNode.SortOrder,
                X = defNode.X,
                Y = defNode.Y
            });
        }

        // Copy UnlockRules
        foreach (var defRule in definition.UnlockRules)
        {
            craft.UnlockRules.Add(new StoryEpicCraftUnlockRule
            {
                Type = defRule.Type,
                FromId = defRule.FromId,
                ToRegionId = defRule.ToRegionId,
                ToStoryId = defRule.ToStoryId,
                RequiredStoriesCsv = defRule.RequiredStoriesCsv,
                MinCount = defRule.MinCount,
                StoryId = defRule.StoryId,
                SortOrder = defRule.SortOrder
            });
        }

        // Copy Translations
        foreach (var defTranslation in definition.Translations)
        {
            craft.Translations.Add(new StoryEpicCraftTranslation
            {
                StoryEpicCraftId = craft.Id,
                LanguageCode = defTranslation.LanguageCode,
                Name = defTranslation.Name,
                Description = defTranslation.Description
            });
        }

        _context.StoryEpicCrafts.Add(craft);
        await _context.SaveChangesAsync(ct);

        // Copy Hero References from StoryEpicHeroReference (definition) to StoryEpicCraftHeroReference (craft)
        foreach (var heroRef in heroReferences)
        {
            craft.HeroReferences.Add(new StoryEpicCraftHeroReference
            {
                EpicId = craft.Id,
                HeroId = heroRef.HeroId,
                StoryId = heroRef.StoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        
        await _context.SaveChangesAsync(ct);

        // Copy Topics from StoryEpicDefinition to StoryEpicCraft
        foreach (var topic in definition.Topics)
        {
            craft.Topics.Add(new StoryEpicCraftTopic
            {
                StoryEpicCraftId = craft.Id,
                StoryTopicId = topic.StoryTopicId,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Copy AgeGroups from StoryEpicDefinition to StoryEpicCraft
        foreach (var ageGroup in definition.AgeGroups)
        {
            craft.AgeGroups.Add(new StoryEpicCraftAgeGroup
            {
                StoryEpicCraftId = craft.Id,
                StoryAgeGroupId = ageGroup.StoryAgeGroupId,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Created StoryEpicCraft from StoryEpicDefinition: epicId={EpicId} baseVersion={BaseVersion} ownerId={OwnerId}",
            epicId, definition.Version, ownerUserId);

        // Return the base version
        return definition.Version;
    }

    private StoryEpicDto MapCraftToDto(StoryEpicCraft craft, string locale)
    {
        // Get translations
        var translations = craft.Translations.Select(t => new StoryEpicTranslationDto
        {
            LanguageCode = t.LanguageCode,
            Name = t.Name,
            Description = t.Description
        }).ToList();

        // Get name and description in requested locale
        var translation = translations.FirstOrDefault(t => t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
        var name = translation?.Name ?? translations.FirstOrDefault()?.Name ?? craft.Name;
        var description = translation?.Description ?? translations.FirstOrDefault()?.Description ?? craft.Description;

        return new StoryEpicDto
        {
            Id = craft.Id,
            Name = name,
            Description = description,
            CoverImageUrl = craft.CoverImageUrl,
            Status = craft.Status,
            PublishedAtUtc = null, // Crafts are not published
            Translations = translations,
            Regions = craft.Regions.Select(r => new StoryEpicRegionDto
            {
                Id = r.RegionId,
                Label = r.Label,
                ImageUrl = r.ImageUrl,
                SortOrder = r.SortOrder,
                IsLocked = r.IsLocked,
                IsStartupRegion = r.IsStartupRegion,
                X = r.X,
                Y = r.Y
            }).ToList(),
            Stories = craft.StoryNodes.Select(sn => new StoryEpicStoryNodeDto
            {
                StoryId = sn.StoryId,
                RegionId = sn.RegionId,
                RewardImageUrl = sn.RewardImageUrl,
                SortOrder = sn.SortOrder,
                X = sn.X,
                Y = sn.Y
            }).ToList(),
            Rules = craft.UnlockRules.Select(r => new StoryEpicUnlockRuleDto
            {
                Type = r.Type,
                FromId = r.FromId,
                ToRegionId = r.ToRegionId,
                ToStoryId = r.ToStoryId,
                RequiredStories = r.RequiredStoriesCsv?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                MinCount = r.MinCount,
                StoryId = r.StoryId,
                SortOrder = r.SortOrder
            }).ToList(),
            Heroes = craft.HeroReferences.Select(h => new StoryEpicHeroReferenceDto
            {
                HeroId = h.HeroId,
                StoryId = h.StoryId
            }).ToList(),
            TopicIds = craft.Topics
                .Select(t => t.StoryTopic?.TopicId ?? t.StoryTopicId.ToString())
                .ToList(),
            AgeGroupIds = craft.AgeGroups
                .Select(ag => ag.StoryAgeGroup?.AgeGroupId ?? ag.StoryAgeGroupId.ToString())
                .ToList()
        };
    }

    private async Task<StoryEpicDto> MapDefinitionToDtoAsync(StoryEpicDefinition definition, string locale, List<StoryEpicHeroReference> heroes, CancellationToken ct = default)
    {
        // Get translations
        var translations = definition.Translations.Select(t => new StoryEpicTranslationDto
        {
            LanguageCode = t.LanguageCode,
            Name = t.Name,
            Description = t.Description
        }).ToList();

        // Get name and description in requested locale
        var translation = translations.FirstOrDefault(t => t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
        var name = translation?.Name ?? translations.FirstOrDefault()?.Name ?? definition.Name;
        var description = translation?.Description ?? translations.FirstOrDefault()?.Description ?? definition.Description;

        // Load published regions with translations to get localized labels
        var regionIds = definition.Regions.Select(r => r.RegionId).ToList();
        var publishedRegions = await _context.StoryRegionDefinitions
            .Include(r => r.Translations)
            .Where(r => regionIds.Contains(r.Id) && r.Status == "published" && r.IsActive)
            .ToDictionaryAsync(r => r.Id, r => r, ct);

        // Map regions with translated labels
        var regions = new List<StoryEpicRegionDto>();
        foreach (var r in definition.Regions)
        {
            var publishedRegion = publishedRegions.TryGetValue(r.RegionId, out var region) ? region : null;
            var regionLabel = r.Label; // Default to stored label
            
            if (publishedRegion?.Translations != null && publishedRegion.Translations.Any())
            {
                // Try to get translation for requested locale
                var regionTranslation = publishedRegion.Translations.FirstOrDefault(t => 
                    t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
                regionLabel = regionTranslation?.Name ?? publishedRegion.Translations.FirstOrDefault()?.Name ?? r.Label;
            }

            regions.Add(new StoryEpicRegionDto
            {
                Id = r.RegionId,
                Label = regionLabel,
                ImageUrl = r.ImageUrl,
                SortOrder = r.SortOrder,
                IsLocked = r.IsLocked,
                IsStartupRegion = r.IsStartupRegion,
                X = r.X,
                Y = r.Y
            });
        }

        return new StoryEpicDto
        {
            Id = definition.Id,
            Name = name,
            Description = description,
            CoverImageUrl = definition.CoverImageUrl,
            Status = definition.Status,
            PublishedAtUtc = definition.PublishedAtUtc,
            Translations = translations,
            Regions = regions,
            Stories = definition.StoryNodes.Select(sn => new StoryEpicStoryNodeDto
            {
                StoryId = sn.StoryId,
                RegionId = sn.RegionId,
                RewardImageUrl = sn.RewardImageUrl,
                SortOrder = sn.SortOrder,
                X = sn.X,
                Y = sn.Y
            }).ToList(),
            Rules = definition.UnlockRules.Select(r => new StoryEpicUnlockRuleDto
            {
                Type = r.Type,
                FromId = r.FromId,
                ToRegionId = r.ToRegionId,
                ToStoryId = r.ToStoryId,
                RequiredStories = r.RequiredStoriesCsv?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                MinCount = r.MinCount,
                StoryId = r.StoryId,
                SortOrder = r.SortOrder
            }).ToList(),
            Heroes = heroes.Select(h => new StoryEpicHeroReferenceDto
            {
                HeroId = h.HeroId,
                StoryId = h.StoryId
            }).ToList(),
            TopicIds = definition.Topics
                .Select(t => t.StoryTopic?.TopicId ?? t.StoryTopicId.ToString())
                .ToList(),
            AgeGroupIds = definition.AgeGroups
                .Select(ag => ag.StoryAgeGroup?.AgeGroupId ?? ag.StoryAgeGroupId.ToString())
                .ToList()
        };
    }

    // REMOVED: All methods using DbStoryEpic (old architecture) - use StoryEpicCraft/StoryEpicDefinition instead

    public async Task<StoryEpicDto?> GetPublishedEpicAsync(string epicId, CancellationToken ct = default)
    {
        var definition = await GetStoryEpicDefinitionByIdAsync(epicId, ct);
        if (definition == null) return null;
        
        // Only return if epic is published and active (not unpublished)
        if (definition.Status != "published" || !definition.IsActive)
        {
            return null;
        }

        var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
        var heroes = await _context.StoryEpicHeroReferences
            .Where(h => h.EpicId == epicId)
            .ToListAsync(ct);
        
        return await MapDefinitionToDtoAsync(definition, locale, heroes, ct);
    }

    public async Task<StoryEpicDefinition?> GetStoryEpicDefinitionByIdAsync(string epicId, CancellationToken ct = default)
    {
        return await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.UnlockRules)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .Include(d => d.Owner)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == epicId, ct);
    }

    public async Task<List<StoryEpicDto>> GetAllPublishedEpicsAsync(string locale, CancellationToken ct = default)
    {
        var definitions = await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.UnlockRules)
            .Include(d => d.Translations)
            .Include(d => d.Topics).ThenInclude(t => t.StoryTopic)
            .Include(d => d.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .AsSplitQuery()
            .Where(d => d.Status == "published" && d.IsActive && d.PublishedAtUtc != null)
            .ToListAsync(ct);

        if (definitions.Count == 0)
        {
            return new List<StoryEpicDto>();
        }

        // Load all heroes for all epics in one query to avoid N+1
        var epicIds = definitions.Select(d => d.Id).ToList();
        var allHeroes = await _context.StoryEpicHeroReferences
            .Where(h => epicIds.Contains(h.EpicId))
            .ToListAsync(ct);
        
        var heroesByEpicId = allHeroes
            .GroupBy(h => h.EpicId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = new List<StoryEpicDto>();
        foreach (var definition in definitions)
        {
            var heroes = heroesByEpicId.GetValueOrDefault(definition.Id, new List<StoryEpicHeroReference>());
            result.Add(await MapDefinitionToDtoAsync(definition, locale, heroes, ct));
        }

        return result;
    }

    private async Task UpdateCraftTranslationsAsync(StoryEpicCraft craft, List<StoryEpicTranslationDto> translationDtos, CancellationToken ct)
    {
        var existingByLang = craft.Translations.ToDictionary(t => t.LanguageCode, StringComparer.OrdinalIgnoreCase);

        // Update or add translations
        foreach (var translationDto in translationDtos)
        {
            var langCode = translationDto.LanguageCode.ToLowerInvariant();
            if (existingByLang.TryGetValue(langCode, out var existingTranslation))
            {
                existingTranslation.Name = translationDto.Name;
                existingTranslation.Description = translationDto.Description;
            }
            else
            {
                craft.Translations.Add(new StoryEpicCraftTranslation
                {
                    StoryEpicCraftId = craft.Id,
                    LanguageCode = langCode,
                    Name = translationDto.Name,
                    Description = translationDto.Description
                });
            }
        }
    }

    private async Task UpdateCraftRegionsAsync(StoryEpicCraft craft, List<StoryEpicRegionDto> regionDtos, CancellationToken ct)
    {
        var dtoRegionIds = regionDtos.Select(r => r.Id).ToHashSet();
        var existingByRegionId = craft.Regions.ToDictionary(r => r.RegionId);

        // Remove regions not in DTO
        var toRemove = craft.Regions.Where(r => !dtoRegionIds.Contains(r.RegionId)).ToList();
        foreach (var region in toRemove)
        {
            craft.Regions.Remove(region);
        }

        // Update or add regions
        foreach (var regionDto in regionDtos)
        {
            if (existingByRegionId.TryGetValue(regionDto.Id, out var existingRegion))
            {
                existingRegion.Label = regionDto.Label;
                existingRegion.ImageUrl = regionDto.ImageUrl;
                existingRegion.SortOrder = regionDto.SortOrder;
                existingRegion.IsLocked = regionDto.IsLocked;
                existingRegion.IsStartupRegion = regionDto.IsStartupRegion;
                existingRegion.X = regionDto.X;
                existingRegion.Y = regionDto.Y;
                existingRegion.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                craft.Regions.Add(new StoryEpicCraftRegion
                {
                    RegionId = regionDto.Id,
                    Label = regionDto.Label,
                    ImageUrl = regionDto.ImageUrl,
                    SortOrder = regionDto.SortOrder,
                    IsLocked = regionDto.IsLocked,
                    IsStartupRegion = regionDto.IsStartupRegion,
                    X = regionDto.X,
                    Y = regionDto.Y,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
    }

    private async Task UpdateCraftStoryNodesAsync(StoryEpicCraft craft, List<StoryEpicStoryNodeDto> storyNodeDtos, CancellationToken ct)
    {
        var dtoStoryKeys = storyNodeDtos.Select(s => (s.StoryId, s.RegionId)).ToHashSet();
        var existingByKey = craft.StoryNodes.ToDictionary(sn => (sn.StoryId, sn.RegionId));

        // Remove nodes not in DTO
        var toRemove = craft.StoryNodes.Where(sn => !dtoStoryKeys.Contains((sn.StoryId, sn.RegionId))).ToList();
        foreach (var node in toRemove)
        {
            craft.StoryNodes.Remove(node);
        }

        // Update or add story nodes
        foreach (var storyNodeDto in storyNodeDtos)
        {
            var key = (storyNodeDto.StoryId, storyNodeDto.RegionId);
            if (existingByKey.TryGetValue(key, out var existingNode))
            {
                existingNode.RewardImageUrl = storyNodeDto.RewardImageUrl;
                existingNode.SortOrder = storyNodeDto.SortOrder;
                existingNode.X = storyNodeDto.X;
                existingNode.Y = storyNodeDto.Y;
                existingNode.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                craft.StoryNodes.Add(new StoryEpicCraftStoryNode
                {
                    StoryId = storyNodeDto.StoryId,
                    RegionId = storyNodeDto.RegionId,
                    RewardImageUrl = storyNodeDto.RewardImageUrl,
                    SortOrder = storyNodeDto.SortOrder,
                    X = storyNodeDto.X,
                    Y = storyNodeDto.Y,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        // Update IsPartOfEpic flag for stories
        var allStoryIds = storyNodeDtos.Select(s => s.StoryId).Distinct().ToList();
        if (allStoryIds.Any())
        {
            var craftsToUpdate = await _context.StoryCrafts
                .Where(c => allStoryIds.Contains(c.StoryId))
                .ToListAsync(ct);
            foreach (var storyCraft in craftsToUpdate)
            {
                storyCraft.IsPartOfEpic = true;
            }

            var definitionsToUpdate = await _context.StoryDefinitions
                .Where(d => allStoryIds.Contains(d.StoryId))
                .ToListAsync(ct);
            foreach (var storyDef in definitionsToUpdate)
            {
                storyDef.IsPartOfEpic = true;
            }
        }
    }

    private async Task UpdateCraftUnlockRulesAsync(StoryEpicCraft craft, List<StoryEpicUnlockRuleDto> ruleDtos, CancellationToken ct)
    {
        // Include ToStoryId in key so story-targeted rules don't collide with region-targeted rules.
        var dtoRuleKeys = ruleDtos.Select(r => (r.FromId, r.ToRegionId, r.ToStoryId ?? "", r.StoryId ?? "")).ToHashSet();
        var existingByKey = craft.UnlockRules.ToDictionary(r => (r.FromId, r.ToRegionId, r.ToStoryId ?? "", r.StoryId ?? ""));

        // Remove rules not in DTO
        var toRemove = craft.UnlockRules.Where(r => !dtoRuleKeys.Contains((r.FromId, r.ToRegionId, r.ToStoryId ?? "", r.StoryId ?? ""))).ToList();
        foreach (var rule in toRemove)
        {
            craft.UnlockRules.Remove(rule);
        }

        // Update or add rules
        foreach (var ruleDto in ruleDtos)
        {
            var key = (ruleDto.FromId, ruleDto.ToRegionId, ruleDto.ToStoryId ?? "", ruleDto.StoryId ?? "");
            if (existingByKey.TryGetValue(key, out var existingRule))
            {
                existingRule.Type = ruleDto.Type;
                existingRule.ToStoryId = ruleDto.ToStoryId;
                existingRule.RequiredStoriesCsv = ruleDto.RequiredStories.Any() 
                    ? string.Join(",", ruleDto.RequiredStories) 
                    : null;
                existingRule.MinCount = ruleDto.MinCount;
                existingRule.SortOrder = ruleDto.SortOrder;
                existingRule.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                craft.UnlockRules.Add(new StoryEpicCraftUnlockRule
                {
                    Type = ruleDto.Type,
                    FromId = ruleDto.FromId,
                    ToRegionId = ruleDto.ToRegionId,
                    ToStoryId = ruleDto.ToStoryId,
                    RequiredStoriesCsv = ruleDto.RequiredStories.Any() 
                        ? string.Join(",", ruleDto.RequiredStories) 
                        : null,
                    MinCount = ruleDto.MinCount,
                    StoryId = ruleDto.StoryId,
                    SortOrder = ruleDto.SortOrder,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
    }

    private async Task UpdateCraftHeroReferencesAsync(StoryEpicCraft craft, List<StoryEpicHeroReferenceDto> heroDtos, CancellationToken ct)
    {
        // Use StoryEpicCraftHeroReference (for drafts) instead of StoryEpicHeroReference (for published)
        var dtoHeroIds = heroDtos.Select(h => h.HeroId).ToHashSet();
        var existingByHeroId = craft.HeroReferences.ToDictionary(h => h.HeroId);
        
        // Remove hero references not in DTO
        var toRemove = craft.HeroReferences.Where(h => !dtoHeroIds.Contains(h.HeroId)).ToList();
        foreach (var heroRef in toRemove)
        {
            craft.HeroReferences.Remove(heroRef);
        }
        
        // Update or add hero references
        foreach (var heroDto in heroDtos)
        {
            if (existingByHeroId.TryGetValue(heroDto.HeroId, out var existingHeroRef))
            {
                existingHeroRef.StoryId = heroDto.StoryId;
                existingHeroRef.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                craft.HeroReferences.Add(new StoryEpicCraftHeroReference
                {
                    EpicId = craft.Id,
                    HeroId = heroDto.HeroId,
                    StoryId = heroDto.StoryId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }
    }

    // REMOVED: UpdateUnlockRulesAsync(Data.DbStoryEpic) - Old architecture removed, use UpdateCraftUnlockRulesAsync instead

    private async Task<StoryEpicPreviewDto> GeneratePreviewAsync(StoryEpicDto epic, CancellationToken ct = default)
    {
        var nodes = new List<StoryEpicPreviewNodeDto>();
        var edges = new List<StoryEpicPreviewEdgeDto>();

        // Get story titles and cover images from database (both published and draft)
        var storyIds = epic.Stories.Select(s => s.StoryId).ToList();
        
        // Get published story titles and cover images from StoryDefinitions
        var storyData = await _context.StoryDefinitions
            .Where(s => storyIds.Contains(s.StoryId) && s.IsActive)
            .Select(s => new { s.StoryId, s.Title, s.CoverImageUrl })
            .ToDictionaryAsync(s => s.StoryId, s => new { s.Title, s.CoverImageUrl }, ct);
        
        var storyTitles = storyData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Title);
        var storyCoverImages = storyData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.CoverImageUrl);
        
        // Get draft story titles and cover images from StoryCrafts (use first translation's title, or fallback to StoryId)
        var craftTitles = new Dictionary<string, string>();
        var craftCoverImages = new Dictionary<string, string?>();
        var crafts = await _context.StoryCrafts
            .Include(c => c.Translations)
            .Where(c => storyIds.Contains(c.StoryId))
            .ToListAsync(ct);
        
        foreach (var craft in crafts)
        {
            // Use first available translation title, or StoryId as fallback
            var title = craft.Translations.FirstOrDefault()?.Title;
            if (!string.IsNullOrWhiteSpace(title))
            {
                craftTitles[craft.StoryId] = title;
            }
            // Get cover image from craft
            if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
            {
                craftCoverImages[craft.StoryId] = craft.CoverImageUrl;
            }
        }

        // Add region nodes (regions already have translated labels from MapDefinitionToDtoAsync)
        foreach (var region in epic.Regions)
        {
            nodes.Add(new StoryEpicPreviewNodeDto
            {
                Id = region.Id,
                Type = "region",
                Label = region.Label, // Already translated in MapDefinitionToDtoAsync
                ImageUrl = region.ImageUrl,
                X = region.X,
                Y = region.Y
            });
        }

        // Add story nodes
        foreach (var story in epic.Stories)
        {
            // Get story title from StoryDefinition (published) or StoryCraft (draft), fallback to StoryId
            var storyTitle = storyTitles.TryGetValue(story.StoryId, out var title) 
                ? title 
                : (craftTitles.TryGetValue(story.StoryId, out var craftTitle) ? craftTitle : story.StoryId);
            
            // Get story cover image from StoryDefinition (published) or StoryCraft (draft)
            var storyCoverImage = storyCoverImages.TryGetValue(story.StoryId, out var coverImage) 
                ? coverImage 
                : (craftCoverImages.TryGetValue(story.StoryId, out var craftCoverImage) ? craftCoverImage : null);
            
            nodes.Add(new StoryEpicPreviewNodeDto
            {
                Id = story.StoryId,
                Type = "story",
                Label = storyTitle, // Use story title instead of ID
                ImageUrl = story.RewardImageUrl, // Reward image (shown when story is not completed)
                CoverImageUrl = storyCoverImage, // Cover image (shown when story is completed)
                X = story.X,
                Y = story.Y
            });

            // Add edge from region to story
            edges.Add(new StoryEpicPreviewEdgeDto
            {
                FromId = story.RegionId,
                ToId = story.StoryId,
                Type = "contains"
            });
        }

        // Add unlock rule edges
        foreach (var rule in epic.Rules)
        {
            var targetId = !string.IsNullOrWhiteSpace(rule.ToStoryId) ? rule.ToStoryId : rule.ToRegionId;
            edges.Add(new StoryEpicPreviewEdgeDto
            {
                FromId = rule.FromId,
                ToId = targetId,
                Type = "unlock"
            });
        }

        return new StoryEpicPreviewDto
        {
            Nodes = nodes,
            Edges = edges
        };
    }

    public async Task UnpublishAsync(Guid ownerUserId, string epicId, string reason, CancellationToken ct = default)
    {
        var definition = await GetStoryEpicDefinitionByIdAsync(epicId, ct);
        if (definition == null)
        {
            throw new InvalidOperationException($"Published epic '{epicId}' not found");
        }

        if (definition.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
        }

        if (definition.Status != "published" || !definition.IsActive)
        {
            throw new InvalidOperationException($"Cannot unpublish epic. Expected Published and Active, got status '{definition.Status}' and IsActive '{definition.IsActive}'");
        }

        // Mark as unpublished (soft delete - preserves player progress in EpicProgress table)
        definition.Status = "unpublished";
        definition.IsActive = false;
        definition.UpdatedAt = DateTime.UtcNow;

        // NOTE: We intentionally DO NOT delete EpicProgress or EpicStoryProgress records
        // This preserves player history and allows them to see the epic was decomissioned
        // Players will see a message that the epic is no longer available for security reasons

        await _context.SaveChangesAsync(ct);
        
        // Extract owner email from CoverImageUrl and delete published assets from blob storage
        var ownerEmail = TryExtractOwnerEmail(definition);
        if (!string.IsNullOrWhiteSpace(ownerEmail))
        {
            await _assetCleanup.DeletePublishedAssetsAsync(ownerEmail, epicId, ct);
        }
        else
        {
            _logger.LogWarning("Could not determine owner email for published assets cleanup. epicId={EpicId}", epicId);
        }
        
        _logger.LogInformation("Epic unpublished and assets deleted: epicId={EpicId} reason={Reason}", epicId, reason);
    }

    private string? TryExtractOwnerEmail(StoryEpicDefinition definition)
    {
        if (string.IsNullOrWhiteSpace(definition.CoverImageUrl))
        {
            return null;
        }

        // CoverImageUrl format: images/tales-of-alchimalia/epics/{ownerEmail}/{epicId}/...
        var parts = definition.CoverImageUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 5 && parts[0] == "images" && parts[1] == "tales-of-alchimalia" && parts[2] == "epics")
        {
            return parts[3]; // ownerEmail
        }

        return null;
    }

    private async Task UpdateCraftTopicsAsync(StoryEpicCraft craft, List<string> topicIds, CancellationToken ct)
    {
        var existingTopics = await _context.Set<StoryEpicCraftTopic>()
            .Where(t => t.StoryEpicCraftId == craft.Id)
            .ToListAsync(ct);
        _context.RemoveRange(existingTopics);

        if (topicIds == null || topicIds.Count == 0)
        {
            return;
        }

        var topics = await _context.StoryTopics
            .Where(t => topicIds.Contains(t.TopicId))
            .ToListAsync(ct);

        foreach (var topic in topics)
        {
            _context.Set<StoryEpicCraftTopic>().Add(new StoryEpicCraftTopic
            {
                StoryEpicCraftId = craft.Id,
                StoryTopicId = topic.Id,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task UpdateCraftAgeGroupsAsync(StoryEpicCraft craft, List<string> ageGroupIds, CancellationToken ct)
    {
        var existingAgeGroups = await _context.Set<StoryEpicCraftAgeGroup>()
            .Where(ag => ag.StoryEpicCraftId == craft.Id)
            .ToListAsync(ct);
        _context.RemoveRange(existingAgeGroups);

        if (ageGroupIds == null || ageGroupIds.Count == 0)
        {
            return;
        }

        var ageGroups = await _context.StoryAgeGroups
            .Where(ag => ageGroupIds.Contains(ag.AgeGroupId))
            .ToListAsync(ct);

        foreach (var ageGroup in ageGroups)
        {
            _context.Set<StoryEpicCraftAgeGroup>().Add(new StoryEpicCraftAgeGroup
            {
                StoryEpicCraftId = craft.Id,
                StoryAgeGroupId = ageGroup.Id,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}

