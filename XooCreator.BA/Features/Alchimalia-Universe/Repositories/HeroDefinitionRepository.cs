using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public class HeroDefinitionRepository : IHeroDefinitionRepository
{
    private readonly XooDbContext _context;

    public HeroDefinitionRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<HeroDefinition?> GetAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        return await _context.HeroDefinitions
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<HeroDefinition?> GetWithTranslationsAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        return await _context.HeroDefinitions
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<HeroDefinition> CreateAsync(HeroDefinition hero, CancellationToken ct = default)
    {
        hero.CreatedAt = DateTime.UtcNow;
        hero.UpdatedAt = DateTime.UtcNow;
        _context.HeroDefinitions.Add(hero);
        await _context.SaveChangesAsync(ct);
        return hero;
    }

    public async Task SaveAsync(HeroDefinition hero, CancellationToken ct = default)
    {
        hero.UpdatedAt = DateTime.UtcNow;
        
        var existing = await _context.HeroDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == hero.Id, ct);
        
        if (existing == null)
        {
            if (string.IsNullOrWhiteSpace(hero.Id))
            {
                throw new ArgumentException("HeroDefinition Id cannot be empty", nameof(hero));
            }
            _context.HeroDefinitions.Add(hero);
        }
        else
        {
            _context.HeroDefinitions.Update(hero);
        }
        
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<HeroDefinition>> ListAsync(string? status = null, string? type = null, string? search = null, CancellationToken ct = default)
    {
        var query = _context.HeroDefinitions
            .Include(x => x.Translations)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }
        
        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(x => x.Type == type);
        }
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => 
                x.Id.Contains(search) ||
                x.Translations.Any(t => t.Name.Contains(search)));
        }
        
        return await query
            .OrderBy(x => x.Id)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? status = null, string? type = null, CancellationToken ct = default)
    {
        var query = _context.HeroDefinitions.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }
        
        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(x => x.Type == type);
        }
        
        return await query.CountAsync(ct);
    }

    public async Task DeleteAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        var hero = await _context.HeroDefinitions
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        
        if (hero != null)
        {
            _context.HeroDefinitions.Remove(hero);
            await _context.SaveChangesAsync(ct);
        }
    }
}
