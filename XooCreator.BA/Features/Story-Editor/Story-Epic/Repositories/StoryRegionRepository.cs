using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public class StoryRegionRepository : IStoryRegionRepository
{
    private readonly XooDbContext _context;

    public StoryRegionRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<StoryRegion?> GetAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return null;

        return await _context.StoryRegions
            .Include(x => x.Owner)
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<StoryRegion> CreateAsync(Guid ownerUserId, string regionId, string name, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Region ID cannot be empty", nameof(regionId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Region name cannot be empty", nameof(name));
        }

        // Check if region already exists for this owner
        var exists = await _context.StoryRegions
            .AnyAsync(x => x.Id == id && x.OwnerUserId == ownerUserId, ct);
        if (exists)
        {
            throw new InvalidOperationException($"Region with ID '{id}' already exists for this owner");
        }

        var region = new StoryRegion
        {
            Id = id,
            Name = name.Trim(),
            OwnerUserId = ownerUserId,
            Status = "draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.StoryRegions.Add(region);
        await _context.SaveChangesAsync(ct);
        return region;
    }

    public async Task SaveAsync(StoryRegion region, CancellationToken ct = default)
    {
        if (region == null)
        {
            throw new ArgumentNullException(nameof(region));
        }

        region.UpdatedAt = DateTime.UtcNow;

        // Check if region exists
        var existing = await _context.StoryRegions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == region.Id && x.OwnerUserId == region.OwnerUserId, ct);

        if (existing == null)
        {
            // New region - add to context
            if (region.CreatedAt == default)
            {
                region.CreatedAt = DateTime.UtcNow;
            }
            _context.StoryRegions.Add(region);
        }
        else
        {
            // Update existing region
            _context.StoryRegions.Update(region);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<StoryRegion>> ListByOwnerAsync(Guid ownerUserId, string? status = null, CancellationToken ct = default)
    {
        var query = _context.StoryRegions
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

    public async Task<List<StoryRegion>> ListPublishedAsync(Guid? excludeOwnerId = null, CancellationToken ct = default)
    {
        var query = _context.StoryRegions
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

    public async Task<List<StoryRegion>> ListForReviewAsync(CancellationToken ct = default)
    {
        return await _context.StoryRegions
            .Where(x => x.Status == "sent_for_approval" || x.Status == "in_review")
            .Include(x => x.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task DeleteAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return;

        var region = await _context.StoryRegions
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (region != null)
        {
            _context.StoryRegions.Remove(region);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        return await _context.StoryRegions
            .AnyAsync(x => x.Id == id, ct);
    }

    public async Task<bool> IsUsedInEpicsAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        return await _context.StoryEpicRegionReferences
            .AnyAsync(x => x.RegionId == id, ct);
    }
}

