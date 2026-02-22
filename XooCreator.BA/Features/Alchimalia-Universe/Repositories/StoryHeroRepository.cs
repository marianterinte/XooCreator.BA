using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public class StoryHeroRepository : IStoryHeroRepository
{
    private readonly XooDbContext _context;

    public StoryHeroRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<StoryHero?> GetAsync(Guid storyHeroId, CancellationToken ct = default)
    {
        return await _context.StoryHeroes
            .FirstOrDefaultAsync(x => x.Id == storyHeroId, ct);
    }

    public async Task<StoryHero?> GetByHeroIdAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        return await _context.StoryHeroes
            .FirstOrDefaultAsync(x => x.HeroId == id, ct);
    }

    public async Task<StoryHero?> GetByHeroIdWithTranslationsAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        return await _context.StoryHeroes
            .AsNoTracking()
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.HeroId == id, ct);
    }

    public async Task<StoryHero?> GetWithTranslationsAsync(Guid storyHeroId, CancellationToken ct = default)
    {
        return await _context.StoryHeroes
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == storyHeroId, ct);
    }

    public async Task<StoryHero> CreateAsync(StoryHero storyHero, CancellationToken ct = default)
    {
        storyHero.CreatedAt = DateTime.UtcNow;
        storyHero.UpdatedAt = DateTime.UtcNow;
        _context.StoryHeroes.Add(storyHero);
        await _context.SaveChangesAsync(ct);
        return storyHero;
    }

    public async Task SaveAsync(StoryHero storyHero, CancellationToken ct = default)
    {
        storyHero.UpdatedAt = DateTime.UtcNow;
        
        var existing = await _context.StoryHeroes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == storyHero.Id, ct);
        
        if (existing == null)
        {
            if (storyHero.Id == Guid.Empty)
            {
                storyHero.Id = Guid.NewGuid();
            }
            storyHero.CreatedAt = DateTime.UtcNow;
            _context.StoryHeroes.Add(storyHero);
        }
        else
        {
            _context.StoryHeroes.Update(storyHero);
        }
        
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<StoryHero>> ListAsync(string? status = null, string? search = null, CancellationToken ct = default)
    {
        var query = _context.StoryHeroes
            .AsNoTracking()
            .Include(x => x.Translations)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => 
                x.HeroId.Contains(search) ||
                x.Translations.Any(t => t.Name.Contains(search)));
        }
        
        return await query
            .OrderBy(x => x.HeroId)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? status = null, CancellationToken ct = default)
    {
        var query = _context.StoryHeroes.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }
        
        return await query.CountAsync(ct);
    }

    public async Task DeleteAsync(Guid storyHeroId, CancellationToken ct = default)
    {
        var storyHero = await _context.StoryHeroes
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == storyHeroId, ct);
        
        if (storyHero != null)
        {
            _context.StoryHeroes.Remove(storyHero);
            await _context.SaveChangesAsync(ct);
        }
    }
}
