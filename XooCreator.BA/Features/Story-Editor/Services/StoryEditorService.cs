using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.Stories.Services;
using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Features.StoryEditor.Services;

public class StoryEditorService : IStoryEditorService
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly XooDbContext _context;

    public StoryEditorService(IStoryCraftsRepository crafts, XooDbContext context)
    {
        _crafts = crafts;
        _context = context;
    }

    public async Task EnsureDraftAsync(Guid ownerUserId, string storyId, StoryType? storyType = null, CancellationToken ct = default)
    {
        var existing = await _crafts.GetAsync(storyId, ct);
        if (existing != null) return;
        
        var craft = await _crafts.CreateAsync(ownerUserId, storyId, StoryStatus.Draft.ToDb(), ct);
        
        // If storyType is provided, update it (default is already set to Indie in CreateAsync)
        if (storyType.HasValue && craft.StoryType != storyType.Value)
        {
            craft.StoryType = storyType.Value;
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task EnsureTranslationAsync(Guid ownerUserId, string storyId, string languageCode, string? title = null, CancellationToken ct = default)
    {
        // Ensure draft exists first (default to Indie if not specified)
        await EnsureDraftAsync(ownerUserId, storyId, StoryType.Indie, ct);
        
        var craft = await _crafts.GetAsync(storyId, ct);
        if (craft == null) return;
        
        var lang = languageCode.ToLowerInvariant();
        var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode == lang);
        
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
    }

    public async Task SaveDraftAsync(Guid ownerUserId, string storyId, string languageCode, EditableStoryDto dto, CancellationToken ct = default)
    {
        // Ensure draft exists (use StoryType from DTO if provided, otherwise default to Indie)
        var storyType = dto.StoryType > 0 ? (StoryType?)dto.StoryType : StoryType.Indie;
        await EnsureDraftAsync(ownerUserId, storyId, storyType, ct);
        
        var craft = await _crafts.GetAsync(storyId, ct);
        if (craft == null) throw new InvalidOperationException($"StoryCraft not found for storyId: {storyId}");
        
        // Verify ownership
        if (craft.OwnerUserId != ownerUserId)
        {
            throw new UnauthorizedAccessException($"User {ownerUserId} is not the owner of story {storyId}");
        }
        
        var lang = languageCode.ToLowerInvariant();
        
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
        
        // Update craft-level fields
        craft.CoverImageUrl = dto.CoverImageUrl;
        craft.StoryType = (StoryType)(dto.StoryType);
        craft.UpdatedAt = DateTime.UtcNow;
        
        // Update tiles
        await UpdateTilesAsync(craft, dto.Tiles ?? new(), lang, ct);
        
        await _context.SaveChangesAsync(ct);
    }

    private async Task UpdateTilesAsync(StoryCraft craft, List<EditableTileDto> tiles, string languageCode, CancellationToken ct)
    {
        var existingTiles = craft.Tiles.ToList();
        var tileDict = existingTiles.ToDictionary(t => t.TileId);
        
        // Process each tile from DTO
        for (int i = 0; i < tiles.Count; i++)
        {
            var tileDto = tiles[i];
            var tile = tileDict.GetValueOrDefault(tileDto.Id);
            
            if (tile == null)
            {
                // Create new tile
                tile = new StoryCraftTile
                {
                    Id = Guid.NewGuid(),
                    StoryCraftId = craft.Id,
                    TileId = tileDto.Id,
                    Type = tileDto.Type ?? "page",
                    SortOrder = i,
                    ImageUrl = tileDto.ImageUrl,
                    AudioUrl = tileDto.AudioUrl,
                    VideoUrl = tileDto.Type == "video" ? tileDto.ImageUrl : null, // Assuming video URL is in ImageUrl for video type
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.StoryCraftTiles.Add(tile);
                craft.Tiles.Add(tile);
            }
            else
            {
                // Update existing tile
                tile.Type = tileDto.Type ?? tile.Type;
                tile.SortOrder = i;
                tile.ImageUrl = tileDto.ImageUrl;
                tile.AudioUrl = tileDto.AudioUrl;
                if (tileDto.Type == "video")
                {
                    tile.VideoUrl = tileDto.ImageUrl;
                }
                tile.UpdatedAt = DateTime.UtcNow;
            }
            
            // Update or create tile translation
            var tileTranslation = tile.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);
            if (tileTranslation == null)
            {
                tileTranslation = new StoryCraftTileTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftTileId = tile.Id,
                    LanguageCode = languageCode,
                    Caption = tileDto.Caption,
                    Text = tileDto.Text,
                    Question = tileDto.Question
                };
                _context.StoryCraftTileTranslations.Add(tileTranslation);
            }
            else
            {
                tileTranslation.Caption = tileDto.Caption;
                tileTranslation.Text = tileDto.Text;
                tileTranslation.Question = tileDto.Question;
            }
            
            // Update answers
            await UpdateAnswersAsync(tile, tileDto.Answers ?? new(), languageCode, ct);
        }
        
        // Remove tiles that are no longer in DTO
        var dtoTileIds = new HashSet<string>(tiles.Select(t => t.Id));
        var tilesToRemove = existingTiles.Where(t => !dtoTileIds.Contains(t.TileId)).ToList();
        foreach (var tileToRemove in tilesToRemove)
        {
            _context.StoryCraftTiles.Remove(tileToRemove);
        }
    }

    private async Task UpdateAnswersAsync(StoryCraftTile tile, List<EditableAnswerDto> answers, string languageCode, CancellationToken ct)
    {
        var existingAnswers = tile.Answers.ToList();
        var answerDict = existingAnswers.ToDictionary(a => a.AnswerId);
        
        for (int i = 0; i < answers.Count; i++)
        {
            var answerDto = answers[i];
            var answer = answerDict.GetValueOrDefault(answerDto.Id);
            
            if (answer == null)
            {
                answer = new StoryCraftAnswer
                {
                    Id = Guid.NewGuid(),
                    StoryCraftTileId = tile.Id,
                    AnswerId = answerDto.Id,
                    SortOrder = i,
                    CreatedAt = DateTime.UtcNow
                };
                _context.StoryCraftAnswers.Add(answer);
                tile.Answers.Add(answer);
            }
            else
            {
                answer.SortOrder = i;
            }
            
            // Update or create answer translation
            var answerTranslation = answer.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);
            if (answerTranslation == null)
            {
                answerTranslation = new StoryCraftAnswerTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftAnswerId = answer.Id,
                    LanguageCode = languageCode,
                    Text = answerDto.Text ?? string.Empty
                };
                _context.StoryCraftAnswerTranslations.Add(answerTranslation);
            }
            else
            {
                answerTranslation.Text = answerDto.Text ?? string.Empty;
            }
            
            // Update tokens (non-translatable)
            var existingTokens = answer.Tokens.ToList();
            _context.StoryCraftAnswerTokens.RemoveRange(existingTokens);
            
            foreach (var tokenDto in answerDto.Tokens ?? new())
            {
                var token = new StoryCraftAnswerToken
                {
                    Id = Guid.NewGuid(),
                    StoryCraftAnswerId = answer.Id,
                    Type = tokenDto.Type ?? "Personality",
                    Value = tokenDto.Value ?? string.Empty,
                    Quantity = tokenDto.Quantity
                };
                _context.StoryCraftAnswerTokens.Add(token);
            }
        }
        
        // Remove answers that are no longer in DTO
        var dtoAnswerIds = new HashSet<string>(answers.Select(a => a.Id));
        var answersToRemove = existingAnswers.Where(a => !dtoAnswerIds.Contains(a.AnswerId)).ToList();
        foreach (var answerToRemove in answersToRemove)
        {
            _context.StoryCraftAnswers.Remove(answerToRemove);
        }
    }

    public async Task DeleteTranslationAsync(Guid ownerUserId, string storyId, string languageCode, CancellationToken ct = default)
    {
        var craft = await _crafts.GetAsync(storyId, ct);
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

    public async Task DeleteDraftAsync(Guid ownerUserId, string storyId, CancellationToken ct = default)
    {
        var craft = await _crafts.GetAsync(storyId, ct);
        if (craft != null && craft.OwnerUserId == ownerUserId)
        {
            await _crafts.DeleteAsync(storyId, ct);
        }
    }
}
