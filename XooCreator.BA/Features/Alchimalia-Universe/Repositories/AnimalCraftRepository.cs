using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public class AnimalCraftRepository : IAnimalCraftRepository
{
    private readonly XooDbContext _context;

    public AnimalCraftRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<AnimalCraft?> GetAsync(Guid animalId, CancellationToken ct = default)
    {
        return await _context.AnimalCrafts
            .Include(x => x.Region)
            .FirstOrDefaultAsync(x => x.Id == animalId, ct);
    }

    public async Task<AnimalCraft?> GetWithTranslationsAsync(Guid animalId, CancellationToken ct = default)
    {
        return await _context.AnimalCrafts
            .Include(x => x.Region)
            .Include(x => x.Translations)
            .Include(x => x.SupportedParts)
            .Include(x => x.HybridParts)
            .FirstOrDefaultAsync(x => x.Id == animalId, ct);
    }

    public async Task<AnimalCraft> CreateAsync(AnimalCraft animal, CancellationToken ct = default)
    {
        animal.CreatedAt = DateTime.UtcNow;
        animal.UpdatedAt = DateTime.UtcNow;
        _context.AnimalCrafts.Add(animal);
        await _context.SaveChangesAsync(ct);
        return animal;
    }

    public async Task SaveAsync(AnimalCraft animal, CancellationToken ct = default)
    {
        var existing = await _context.AnimalCrafts
            .Include(x => x.Translations)
            .Include(x => x.SupportedParts)
            .FirstOrDefaultAsync(x => x.Id == animal.Id, ct);

        if (existing == null)
        {
            _context.AnimalCrafts.Add(animal);
        }
        else
        {
            _context.AnimalCrafts.Update(animal);
        }

        animal.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid animalId, CancellationToken ct = default)
    {
        var animal = await _context.AnimalCrafts
            .Include(x => x.Translations)
            .Include(x => x.SupportedParts)
            .Include(x => x.HybridParts)
            .FirstOrDefaultAsync(x => x.Id == animalId, ct);

        if (animal != null)
        {
            _context.AnimalCraftTranslations.RemoveRange(animal.Translations);
            _context.AnimalCraftPartSupports.RemoveRange(animal.SupportedParts);
            _context.AnimalHybridCraftParts.RemoveRange(animal.HybridParts);
            _context.AnimalCrafts.Remove(animal);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<List<AnimalCraft>> ListAsync(string? status = null, Guid? regionId = null, bool? isHybrid = null, string? search = null, CancellationToken ct = default)
    {
        var query = _context.AnimalCrafts
            .Include(x => x.Region)
            .Include(x => x.Translations)
            .Include(x => x.SupportedParts)
            .Include(x => x.HybridParts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalized = status.Trim()
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .ToLowerInvariant();

            // Special virtual status used by the editor tab: return all non-published crafts
            // (draft/sent_for_approval/in_review/approved/changes_requested)
            if (normalized == "inprogress")
            {
                query = query.Where(x =>
                    x.Status != AlchimaliaUniverseStatus.Published.ToDb() &&
                    x.Status != AlchimaliaUniverseStatus.Archived.ToDb());
            }
            else
            {
                query = query.Where(x => x.Status == status);
            }
        }

        if (regionId.HasValue)
            query = query.Where(x => x.RegionId == regionId.Value);

        if (isHybrid.HasValue)
            query = query.Where(x => x.IsHybrid == isHybrid.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Label.Contains(search) || x.Translations.Any(t => t.Label.Contains(search)));

        return await query
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? status = null, Guid? regionId = null, CancellationToken ct = default)
    {
        var query = _context.AnimalCrafts.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalized = status.Trim()
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .ToLowerInvariant();

            if (normalized == "inprogress")
            {
                query = query.Where(x =>
                    x.Status != AlchimaliaUniverseStatus.Published.ToDb() &&
                    x.Status != AlchimaliaUniverseStatus.Archived.ToDb());
            }
            else
            {
                query = query.Where(x => x.Status == status);
            }
        }
        if (regionId.HasValue)
            query = query.Where(x => x.RegionId == regionId.Value);
        return await query.CountAsync(ct);
    }
}
