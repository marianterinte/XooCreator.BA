using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Features.StoryEditor.Services.Content;

public interface IStoryTranslationManager
{
    Task EnsureTranslationAsync(Guid ownerUserId, string storyId, string languageCode, string? title = null, CancellationToken ct = default);
    Task DeleteTranslationAsync(Guid ownerUserId, string storyId, string languageCode, CancellationToken ct = default);
}

public class StoryTranslationManager : IStoryTranslationManager
{
    private readonly XooDbContext _context;
    private readonly IStoryDraftManager _draftManager;

    public StoryTranslationManager(XooDbContext context, IStoryDraftManager draftManager)
    {
        _context = context;
        _draftManager = draftManager;
    }

    public async Task EnsureTranslationAsync(Guid ownerUserId, string storyId, string languageCode, string? title = null, CancellationToken ct = default)
    {
        // Ensure draft exists first (default to Indie if not specified)
        await _draftManager.EnsureDraftAsync(ownerUserId, storyId, Data.Enums.StoryType.Indie, ct);
        
        var craft = await _context.StoryCrafts
            .Include(c => c.Translations)
            .Include(c =>c.Tiles)
                .ThenInclude(t => t.Translations)
            .FirstOrDefaultAsync(c => c.StoryId == storyId, ct);
            
        if (craft == null) return;
        
        var lang = languageCode.ToLowerInvariant();
        var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode == lang);
        var isNewTranslation = translation == null;
        
        if (translation == null)
        {
            translation = new StoryCraftTranslation
            {
                Id = Guid.NewGuid(),
                StoryCraftId = craft.Id,
                LanguageCode = lang,
                Title = title ?? string.Empty,
                Summary = null
            };
            _context.StoryCraftTranslations.Add(translation);
            await _context.SaveChangesAsync(ct);
        }
        else if (!string.IsNullOrWhiteSpace(title))
        {
            // Update title if provided and translation already exists
            translation.Title = title;
            await _context.SaveChangesAsync(ct);
        }
        
        // If this is a new translation, clone tile translations from source language (including media)
        if (isNewTranslation)
        {
            // Find source language (prefer ro-ro, otherwise first available)
            var sourceLang = craft.Translations
                .FirstOrDefault(t => t.LanguageCode == "ro-ro")?.LanguageCode
                ?? craft.Translations.FirstOrDefault()?.LanguageCode;
            
            if (sourceLang != null && sourceLang != lang)
            {
                foreach (var tile in craft.Tiles)
                {
                    var sourceTileTranslation = tile.Translations.FirstOrDefault(t => t.LanguageCode == sourceLang);
                    if (sourceTileTranslation != null)
                    {
                        // Check if target translation already exists
                        var existingTargetTranslation = tile.Translations.FirstOrDefault(t => t.LanguageCode == lang);
                        if (existingTargetTranslation == null)
                        {
                            // Clone tile translation including audio/video
                            var newTileTranslation = new StoryCraftTileTranslation
                            {
                                Id = Guid.NewGuid(),
                                StoryCraftTileId = tile.Id,
                                LanguageCode = lang,
                                Caption = sourceTileTranslation.Caption,
                                Text = sourceTileTranslation.Text,
                                Question = sourceTileTranslation.Question,
                                // Clone media from source language
                                AudioUrl = sourceTileTranslation.AudioUrl,
                                VideoUrl = sourceTileTranslation.VideoUrl
                            };
                            _context.StoryCraftTileTranslations.Add(newTileTranslation);
                        }
                    }
                }
                await _context.SaveChangesAsync(ct);
            }
        }
    }

    public async Task DeleteTranslationAsync(Guid ownerUserId, string storyId, string languageCode, CancellationToken ct = default)
    {
        var craft = await _context.StoryCrafts
            .Include(c => c.Translations)
            .Include(c => c.Tiles)
                .ThenInclude(t => t.Translations)
            .Include(c => c.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Translations)
            .FirstOrDefaultAsync(c => c.StoryId == storyId, ct);
            
        if (craft == null || craft.OwnerUserId != ownerUserId) return;
        
        var lang = languageCode.ToLowerInvariant();
        
        // Remove translation
        var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode == lang);
        if (translation != null)
        {
            _context.StoryCraftTranslations.Remove(translation);
        }
        
        // Remove tile translations
        foreach (var tile in craft.Tiles)
        {
            var tileTranslation = tile.Translations.FirstOrDefault(t => t.LanguageCode == lang);
            if (tileTranslation != null)
            {
                _context.StoryCraftTileTranslations.Remove(tileTranslation);
            }
            
            // Remove answer translations
            foreach (var answer in tile.Answers)
            {
                var answerTranslation = answer.Translations.FirstOrDefault(t => t.LanguageCode == lang);
                if (answerTranslation != null)
                {
                    _context.StoryCraftAnswerTranslations.Remove(answerTranslation);
                }
            }
        }
        
        await _context.SaveChangesAsync(ct);
    }
}
