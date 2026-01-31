using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services.Content;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Refactored StoryEditorService - now acts as a Facade pattern,
/// delegating to specialized services for each responsibility.
/// </summary>
public class StoryEditorService : IStoryEditorService
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly XooDbContext _context;
    private readonly IStoryDraftManager _draftManager;
    private readonly IStoryTranslationManager _translationManager;
    private readonly IStoryOwnershipService _ownershipService;
    private readonly IStoryTileUpdater _tileUpdater;
    private readonly IStoryPublishChangeLogService _changeLogService;

    public StoryEditorService(
        IStoryCraftsRepository crafts,
        XooDbContext context,
        IStoryDraftManager draftManager,
        IStoryTranslationManager translationManager,
        IStoryOwnershipService ownershipService,
        IStoryTileUpdater tileUpdater,
        IStoryPublishChangeLogService changeLogService)
    {
        _crafts = crafts;
        _context = context;
        _draftManager = draftManager;
        _translationManager = translationManager;
        _ownershipService = ownershipService;
        _tileUpdater = tileUpdater;
        _changeLogService = changeLogService;
    }

    public async Task EnsureDraftAsync(Guid ownerUserId, string storyId, StoryType? storyType = null, CancellationToken ct = default)
    {
        await _draftManager.EnsureDraftAsync(ownerUserId, storyId, storyType, ct);
    }

    public async Task EnsureTranslationAsync(Guid ownerUserId, string storyId, string languageCode, string? title = null, CancellationToken ct = default)
    {
        await _translationManager.EnsureTranslationAsync(ownerUserId, storyId, languageCode, title, ct);
    }

    public async Task SaveDraftAsync(Guid ownerUserId, string storyId, string languageCode, EditableStoryDto dto, bool bypassOwnershipCheck = false, CancellationToken ct = default)
    {
        // Ensure draft exists (use StoryType from DTO if provided, otherwise default to Indie)
        var storyType = dto.StoryType > 0 ? (StoryType?)dto.StoryType : StoryType.Indie;
        await _draftManager.EnsureDraftAsync(ownerUserId, storyId, storyType, ct);
        
        var craft = await _crafts.GetAsync(storyId, ct);
        if (craft == null) throw new InvalidOperationException($"StoryCraft not found for storyId: {storyId}");
        
        // Verify ownership
        _ownershipService.VerifyOwnership(craft, ownerUserId, bypassOwnershipCheck);
        
        var lang = languageCode.ToLowerInvariant();
        var snapshotBeforeChanges = _changeLogService.CaptureSnapshot(craft, lang);
        
        // Update or create translation
        var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode == lang);
        if (translation == null)
        {
            translation = new StoryCraftTranslation
            {
                Id = Guid.NewGuid(),
                StoryCraftId = craft.Id,
                LanguageCode = lang,
                Title = dto.Title ?? string.Empty,
                Summary = dto.Summary
            };
            _context.StoryCraftTranslations.Add(translation);
        }
        else
        {
            translation.Title = dto.Title ?? string.Empty;
            translation.Summary = dto.Summary;
        }
        
        craft.CoverImageUrl = ExtractFileName(dto.CoverImageUrl);
        craft.StoryTopic = dto.StoryTopic; // Keep for backward compatibility
        craft.AuthorName = dto.AuthorName; // Save author name (for "Other" option)
        craft.ClassicAuthorId = dto.ClassicAuthorId; // Save classic author ID if selected
        craft.StoryType = (StoryType)(dto.StoryType);
        craft.IsEvaluative = dto.IsEvaluative;
        craft.IsPartOfEpic = dto.IsPartOfEpic;
        craft.PriceInCredits = dto.PriceInCredits;
        craft.UpdatedAt = DateTime.UtcNow;
        
        // Update topics (many-to-many)
        await UpdateTopicsAsync(craft, dto.TopicIds ?? new(), ct);
        
        // Update age groups (many-to-many)
        await UpdateAgeGroupsAsync(craft, dto.AgeGroupIds ?? new(), ct);
        
        // Update unlocked heroes (many-to-many)
        await UpdateUnlockedHeroesAsync(craft, dto.UnlockedStoryHeroes ?? new(), ct);
        
        // Update tiles (delegated to TileUpdater)
        await _tileUpdater.UpdateTilesAsync(craft, dto.Tiles ?? new(), lang, ct);
        
        await _context.SaveChangesAsync(ct);
        await _changeLogService.AppendChangesAsync(craft, snapshotBeforeChanges, lang, ownerUserId, ct);
    }

    private async Task UpdateTopicsAsync(StoryCraft craft, List<string> topicIds, CancellationToken ct)
    {
        // Remove existing topics
        var existingTopics = _context.StoryCraftTopics
            .Where(t => t.StoryCraftId == craft.Id)
            .ToList();
        _context.StoryCraftTopics.RemoveRange(existingTopics);

        if (topicIds == null || topicIds.Count == 0)
        {
            return;
        }

        // Get topic entities by TopicId
        var topics = await _context.StoryTopics
            .Where(t => topicIds.Contains(t.TopicId))
            .ToListAsync(ct);

        // Add new topics
        foreach (var topic in topics)
        {
            _context.StoryCraftTopics.Add(new StoryCraftTopic
            {
                StoryCraftId = craft.Id,
                StoryTopicId = topic.Id,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task UpdateAgeGroupsAsync(StoryCraft craft, List<string> ageGroupIds, CancellationToken ct)
    {
        // Remove existing age groups
        var existingAgeGroups = _context.StoryCraftAgeGroups
            .Where(ag => ag.StoryCraftId == craft.Id)
            .ToList();
        _context.StoryCraftAgeGroups.RemoveRange(existingAgeGroups);

        if (ageGroupIds == null || ageGroupIds.Count == 0)
        {
            return;
        }

        // Get age group entities by AgeGroupId
        var ageGroups = await _context.StoryAgeGroups
            .Where(ag => ageGroupIds.Contains(ag.AgeGroupId))
            .ToListAsync(ct);

        // Add new age groups
        foreach (var ageGroup in ageGroups)
        {
            _context.StoryCraftAgeGroups.Add(new StoryCraftAgeGroup
            {
                StoryCraftId = craft.Id,
                StoryAgeGroupId = ageGroup.Id,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private async Task UpdateUnlockedHeroesAsync(StoryCraft craft, List<string> heroIds, CancellationToken ct)
    {
        // Remove existing unlocked heroes
        var existingHeroes = _context.StoryCraftUnlockedHeroes
            .Where(h => h.StoryCraftId == craft.Id)
            .ToList();
        _context.StoryCraftUnlockedHeroes.RemoveRange(existingHeroes);

        if (heroIds == null || heroIds.Count == 0)
        {
            return;
        }

        // Add new unlocked heroes (stored as string IDs, no separate entity needed)
        foreach (var heroId in heroIds)
        {
            if (string.IsNullOrWhiteSpace(heroId))
                continue;

            _context.StoryCraftUnlockedHeroes.Add(new StoryCraftUnlockedHero
            {
                StoryCraftId = craft.Id,
                HeroId = heroId.Trim(),
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    public async Task DeleteTranslationAsync(Guid ownerUserId, string storyId, string languageCode, CancellationToken ct = default)
    {
        await _translationManager.DeleteTranslationAsync(ownerUserId, storyId, languageCode, ct);
    }

    public async Task DeleteDraftAsync(Guid requestingUserId, string storyId, bool allowAdminOverride = false, CancellationToken ct = default)
    {
        var craft = await _crafts.GetAsync(storyId, ct);
        if (craft == null) return;
        if (!allowAdminOverride && craft.OwnerUserId != requestingUserId) return;
        await _crafts.DeleteAsync(storyId, ct);
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
