using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Repositories;

public class StoryCraftsRepository : IStoryCraftsRepository
{
    private readonly XooDbContext _context;

    public StoryCraftsRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<StoryCraft?> GetAsync(string storyId, CancellationToken ct = default)
    {
        var id = (storyId ?? string.Empty).Trim();
        return await _context.StoryCrafts
            .Include(x => x.Translations)
            .Include(x => x.Tiles)
                .ThenInclude(t => t.Translations)
            .Include(x => x.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Translations)
            .Include(x => x.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Tokens)
            .Include(x => x.Topics)
                .ThenInclude(t => t.StoryTopic)
            .Include(x => x.AgeGroups)
                .ThenInclude(ag => ag.StoryAgeGroup)
            .FirstOrDefaultAsync(x => x.StoryId == id, ct);
    }

    public async Task<StoryCraft?> GetWithLanguageAsync(string storyId, string languageCode, CancellationToken ct = default)
    {
        var id = (storyId ?? string.Empty).Trim();
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        
        return await _context.StoryCrafts
            .Include(x => x.Translations.Where(t => t.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.Translations.Where(tr => tr.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Translations.Where(at => at.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Tokens)
            .Include(x => x.Topics)
                .ThenInclude(t => t.StoryTopic)
            .Include(x => x.AgeGroups)
                .ThenInclude(ag => ag.StoryAgeGroup)
            .FirstOrDefaultAsync(x => x.StoryId == id, ct);
    }

    public async Task<StoryCraft> CreateAsync(Guid ownerUserId, string storyId, string status, CancellationToken ct = default)
    {
        var craft = new StoryCraft
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            StoryId = (storyId ?? string.Empty).Trim(),
            Status = string.IsNullOrWhiteSpace(status) ? "draft" : status,
            StoryType = StoryType.Indie, // Default to Indie (1) for new stories
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _context.StoryCrafts.Add(craft);
        await _context.SaveChangesAsync(ct);
        return craft;
    }

    public async Task SaveAsync(StoryCraft craft, CancellationToken ct = default)
    {
        craft.UpdatedAt = DateTime.UtcNow;
        
        // Check if craft is already tracked
        var existing = await _context.StoryCrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.StoryId == craft.StoryId, ct);
        
        if (existing == null)
        {
            // New craft - add to context
            if (craft.Id == Guid.Empty)
            {
                craft.Id = Guid.NewGuid();
            }
            _context.StoryCrafts.Add(craft);
            
            // Add related entities to context
            foreach (var translation in craft.Translations)
            {
                if (translation.Id == Guid.Empty)
                {
                    translation.Id = Guid.NewGuid();
                }
                if (translation.StoryCraftId == Guid.Empty)
                {
                    translation.StoryCraftId = craft.Id;
                }
                _context.StoryCraftTranslations.Add(translation);
            }
            
            foreach (var tile in craft.Tiles)
            {
                if (tile.Id == Guid.Empty)
                {
                    tile.Id = Guid.NewGuid();
                }
                if (tile.StoryCraftId == Guid.Empty)
                {
                    tile.StoryCraftId = craft.Id;
                }
                _context.StoryCraftTiles.Add(tile);
                
                // Add tile translations
                foreach (var tileTranslation in tile.Translations)
                {
                    if (tileTranslation.Id == Guid.Empty)
                    {
                        tileTranslation.Id = Guid.NewGuid();
                    }
                    if (tileTranslation.StoryCraftTileId == Guid.Empty)
                    {
                        tileTranslation.StoryCraftTileId = tile.Id;
                    }
                    _context.StoryCraftTileTranslations.Add(tileTranslation);
                }
                
                // Add answers
                foreach (var answer in tile.Answers)
                {
                    if (answer.Id == Guid.Empty)
                    {
                        answer.Id = Guid.NewGuid();
                    }
                    if (answer.StoryCraftTileId == Guid.Empty)
                    {
                        answer.StoryCraftTileId = tile.Id;
                    }
                    _context.StoryCraftAnswers.Add(answer);
                    
                    // Add answer translations
                    foreach (var answerTranslation in answer.Translations)
                    {
                        if (answerTranslation.Id == Guid.Empty)
                        {
                            answerTranslation.Id = Guid.NewGuid();
                        }
                        if (answerTranslation.StoryCraftAnswerId == Guid.Empty)
                        {
                            answerTranslation.StoryCraftAnswerId = answer.Id;
                        }
                        _context.StoryCraftAnswerTranslations.Add(answerTranslation);
                    }
                    
                    // Add tokens
                    foreach (var token in answer.Tokens)
                    {
                        if (token.Id == Guid.Empty)
                        {
                            token.Id = Guid.NewGuid();
                        }
                        if (token.StoryCraftAnswerId == Guid.Empty)
                        {
                            token.StoryCraftAnswerId = answer.Id;
                        }
                        _context.StoryCraftAnswerTokens.Add(token);
                    }
                }
            }
            
            // Add topics (many-to-many join entity - no Id property, uses composite key)
            foreach (var topic in craft.Topics)
            {
                if (topic.StoryCraftId == Guid.Empty)
                {
                    topic.StoryCraftId = craft.Id;
                }
                _context.StoryCraftTopics.Add(topic);
            }
            
            // Add age groups (many-to-many join entity - no Id property, uses composite key)
            foreach (var ageGroup in craft.AgeGroups)
            {
                if (ageGroup.StoryCraftId == Guid.Empty)
                {
                    ageGroup.StoryCraftId = craft.Id;
                }
                _context.StoryCraftAgeGroups.Add(ageGroup);
            }
        }
        else
        {
            // Existing craft - update
            craft.Id = existing.Id;
            _context.StoryCrafts.Update(craft);
        }
        
        await _context.SaveChangesAsync(ct);
    }

    public Task<List<StoryCraft>> ListByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
        => _context.StoryCrafts
            .Include(s => s.Translations)
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

    public Task<List<StoryCraft>> ListAllAsync(CancellationToken ct = default)
        => _context.StoryCrafts
            .Include(s=>s.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

    public async Task<int> CountDistinctStoryIdsByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
    {
        return await _context.StoryCrafts
            .Where(x => x.OwnerUserId == ownerUserId)
            .Select(x => x.StoryId)
            .Distinct()
            .CountAsync(ct);
    }

    public async Task DeleteAsync(string storyId, CancellationToken ct = default)
    {
        var id = (storyId ?? string.Empty).Trim();
        var craft = await _context.StoryCrafts.FirstOrDefaultAsync(x => x.StoryId == id, ct);
        if (craft != null)
        {
            _context.StoryCrafts.Remove(craft);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<List<string>> GetAvailableLanguagesAsync(string storyId, CancellationToken ct = default)
    {
        var id = (storyId ?? string.Empty).Trim();
        var languages = await _context.StoryCraftTranslations
            .Where(x => x.StoryCraft.StoryId == id)
            .Select(x => x.LanguageCode)
            .Distinct()
            .ToListAsync(ct);
        return languages;
    }
}
