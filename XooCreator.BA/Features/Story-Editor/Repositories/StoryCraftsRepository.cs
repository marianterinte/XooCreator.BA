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
