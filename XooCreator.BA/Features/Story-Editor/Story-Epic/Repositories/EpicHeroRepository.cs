using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public class EpicHeroRepository : IEpicHeroRepository
{
    private readonly XooDbContext _context;

    public EpicHeroRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<EpicHero?> GetAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return null;

        return await _context.EpicHeroes
            .Include(x => x.Owner)
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<EpicHero> CreateAsync(Guid ownerUserId, string heroId, string name, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Hero ID cannot be empty", nameof(heroId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Hero name cannot be empty", nameof(name));
        }

        // Check if hero already exists for this owner
        var exists = await _context.EpicHeroes
            .AnyAsync(x => x.Id == id && x.OwnerUserId == ownerUserId, ct);
        if (exists)
        {
            throw new InvalidOperationException($"Hero with ID '{id}' already exists for this owner");
        }

        var hero = new EpicHero
        {
            Id = id,
            Name = name.Trim(),
            OwnerUserId = ownerUserId,
            Status = "draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.EpicHeroes.Add(hero);
        await _context.SaveChangesAsync(ct);
        return hero;
    }

    public async Task SaveAsync(EpicHero hero, CancellationToken ct = default)
    {
        if (hero == null)
        {
            throw new ArgumentNullException(nameof(hero));
        }

        hero.UpdatedAt = DateTime.UtcNow;

        // Check if hero exists
        var existing = await _context.EpicHeroes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == hero.Id && x.OwnerUserId == hero.OwnerUserId, ct);

        if (existing == null)
        {
            // New hero - add to context
            if (hero.CreatedAt == default)
            {
                hero.CreatedAt = DateTime.UtcNow;
            }
            _context.EpicHeroes.Add(hero);
        }
        else
        {
            // Update existing hero
            _context.EpicHeroes.Update(hero);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<EpicHero>> ListByOwnerAsync(Guid ownerUserId, string? status = null, CancellationToken ct = default)
    {
        var query = _context.EpicHeroes
            .Where(x => x.OwnerUserId == ownerUserId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }

        return await query
            .Include(x => x.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<EpicHero>> ListPublishedAsync(Guid? excludeOwnerId = null, CancellationToken ct = default)
    {
        var query = _context.EpicHeroes
            .Where(x => x.Status == "published")
            .AsQueryable();

        if (excludeOwnerId.HasValue)
        {
            var ownerId = excludeOwnerId.Value;
            query = query.Where(x => x.OwnerUserId != ownerId);
        }

        return await query
            .Include(x => x.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<EpicHero>> ListForReviewAsync(CancellationToken ct = default)
    {
        return await _context.EpicHeroes
            .Where(x => x.Status == "sent_for_approval" || x.Status == "in_review")
            .Include(x => x.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task DeleteAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return;

        var hero = await _context.EpicHeroes
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (hero != null)
        {
            _context.EpicHeroes.Remove(hero);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        return await _context.EpicHeroes
            .AnyAsync(x => x.Id == id, ct);
    }

    public async Task<bool> IsUsedInEpicsAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        return await _context.StoryEpicHeroReferences
            .AnyAsync(x => x.HeroId == id, ct);
    }
}

