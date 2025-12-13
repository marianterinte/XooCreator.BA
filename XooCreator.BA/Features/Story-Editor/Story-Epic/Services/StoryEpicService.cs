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

    public StoryEpicService(IStoryEpicRepository repository, XooDbContext context, IUserContextService userContext)
    {
        _repository = repository;
        _context = context;
        _userContext = userContext;
    }

    public async Task EnsureEpicAsync(Guid ownerUserId, string epicId, string name, CancellationToken ct = default)
    {
        var exists = await _repository.ExistsAsync(epicId, ct);
        if (!exists)
        {
            var epic = await _repository.CreateAsync(ownerUserId, epicId, name, ct);
            // Create default translation (ro-ro) with the provided name
            var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
            var defaultTranslation = new Data.StoryEpicTranslation
            {
                Id = Guid.NewGuid(),
                StoryEpicId = epic.Id,
                LanguageCode = locale,
                Name = name
            };
            _context.StoryEpicTranslations.Add(defaultTranslation);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task SaveEpicAsync(Guid ownerUserId, string epicId, StoryEpicDto dto, CancellationToken ct = default)
    {
        // Use GetFullAsync to load related entities (regions, stories, rules)
        var existing = await _repository.GetFullAsync(epicId, ct);
        
        if (existing != null && existing.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
        }

        // Ensure epic exists
        if (existing == null)
        {
            // Use first translation name or dto.Name as fallback
            var defaultName = dto.Translations.FirstOrDefault()?.Name ?? dto.Name;
            existing = await _repository.CreateAsync(ownerUserId, epicId, defaultName, ct);
            
            // Create translations if provided
            if (dto.Translations.Any())
            {
                foreach (var translationDto in dto.Translations)
                {
                    var translation = new Data.StoryEpicTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryEpicId = existing.Id,
                        LanguageCode = translationDto.LanguageCode.ToLowerInvariant(),
                        Name = translationDto.Name,
                        Description = translationDto.Description
                    };
                    _context.StoryEpicTranslations.Add(translation);
                }
            }
            else
            {
                // Create default translation (ro-ro) with the provided name
                var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
                var defaultTranslation = new Data.StoryEpicTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryEpicId = existing.Id,
                    LanguageCode = locale,
                    Name = defaultName,
                    Description = dto.Description
                };
                _context.StoryEpicTranslations.Add(defaultTranslation);
            }
            await _context.SaveChangesAsync(ct);
            
            // After creation, we need to reload with collections initialized
            existing = await _repository.GetFullAsync(epicId, ct);
            if (existing == null)
            {
                throw new InvalidOperationException($"Failed to create epic '{epicId}'");
            }
        }

        // Update basic properties (keep Name and Description for backward compatibility, but use translations)
        existing.CoverImageUrl = dto.CoverImageUrl;
        existing.Status = dto.Status;
        existing.UpdatedAt = DateTime.UtcNow;

        // Update translations
        await UpdateTranslationsAsync(existing, dto.Translations, ct);

        // Update regions (now with proper tracking since we used GetFullAsync)
        await UpdateRegionsAsync(existing, dto.Regions, ct);

        // Update story nodes
        await UpdateStoryNodesAsync(existing, dto.Stories, ct);

        // Update unlock rules
        await UpdateUnlockRulesAsync(existing, dto.Rules, ct);

        // Save changes
        await _repository.SaveAsync(existing, ct);
    }

    public async Task<StoryEpicDto?> GetEpicAsync(string epicId, CancellationToken ct = default)
    {
        var epic = await _repository.GetFullAsync(epicId, ct);
        if (epic == null) return null;

        var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
        return MapToDto(epic, locale);
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
        var epics = await _repository.ListByOwnerAsync(ownerUserId, ct);
        var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
        
        // Load translations for all epics
        var epicIds = epics.Select(e => e.Id).ToList();
        var translations = await _context.StoryEpicTranslations
            .Where(t => epicIds.Contains(t.StoryEpicId))
            .ToListAsync(ct);
        var translationsByEpic = translations.GroupBy(t => t.StoryEpicId)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        return epics.Select(e =>
        {
            // Compute flags for current user
            var isOwnedByCurrentUser = currentUserId.HasValue && e.OwnerUserId == currentUserId.Value;
            var isAssignedToCurrentUser = currentUserId.HasValue && 
                                          e.AssignedReviewerUserId.HasValue && 
                                          e.AssignedReviewerUserId.Value == currentUserId.Value;
            
            // Get name and description in requested locale
            var epicTranslations = translationsByEpic.GetValueOrDefault(e.Id, new List<Data.StoryEpicTranslation>());
            var translation = epicTranslations.FirstOrDefault(t => t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
            var name = translation?.Name ?? epicTranslations.FirstOrDefault()?.Name ?? e.Name;
            var description = translation?.Description ?? epicTranslations.FirstOrDefault()?.Description ?? e.Description;
            
            return new StoryEpicListItemDto
            {
                Id = e.Id,
                Name = name,
                Description = description,
                CoverImageUrl = e.CoverImageUrl,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                PublishedAtUtc = e.PublishedAtUtc,
                StoryCount = e.StoryNodes.Count,
                RegionCount = e.Regions.Count,
                AssignedReviewerUserId = e.AssignedReviewerUserId,
                IsAssignedToCurrentUser = isAssignedToCurrentUser,
                IsOwnedByCurrentUser = isOwnedByCurrentUser
            };
        }).ToList();
    }

    public async Task DeleteEpicAsync(Guid ownerUserId, string epicId, CancellationToken ct = default)
    {
        // Verify ownership
        var epic = await _repository.GetAsync(epicId, ct);
        if (epic == null)
        {
            throw new InvalidOperationException($"Epic '{epicId}' not found");
        }

        if (epic.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
        }

        await _repository.DeleteAsync(epicId, ct);
    }

    public async Task<int> CreateVersionFromPublishedAsync(Guid ownerUserId, string epicId, CancellationToken ct = default)
    {
        // Load published epic with all related data
        var publishedEpic = await _repository.GetFullAsync(epicId, ct);
        if (publishedEpic == null)
        {
            throw new InvalidOperationException($"Epic '{epicId}' not found");
        }

        // Verify ownership
        if (publishedEpic.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User does not own epic '{epicId}'");
        }

        // Check if epic is published
        if (publishedEpic.Status != "published")
        {
            throw new InvalidOperationException($"Epic '{epicId}' is not published (status: {publishedEpic.Status})");
        }

        // Check if draft already exists (status != "published")
        var existingEpic = await _repository.GetAsync(epicId, ct);
        if (existingEpic != null && existingEpic.Status != "published")
        {
            throw new InvalidOperationException("A draft already exists for this epic. Please edit or publish it first.");
        }

        // IMPORTANT: Epic-ul publicat rămâne neschimbat în market!
        // Nu actualizăm epic-ul publicat - doar actualizăm dacă există deja un draft (ceea ce nu e cazul aici)
        // Epic-ul publicat va rămâne cu status="published" și va fi vizibil în market.

        // Actualizăm epic-ul existent (care este publicat) la draft, dar păstrăm datele publicate
        // În realitate, epic-ul publicat rămâne în market pentru că query-ul din market verifică status="published"
        // Dar aici trebuie să actualizăm epic-ul pentru a crea draft-ul
        
        // Salvăm versiunea publicată înainte de a actualiza
        var publishedVersion = publishedEpic.Version;
        var publishedCoverImageUrl = publishedEpic.CoverImageUrl;
        var publishedPublishedAtUtc = publishedEpic.PublishedAtUtc;

        // IMPORTANT: Nu actualizăm status-ul epic-ului publicat!
        // Epic-ul publicat rămâne cu status="published" astfel încât să rămână vizibil în market.
        // Doar setăm BaseVersion pentru a ști de la ce versiune publicată am început editarea.
        publishedEpic.BaseVersion = publishedVersion; // Set base version to published version
        publishedEpic.LastDraftVersion = 0; // Reset draft version counter
        publishedEpic.UpdatedAt = DateTime.UtcNow;
        // NU schimbăm Status - rămâne "published"!
        // NU schimbăm Version - rămâne versiunea publicată!
        // NU schimbăm PublishedAtUtc - rămâne data publicării!
        // Nu ștergem sau copiem entitățile - ele rămân neschimbate pentru market

        await _context.SaveChangesAsync(ct);
        
        // Return the base version that was set
        return publishedEpic.BaseVersion;
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

