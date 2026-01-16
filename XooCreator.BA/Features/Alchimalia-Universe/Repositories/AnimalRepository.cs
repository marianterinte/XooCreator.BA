using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private readonly XooDbContext _context;

    public AnimalRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<Animal?> GetAsync(Guid animalId, CancellationToken ct = default)
    {
        return await _context.Animals
            .FirstOrDefaultAsync(x => x.Id == animalId, ct);
    }

    public async Task<Animal?> GetWithTranslationsAsync(Guid animalId, CancellationToken ct = default)
    {
        return await _context.Animals
            .Include(x => x.Translations)
            .Include(x => x.SupportedParts)
            .Include(x => x.Region)
            .FirstOrDefaultAsync(x => x.Id == animalId, ct);
    }

    public async Task<Animal> CreateAsync(Animal animal, CancellationToken ct = default)
    {
        animal.CreatedAt = DateTime.UtcNow;
        animal.UpdatedAt = DateTime.UtcNow;
        _context.Animals.Add(animal);
        await _context.SaveChangesAsync(ct);
        return animal;
    }

    public async Task SaveAsync(Animal animal, CancellationToken ct = default)
    {
        animal.UpdatedAt = DateTime.UtcNow;
        
        var existing = await _context.Animals
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == animal.Id, ct);
        
        if (existing == null)
        {
            if (animal.Id == Guid.Empty)
            {
                animal.Id = Guid.NewGuid();
            }
            animal.CreatedAt = DateTime.UtcNow;
            _context.Animals.Add(animal);
        }
        else
        {
            _context.Animals.Update(animal);
        }
        
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<Animal>> ListAsync(string? status = null, Guid? regionId = null, bool? isHybrid = null, string? search = null, CancellationToken ct = default)
    {
        var query = _context.Animals
            .Include(x => x.Translations)
            .Include(x => x.Region)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }
        
        if (regionId.HasValue)
        {
            query = query.Where(x => x.RegionId == regionId.Value);
        }
        
        if (isHybrid.HasValue)
        {
            query = query.Where(x => x.IsHybrid == isHybrid.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => 
                x.Label.Contains(search) ||
                x.Translations.Any(t => t.Label.Contains(search)));
        }
        
        return await query
            .OrderBy(x => x.Label)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? status = null, Guid? regionId = null, bool? isHybrid = null, CancellationToken ct = default)
    {
        var query = _context.Animals.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }
        
        if (regionId.HasValue)
        {
            query = query.Where(x => x.RegionId == regionId.Value);
        }
        
        if (isHybrid.HasValue)
        {
            query = query.Where(x => x.IsHybrid == isHybrid.Value);
        }
        
        return await query.CountAsync(ct);
    }

    public async Task DeleteAsync(Guid animalId, CancellationToken ct = default)
    {
        var animal = await _context.Animals
            .Include(x => x.Translations)
            .Include(x => x.SupportedParts)
            .FirstOrDefaultAsync(x => x.Id == animalId, ct);
        
        if (animal != null)
        {
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync(ct);
        }
    }
}
