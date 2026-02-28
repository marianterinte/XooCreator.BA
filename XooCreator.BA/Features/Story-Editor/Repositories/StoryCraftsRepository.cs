using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Repositories;

public class StoryCraftsRepository : IStoryCraftsRepository
{
    private readonly XooDbContext _context;
    private readonly IMemoryCache? _cache;

    public StoryCraftsRepository(XooDbContext context, IMemoryCache? cache = null)
    {
        _context = context;
        _cache = cache;
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
            .Include(x => x.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.Translations)
            .Include(x => x.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.OutgoingEdges)
                            .ThenInclude(e => e.Translations)
            .Include(x => x.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.OutgoingEdges)
                            .ThenInclude(e => e.Tokens)
            .Include(x => x.Topics)
                .ThenInclude(t => t.StoryTopic)
            .Include(x => x.AgeGroups)
                .ThenInclude(ag => ag.StoryAgeGroup)
            .Include(x => x.CoAuthors).ThenInclude(c => c.User)
            .Include(x => x.UnlockedHeroes)
            .Include(x => x.DialogParticipants)
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
            .Include(x => x.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.Translations.Where(nt => nt.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.OutgoingEdges)
                            .ThenInclude(e => e.Translations.Where(et => et.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.OutgoingEdges)
                            .ThenInclude(e => e.Tokens)
            .Include(x => x.Topics)
                .ThenInclude(t => t.StoryTopic)
            .Include(x => x.AgeGroups)
                .ThenInclude(ag => ag.StoryAgeGroup)
            .Include(x => x.CoAuthors).ThenInclude(c => c.User)
            .Include(x => x.UnlockedHeroes)
            .Include(x => x.DialogParticipants)
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
                // Dialog tile must reference the craft and tile (required for FK when copying from definition / new version).
                if (tile.DialogTile != null)
                {
                    if (tile.DialogTile.StoryCraftId == Guid.Empty)
                        tile.DialogTile.StoryCraftId = craft.Id;
                    if (tile.DialogTile.StoryCraftTileId == Guid.Empty)
                        tile.DialogTile.StoryCraftTileId = tile.Id;
                    if (tile.DialogTile.Id == Guid.Empty)
                        tile.DialogTile.Id = Guid.NewGuid();
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
            
            // Dialog participants must reference the craft (FK); set when saving new craft from copy/definition.
            foreach (var participant in craft.DialogParticipants)
            {
                if (participant.Id == Guid.Empty)
                    participant.Id = Guid.NewGuid();
                if (participant.StoryCraftId == Guid.Empty)
                    participant.StoryCraftId = craft.Id;
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
            .AsNoTracking()
            .Include(s => s.Translations)
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

    public async Task<List<StoryCraft>> ListByAssignedReviewerAsync(Guid reviewerUserId, CancellationToken ct = default)
    {
        return await _context.StoryCrafts
            .AsNoTracking()
            .Include(s => s.Translations)
            .Where(x => x.AssignedReviewerUserId == reviewerUserId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<StoryCraft>> ListClaimableAsync(CancellationToken ct = default)
    {
        return await _context.StoryCrafts
            .AsNoTracking()
            .Include(s => s.Translations)
            .Where(x => x.AssignedReviewerUserId == null
                && (x.Status == "sent_for_approval" || x.Status == "review" || x.Status == "submitted"))
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task<(List<StoryCraft> Items, int TotalCount)> ListByOwnerPagedAsync(Guid ownerUserId, int skip, int take, CancellationToken ct = default)
    {
        var query = _context.StoryCrafts
            .AsNoTracking()
            .Include(s => s.Translations)
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderByDescending(x => x.UpdatedAt);
        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip(skip).Take(take).ToListAsync(ct);
        return (items, totalCount);
    }

    public Task<List<StoryCraft>> ListAllAsync(CancellationToken ct = default)
        => _context.StoryCrafts
            .AsNoTracking()
            .Include(s => s.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

    public async Task<(List<StoryCraft> Items, int TotalCount)> ListAllPagedAsync(int skip, int take, CancellationToken ct = default)
    {
        var query = _context.StoryCrafts
            .AsNoTracking()
            .Include(s => s.Translations)
            .OrderByDescending(x => x.UpdatedAt);
        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip(skip).Take(take).ToListAsync(ct);
        return (items, totalCount);
    }

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
        if (string.IsNullOrEmpty(id)) return;

        var craft = await _context.StoryCrafts
            .Include(c => c.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(c => c.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Translations)
            .Include(c => c.Tiles).ThenInclude(t => t.DialogTile!).ThenInclude(dt => dt.Nodes).ThenInclude(n => n.OutgoingEdges).ThenInclude(e => e.Tokens)
            .Include(c => c.Topics)
            .Include(c => c.AgeGroups)
            .Include(c => c.UnlockedHeroes)
            .Include(c => c.DialogParticipants)
            .FirstOrDefaultAsync(x => x.StoryId == id, ct);

        if (craft == null)
        {
            return;
        }

        _context.StoryCrafts.Remove(craft);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<string>> GetAvailableLanguagesAsync(string storyId, CancellationToken ct = default)
    {
        var id = (storyId ?? string.Empty).Trim();
        var cacheKey = $"story_languages_{id}";

        if (_cache != null && _cache.TryGetValue(cacheKey, out List<string>? cached))
            return cached ?? new List<string>();

        // First try to get languages from craft (draft)
        var languages = await _context.StoryCraftTranslations
            .AsNoTracking()
            .Where(x => x.StoryCraft.StoryId == id)
            .Select(x => x.LanguageCode)
            .Distinct()
            .ToListAsync(ct);
        
        // If no craft exists (e.g., published story with deleted draft), 
        // get languages from definition (published)
        if (languages.Count == 0)
        {
            languages = await _context.StoryDefinitionTranslations
                .AsNoTracking()
                .Where(x => x.StoryDefinition.StoryId == id)
                .Select(x => x.LanguageCode)
                .Distinct()
                .ToListAsync(ct);
        }

        if (_cache != null)
        {
            _cache.Set(cacheKey, languages, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }
        
        return languages;
    }
}
