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

    public async Task<EpicHeroCraft?> GetCraftAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return null;

        return await _context.EpicHeroCrafts
            .Include(x => x.Owner)
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<EpicHeroDefinition?> GetDefinitionAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return null;

        return await _context.EpicHeroDefinitions
            .Include(x => x.Owner)
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<EpicHeroCraft> CreateCraftAsync(Guid ownerUserId, string heroId, string name, CancellationToken ct = default)
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

        // Check if hero craft already exists
        var craftExists = await _context.EpicHeroCrafts.AnyAsync(x => x.Id == id, ct);
        if (craftExists)
        {
            throw new InvalidOperationException($"Hero craft with ID '{id}' already exists");
        }

        var heroCraft = new EpicHeroCraft
        {
            Id = id,
            Name = name.Trim(),
            OwnerUserId = ownerUserId,
            Status = "draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BaseVersion = 0,
            LastDraftVersion = 0
        };

        _context.EpicHeroCrafts.Add(heroCraft);
        await _context.SaveChangesAsync(ct);
        return heroCraft;
    }

    public async Task SaveCraftAsync(EpicHeroCraft heroCraft, CancellationToken ct = default)
    {
        if (heroCraft == null)
        {
            throw new ArgumentNullException(nameof(heroCraft));
        }

        heroCraft.UpdatedAt = DateTime.UtcNow;

        // Check if hero craft exists
        var existing = await _context.EpicHeroCrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == heroCraft.Id, ct);

        if (existing == null)
        {
            // New hero craft - add to context
            if (heroCraft.CreatedAt == default)
            {
                heroCraft.CreatedAt = DateTime.UtcNow;
            }
            _context.EpicHeroCrafts.Add(heroCraft);
        }
        else
        {
            // Update existing hero craft
            _context.EpicHeroCrafts.Update(heroCraft);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<EpicHeroCraft>> ListCraftsByOwnerAsync(Guid ownerUserId, string? status = null, CancellationToken ct = default)
    {
        var query = _context.EpicHeroCrafts
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

    public async Task<List<EpicHeroDefinition>> ListPublishedDefinitionsAsync(Guid? excludeOwnerId = null, CancellationToken ct = default)
    {
        var query = _context.EpicHeroDefinitions
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

    public async Task<List<EpicHeroCraft>> ListCraftsForReviewAsync(CancellationToken ct = default)
    {
        return await _context.EpicHeroCrafts
            .Where(x => x.Status == "sent_for_approval" || x.Status == "in_review")
            .Include(x => x.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task DeleteCraftAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return;

        var heroCraft = await _context.EpicHeroCrafts
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (heroCraft != null)
        {
            _context.EpicHeroCrafts.Remove(heroCraft);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> CraftExistsAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        return await _context.EpicHeroCrafts.AnyAsync(x => x.Id == id, ct);
    }

    public async Task<bool> DefinitionExistsAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        return await _context.EpicHeroDefinitions.AnyAsync(x => x.Id == id, ct);
    }

    public async Task<bool> IsUsedInEpicsAsync(string heroId, CancellationToken ct = default)
    {
        var id = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        // Check both Craft and Definition references
        var usedInCrafts = await _context.StoryEpicCraftHeroReferences
            .AnyAsync(x => x.HeroId == id, ct);
        var usedInDefinitions = await _context.StoryEpicHeroReferences
            .AnyAsync(x => x.HeroId == id, ct);

        return usedInCrafts || usedInDefinitions;
    }
}

