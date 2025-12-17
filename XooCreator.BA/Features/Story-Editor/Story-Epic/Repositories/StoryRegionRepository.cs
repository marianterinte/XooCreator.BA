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

    public async Task<StoryRegionCraft?> GetCraftAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return null;

        return await _context.StoryRegionCrafts
            .Include(x => x.Owner)
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<StoryRegionDefinition?> GetDefinitionAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return null;

        return await _context.StoryRegionDefinitions
            .Include(x => x.Owner)
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<StoryRegionCraft> CreateCraftAsync(Guid ownerUserId, string regionId, string name, CancellationToken ct = default)
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

        // Check if region craft already exists
        var craftExists = await _context.StoryRegionCrafts.AnyAsync(x => x.Id == id, ct);
        if (craftExists)
        {
            throw new InvalidOperationException($"Region craft with ID '{id}' already exists");
        }

        var regionCraft = new StoryRegionCraft
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

        _context.StoryRegionCrafts.Add(regionCraft);
        await _context.SaveChangesAsync(ct);
        return regionCraft;
    }

    public async Task SaveCraftAsync(StoryRegionCraft regionCraft, CancellationToken ct = default)
    {
        if (regionCraft == null)
        {
            throw new ArgumentNullException(nameof(regionCraft));
        }

        regionCraft.UpdatedAt = DateTime.UtcNow;

        // Check if region craft exists
        var existing = await _context.StoryRegionCrafts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == regionCraft.Id, ct);

        if (existing == null)
        {
            // New region craft - add to context
            if (regionCraft.CreatedAt == default)
            {
                regionCraft.CreatedAt = DateTime.UtcNow;
            }
            _context.StoryRegionCrafts.Add(regionCraft);
        }
        else
        {
            // Update existing region craft
            _context.StoryRegionCrafts.Update(regionCraft);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<StoryRegionCraft>> ListCraftsByOwnerAsync(Guid ownerUserId, string? status = null, CancellationToken ct = default)
    {
        var query = _context.StoryRegionCrafts
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

    public async Task<List<StoryRegionDefinition>> ListPublishedDefinitionsAsync(Guid? excludeOwnerId = null, CancellationToken ct = default)
    {
        var query = _context.StoryRegionDefinitions
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

    public async Task<List<StoryRegionCraft>> ListCraftsForReviewAsync(CancellationToken ct = default)
    {
        return await _context.StoryRegionCrafts
            .Where(x => x.Status == "sent_for_approval" || x.Status == "in_review")
            .Include(x => x.Translations)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task DeleteCraftAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return;

        var regionCraft = await _context.StoryRegionCrafts
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (regionCraft != null)
        {
            _context.StoryRegionCrafts.Remove(regionCraft);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> CraftExistsAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        return await _context.StoryRegionCrafts.AnyAsync(x => x.Id == id, ct);
    }

    public async Task<bool> DefinitionExistsAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        return await _context.StoryRegionDefinitions.AnyAsync(x => x.Id == id, ct);
    }

    public async Task<bool> IsUsedInEpicsAsync(string regionId, CancellationToken ct = default)
    {
        var id = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return false;

        // Check both Craft and Definition references
        var usedInCrafts = await _context.StoryEpicCraftRegions
            .AnyAsync(x => x.RegionId == id, ct);
        var usedInDefinitions = await _context.StoryEpicDefinitionRegions
            .AnyAsync(x => x.RegionId == id, ct);

        return usedInCrafts || usedInDefinitions;
    }
}

