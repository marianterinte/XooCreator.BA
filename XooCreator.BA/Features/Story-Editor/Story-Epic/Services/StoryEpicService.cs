using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryEpicService : IStoryEpicService
{
    private readonly IStoryEpicRepository _repository;
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContext;
    private readonly ILogger<StoryEpicService> _logger;

    public StoryEpicService(IStoryEpicRepository repository, XooDbContext context, IUserContextService userContext, ILogger<StoryEpicService> logger)
    {
        _repository = repository;
        _context = context;
        _userContext = userContext;
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

    public async Task SaveEpicAsync(Guid ownerUserId, string epicId, StoryEpicDto dto, CancellationToken ct = default)
    {
        // Load or create StoryEpicCraft (always work with draft)
        var craft = await _context.StoryEpicCrafts
            .Include(c => c.Regions)
            .Include(c => c.StoryNodes)
            .Include(c => c.UnlockRules)
            .Include(c => c.Translations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == epicId, ct);
        
        if (craft != null && craft.OwnerUserId != ownerUserId)
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
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == epicId, ct);
            
            if (craft == null)
            {
                throw new InvalidOperationException($"Failed to create epic '{epicId}'");
            }
        }

        // Update basic properties
        craft.CoverImageUrl = dto.CoverImageUrl;
        craft.Status = dto.Status ?? craft.Status;
        craft.UpdatedAt = DateTime.UtcNow;
        craft.LastDraftVersion += 1; // Increment draft version on each save

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

        // Save changes
        await _context.SaveChangesAsync(ct);
    }

    public async Task<StoryEpicDto?> GetEpicAsync(string epicId, CancellationToken ct = default)
    {
        // Try to get draft (StoryEpicCraft) first
        var craft = await _context.StoryEpicCrafts
            .Include(c => c.Regions)
            .Include(c => c.StoryNodes)
            .Include(c => c.UnlockRules)
            .Include(c => c.Translations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == epicId, ct);

        if (craft != null)
        {
            var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
            var heroes = await _context.StoryEpicHeroReferences
                .Where(h => h.EpicId == epicId)
                .ToListAsync(ct);
            return MapCraftToDto(craft, locale, heroes);
        }

        // If no draft, try to get published (StoryEpicDefinition)
        var definition = await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.UnlockRules)
            .Include(d => d.Translations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == epicId, ct);

        if (definition == null) return null;

        var locale2 = _userContext.GetRequestLocaleOrDefault("ro-ro");
        var heroes2 = await _context.StoryEpicHeroReferences
            .Where(h => h.EpicId == epicId)
            .ToListAsync(ct);
        return MapDefinitionToDto(definition, locale2, heroes2);
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
                IsOwnedByCurrentUser = isOwnedByCurrentUser
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
                IsOwnedByCurrentUser = isOwnedByCurrentUser
            });
        }
        
        return result.OrderByDescending(e => e.UpdatedAt).ToList();
    }

    public async Task DeleteEpicAsync(Guid ownerUserId, string epicId, CancellationToken ct = default)
    {
        // Delete craft if exists
        var craft = await _context.StoryEpicCrafts.FirstOrDefaultAsync(c => c.Id == epicId, ct);
        if (craft != null)
        {
            if (craft.OwnerUserId != ownerUserId)
            {
                throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
            }
            _context.StoryEpicCrafts.Remove(craft);
        }
        
        // Delete definition if exists
        var definition = await _context.StoryEpicDefinitions.FirstOrDefaultAsync(d => d.Id == epicId, ct);
        if (definition != null)
        {
            if (definition.OwnerUserId != ownerUserId)
            {
                throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
            }
            _context.StoryEpicDefinitions.Remove(definition);
        }
        
        if (craft == null && definition == null)
        {
            throw new InvalidOperationException($"Epic '{epicId}' not found");
        }
        
        await _context.SaveChangesAsync(ct);
    }

    public async Task<int> CreateVersionFromPublishedAsync(Guid ownerUserId, string epicId, CancellationToken ct = default)
    {
        // Load published StoryEpicDefinition with all related data
        var definition = await _context.StoryEpicDefinitions
            .Include(d => d.Regions)
            .Include(d => d.StoryNodes)
            .Include(d => d.UnlockRules)
            .Include(d => d.Translations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(d => d.Id == epicId, ct);

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

        // Check if draft already exists
        var existingCraft = await _context.StoryEpicCrafts.FirstOrDefaultAsync(c => c.Id == epicId, ct);
        if (existingCraft != null && existingCraft.Status != "published")
        {
            throw new InvalidOperationException("A draft already exists for this epic. Please edit or publish it first.");
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

        _logger.LogInformation(
            "Created StoryEpicCraft from StoryEpicDefinition: epicId={EpicId} baseVersion={BaseVersion} ownerId={OwnerId}",
            epicId, definition.Version, ownerUserId);

        // Return the base version
        return definition.Version;
    }

    private StoryEpicDto MapCraftToDto(StoryEpicCraft craft, string locale, List<StoryEpicHeroReference> heroes)
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
                RequiredStories = r.RequiredStoriesCsv?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                MinCount = r.MinCount,
                StoryId = r.StoryId,
                SortOrder = r.SortOrder
            }).ToList(),
            Heroes = heroes.Select(h => new StoryEpicHeroReferenceDto
            {
                HeroId = h.HeroId,
                StoryId = h.StoryId
            }).ToList()
        };
    }

    private StoryEpicDto MapDefinitionToDto(StoryEpicDefinition definition, string locale, List<StoryEpicHeroReference> heroes)
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

        return new StoryEpicDto
        {
            Id = definition.Id,
            Name = name,
            Description = description,
            CoverImageUrl = definition.CoverImageUrl,
            Status = definition.Status,
            PublishedAtUtc = definition.PublishedAtUtc,
            Translations = translations,
            Regions = definition.Regions.Select(r => new StoryEpicRegionDto
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
                RequiredStories = r.RequiredStoriesCsv?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                MinCount = r.MinCount,
                StoryId = r.StoryId,
                SortOrder = r.SortOrder
            }).ToList(),
            Heroes = heroes.Select(h => new StoryEpicHeroReferenceDto
            {
                HeroId = h.HeroId,
                StoryId = h.StoryId
            }).ToList()
        };
    }

    private StoryEpicDto MapToDto(Data.DbStoryEpic epic, string locale)
    {
        // Get translations
        var translations = epic.Translations.Select(t => new StoryEpicTranslationDto
        {
            LanguageCode = t.LanguageCode,
            Name = t.Name,
            Description = t.Description
        }).ToList();

        // Get name and description in requested locale (fallback to first available or epic.Name/Description)
        var translation = translations.FirstOrDefault(t => t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
        var name = translation?.Name ?? translations.FirstOrDefault()?.Name ?? epic.Name;
        var description = translation?.Description ?? translations.FirstOrDefault()?.Description ?? epic.Description;

        return new StoryEpicDto
        {
            Id = epic.Id,
            Name = name,
            Description = description,
            CoverImageUrl = epic.CoverImageUrl,
            Status = epic.Status,
            PublishedAtUtc = epic.PublishedAtUtc,
            Translations = translations,
            Regions = epic.Regions.Select(r => new StoryEpicRegionDto
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
            Stories = epic.StoryNodes.Select(sn => new StoryEpicStoryNodeDto
            {
                StoryId = sn.StoryId,
                RegionId = sn.RegionId,
                RewardImageUrl = sn.RewardImageUrl,
                SortOrder = sn.SortOrder,
                X = sn.X,
                Y = sn.Y
            }).ToList(),
            Rules = epic.UnlockRules.Select(r => new StoryEpicUnlockRuleDto
            {
                Type = r.Type,
                FromId = r.FromId,
                ToRegionId = r.ToRegionId,
                RequiredStories = r.RequiredStoriesCsv?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>(),
                MinCount = r.MinCount,
                StoryId = r.StoryId,
                SortOrder = r.SortOrder
            }).ToList()
        };
    }

    private async Task UpdateTranslationsAsync(Data.DbStoryEpic epic, List<StoryEpicTranslationDto> translationDtos, CancellationToken ct)
    {
        // Get existing translations from DB
        var existingTranslations = await _context.StoryEpicTranslations
            .Where(t => t.StoryEpicId == epic.Id)
            .ToListAsync(ct);
        var existingByLang = existingTranslations.ToDictionary(t => t.LanguageCode, StringComparer.OrdinalIgnoreCase);

        // Update or add translations
        foreach (var translationDto in translationDtos)
        {
            var langCode = translationDto.LanguageCode.ToLowerInvariant();
            if (existingByLang.TryGetValue(langCode, out var existingTranslation))
            {
                // Update existing
                existingTranslation.Name = translationDto.Name;
                existingTranslation.Description = translationDto.Description;
            }
            else
            {
                // Add new
                var newTranslation = new Data.StoryEpicTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryEpicId = epic.Id,
                    LanguageCode = langCode,
                    Name = translationDto.Name,
                    Description = translationDto.Description
                };
                _context.StoryEpicTranslations.Add(newTranslation);
            }
        }

        // Remove translations not in DTO (optional - we might want to keep all translations)
        // For now, we'll keep all existing translations and only update/add what's in the DTO
    }

    private async Task UpdateRegionsAsync(Data.DbStoryEpic epic, List<StoryEpicRegionDto> regionDtos, CancellationToken ct)
    {
        var dtoRegionIds = regionDtos.Select(r => r.Id).ToHashSet();
        
        // Get existing regions from DB for this epic
        var existingDbRegions = await _context.StoryEpicRegions
            .Where(r => r.EpicId == epic.Id)
            .ToListAsync(ct);
        var existingDbRegionIds = existingDbRegions.ToDictionary(r => r.RegionId);
        
        // Remove regions not in DTO
        foreach (var dbRegion in existingDbRegions)
        {
            if (!dtoRegionIds.Contains(dbRegion.RegionId))
            {
                _context.StoryEpicRegions.Remove(dbRegion);
            }
        }

        // Update or add regions
        foreach (var regionDto in regionDtos)
        {
            if (existingDbRegionIds.TryGetValue(regionDto.Id, out var existingRegion))
            {
                // Update existing
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
                // Add new
                var newRegion = new Data.StoryEpicRegion
                {
                    EpicId = epic.Id,
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
                };
                _context.StoryEpicRegions.Add(newRegion);
            }
        }
    }

    private async Task UpdateStoryNodesAsync(Data.DbStoryEpic epic, List<StoryEpicStoryNodeDto> storyNodeDtos, CancellationToken ct)
    {
        var dtoStoryKeys = storyNodeDtos.Select(s => (s.StoryId, s.RegionId)).ToHashSet();
        
        // Get existing story nodes from DB for this epic
        var existingDbNodes = await _context.StoryEpicStoryNodes
            .Where(sn => sn.EpicId == epic.Id)
            .ToListAsync(ct);
        var existingDbNodesByKey = existingDbNodes.ToDictionary(sn => (sn.StoryId, sn.RegionId));
        
        // Collect story IDs that will be removed and added
        var removedStoryIds = new HashSet<string>();
        var addedStoryIds = new HashSet<string>();

        // Remove nodes not in DTO
        foreach (var dbNode in existingDbNodes)
        {
            if (!dtoStoryKeys.Contains((dbNode.StoryId, dbNode.RegionId)))
            {
                removedStoryIds.Add(dbNode.StoryId);
                _context.StoryEpicStoryNodes.Remove(dbNode);
            }
        }

        // Update or add story nodes
        foreach (var storyNodeDto in storyNodeDtos)
        {
            var key = (storyNodeDto.StoryId, storyNodeDto.RegionId);
            if (existingDbNodesByKey.TryGetValue(key, out var existingNode))
            {
                // Update existing
                existingNode.RewardImageUrl = storyNodeDto.RewardImageUrl;
                existingNode.SortOrder = storyNodeDto.SortOrder;
                existingNode.X = storyNodeDto.X;
                existingNode.Y = storyNodeDto.Y;
                existingNode.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new
                addedStoryIds.Add(storyNodeDto.StoryId);
                var newNode = new Data.StoryEpicStoryNode
                {
                    EpicId = epic.Id,
                    StoryId = storyNodeDto.StoryId,
                    RegionId = storyNodeDto.RegionId,
                    RewardImageUrl = storyNodeDto.RewardImageUrl,
                    SortOrder = storyNodeDto.SortOrder,
                    X = storyNodeDto.X,
                    Y = storyNodeDto.Y,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.StoryEpicStoryNodes.Add(newNode);
            }
        }

        // Update IsPartOfEpic flag for stories that were added to epic
        if (addedStoryIds.Any())
        {
            // Update StoryCrafts
            var craftsToUpdate = await _context.StoryCrafts
                .Where(c => addedStoryIds.Contains(c.StoryId))
                .ToListAsync(ct);
            foreach (var craft in craftsToUpdate)
            {
                craft.IsPartOfEpic = true;
            }

            // Update StoryDefinitions
            var definitionsToUpdate = await _context.StoryDefinitions
                .Where(d => addedStoryIds.Contains(d.StoryId))
                .ToListAsync(ct);
            foreach (var def in definitionsToUpdate)
            {
                def.IsPartOfEpic = true;
            }
        }

        // Update IsPartOfEpic flag for stories that were removed from epic
        if (removedStoryIds.Any())
        {
            // Check if story is still in any other epic
            var storiesStillInEpics = await _context.StoryEpicStoryNodes
                .Where(sn => removedStoryIds.Contains(sn.StoryId))
                .Select(sn => sn.StoryId)
                .Distinct()
                .ToListAsync(ct);
            
            var storiesToRemoveFlag = removedStoryIds.Except(storiesStillInEpics).ToList();

            if (storiesToRemoveFlag.Any())
            {
                // Update StoryCrafts
                var craftsToUpdate = await _context.StoryCrafts
                    .Where(c => storiesToRemoveFlag.Contains(c.StoryId))
                    .ToListAsync(ct);
                foreach (var craft in craftsToUpdate)
                {
                    craft.IsPartOfEpic = false;
                }

                // Update StoryDefinitions
                var definitionsToUpdate = await _context.StoryDefinitions
                    .Where(d => storiesToRemoveFlag.Contains(d.StoryId))
                    .ToListAsync(ct);
                foreach (var def in definitionsToUpdate)
                {
                    def.IsPartOfEpic = false;
                }
            }
        }
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
        var dtoRuleKeys = ruleDtos.Select(r => (r.FromId, r.ToRegionId, r.StoryId ?? "")).ToHashSet();
        var existingByKey = craft.UnlockRules.ToDictionary(r => (r.FromId, r.ToRegionId, r.StoryId ?? ""));

        // Remove rules not in DTO
        var toRemove = craft.UnlockRules.Where(r => !dtoRuleKeys.Contains((r.FromId, r.ToRegionId, r.StoryId ?? ""))).ToList();
        foreach (var rule in toRemove)
        {
            craft.UnlockRules.Remove(rule);
        }

        // Update or add rules
        foreach (var ruleDto in ruleDtos)
        {
            var key = (ruleDto.FromId, ruleDto.ToRegionId, ruleDto.StoryId ?? "");
            if (existingByKey.TryGetValue(key, out var existingRule))
            {
                existingRule.Type = ruleDto.Type;
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
        // Get existing hero references from DB for this epic
        var existingHeroRefs = await _context.StoryEpicHeroReferences
            .Where(h => h.EpicId == craft.Id)
            .ToListAsync(ct);
        
        var dtoHeroIds = heroDtos.Select(h => h.HeroId).ToHashSet();
        var existingByHeroId = existingHeroRefs.ToDictionary(h => h.HeroId);
        
        // Remove hero references not in DTO
        var toRemove = existingHeroRefs.Where(h => !dtoHeroIds.Contains(h.HeroId)).ToList();
        foreach (var heroRef in toRemove)
        {
            _context.StoryEpicHeroReferences.Remove(heroRef);
        }
        
        // Update or add hero references
        foreach (var heroDto in heroDtos)
        {
            if (existingByHeroId.TryGetValue(heroDto.HeroId, out var existingHeroRef))
            {
                existingHeroRef.StoryId = heroDto.StoryId;
            }
            else
            {
                _context.StoryEpicHeroReferences.Add(new StoryEpicHeroReference
                {
                    EpicId = craft.Id,
                    HeroId = heroDto.HeroId,
                    StoryId = heroDto.StoryId
                });
            }
        }
    }

    private async Task UpdateUnlockRulesAsync(Data.DbStoryEpic epic, List<StoryEpicUnlockRuleDto> ruleDtos, CancellationToken ct)
    {
        // Get existing rules from DB for this epic
        var existingDbRules = await _context.StoryEpicUnlockRules
            .Where(r => r.EpicId == epic.Id)
            .ToListAsync(ct);
        
        // Create key for matching rules
        var dtoRuleKeys = ruleDtos.Select(r => (r.FromId, r.ToRegionId, r.StoryId ?? "")).ToHashSet();
        var existingDbRulesByKey = existingDbRules.ToDictionary(r => (r.FromId, r.ToRegionId, r.StoryId ?? ""));
        
        // Remove rules not in DTO
        foreach (var dbRule in existingDbRules)
        {
            var key = (dbRule.FromId, dbRule.ToRegionId, dbRule.StoryId ?? "");
            if (!dtoRuleKeys.Contains(key))
            {
                _context.StoryEpicUnlockRules.Remove(dbRule);
            }
        }

        // Update or add rules
        foreach (var ruleDto in ruleDtos)
        {
            var key = (ruleDto.FromId, ruleDto.ToRegionId, ruleDto.StoryId ?? "");
            if (existingDbRulesByKey.TryGetValue(key, out var existingRule))
            {
                // Update existing
                existingRule.Type = ruleDto.Type;
                existingRule.RequiredStoriesCsv = ruleDto.RequiredStories.Any() 
                    ? string.Join(",", ruleDto.RequiredStories) 
                    : null;
                existingRule.MinCount = ruleDto.MinCount;
                existingRule.SortOrder = ruleDto.SortOrder;
                existingRule.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new
                var newRule = new Data.StoryEpicUnlockRule
                {
                    EpicId = epic.Id,
                    Type = ruleDto.Type,
                    FromId = ruleDto.FromId,
                    ToRegionId = ruleDto.ToRegionId,
                    RequiredStoriesCsv = ruleDto.RequiredStories.Any() 
                        ? string.Join(",", ruleDto.RequiredStories) 
                        : null,
                    MinCount = ruleDto.MinCount,
                    StoryId = ruleDto.StoryId,
                    SortOrder = ruleDto.SortOrder,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.StoryEpicUnlockRules.Add(newRule);
            }
        }
    }

    private async Task<StoryEpicPreviewDto> GeneratePreviewAsync(StoryEpicDto epic, CancellationToken ct = default)
    {
        var nodes = new List<StoryEpicPreviewNodeDto>();
        var edges = new List<StoryEpicPreviewEdgeDto>();

        // Get story titles from database (both published and draft)
        var storyIds = epic.Stories.Select(s => s.StoryId).ToList();
        
        // Get published story titles from StoryDefinitions
        var storyTitles = await _context.StoryDefinitions
            .Where(s => storyIds.Contains(s.StoryId) && s.IsActive)
            .Select(s => new { s.StoryId, s.Title })
            .ToDictionaryAsync(s => s.StoryId, s => s.Title, ct);
        
        // Get draft story titles from StoryCrafts (use first translation's title, or fallback to StoryId)
        var craftTitles = new Dictionary<string, string>();
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
        }

        // Add region nodes
        foreach (var region in epic.Regions)
        {
            nodes.Add(new StoryEpicPreviewNodeDto
            {
                Id = region.Id,
                Type = "region",
                Label = region.Label,
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
            
            nodes.Add(new StoryEpicPreviewNodeDto
            {
                Id = story.StoryId,
                Type = "story",
                Label = storyTitle, // Use story title instead of ID
                ImageUrl = story.RewardImageUrl,
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
            edges.Add(new StoryEpicPreviewEdgeDto
            {
                FromId = rule.FromId,
                ToId = rule.ToRegionId,
                Type = "unlock"
            });
        }

        return new StoryEpicPreviewDto
        {
            Nodes = nodes,
            Edges = edges
        };
    }
}

