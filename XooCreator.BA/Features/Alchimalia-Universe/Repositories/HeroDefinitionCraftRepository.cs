using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public class HeroDefinitionCraftRepository : IHeroDefinitionCraftRepository
{
    private readonly XooDbContext _context;

    public HeroDefinitionCraftRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<HeroDefinitionCraft?> GetAsync(string heroId, CancellationToken ct = default)
    {
        return await _context.HeroDefinitionCrafts
            .FirstOrDefaultAsync(x => x.Id == heroId, ct);
    }

    public async Task<HeroDefinitionCraft?> GetWithTranslationsAsync(string heroId, CancellationToken ct = default)
    {
        return await _context.HeroDefinitionCrafts
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == heroId, ct);
    }

    public async Task<HeroDefinitionCraft> CreateAsync(HeroDefinitionCraft hero, CancellationToken ct = default)
    {
        hero.CreatedAt = DateTime.UtcNow;
        hero.UpdatedAt = DateTime.UtcNow;
        _context.HeroDefinitionCrafts.Add(hero);
        await _context.SaveChangesAsync(ct);
        return hero;
    }

    public async Task SaveAsync(HeroDefinitionCraft hero, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(hero.Id))
            throw new ArgumentException("HeroDefinitionCraft Id cannot be empty", nameof(hero));

        // Always rely on tracked graph when possible.
        // Update() on a detached graph can incorrectly mark nested entities as Modified,
        // which can cause DbUpdateConcurrencyException when inserting new translations.
        var tracked = _context.ChangeTracker.Entries<HeroDefinitionCraft>()
            .FirstOrDefault(x => x.Entity.Id == hero.Id);

        if (tracked == null)
        {
            _context.HeroDefinitionCrafts.Update(hero);
        }

        hero.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(string heroId, CancellationToken ct = default)
    {
        var hero = await _context.HeroDefinitionCrafts
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == heroId, ct);

        if (hero != null)
        {
            _context.HeroDefinitionCraftTranslations.RemoveRange(hero.Translations);
            _context.HeroDefinitionCrafts.Remove(hero);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<List<HeroDefinitionCraft>> ListAsync(string? status = null, string? type = null, string? search = null, CancellationToken ct = default)
    {
        // Type parameter kept for backward compatibility but no longer used
        var query = _context.HeroDefinitionCrafts
            .AsNoTracking()
            .Include(x => x.Translations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Id.Contains(search) || x.Translations.Any(t => t.Name.Contains(search)));

        return await query
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? status = null, string? type = null, CancellationToken ct = default)
    {
        var query = _context.HeroDefinitionCrafts.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status == status);
        // Type parameter kept for backward compatibility but no longer used
        return await query.CountAsync(ct);
    }
}
