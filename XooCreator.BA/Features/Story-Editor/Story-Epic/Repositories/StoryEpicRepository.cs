using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public class StoryEpicRepository : IStoryEpicRepository
{
    private readonly XooDbContext _context;

    public StoryEpicRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<Data.StoryEpic?> GetAsync(string epicId, CancellationToken ct = default)
    {
        var id = (epicId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return null;

        return await _context.StoryEpics
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Data.StoryEpic?> GetFullAsync(string epicId, CancellationToken ct = default)
    {
        var id = (epicId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return null;

        return await _context.StoryEpics
            .Include(x => x.Regions)
            .Include(x => x.StoryNodes)
                .ThenInclude(sn => sn.Region)
            .Include(x => x.StoryNodes)
                .ThenInclude(sn => sn.StoryCraft)
            .Include(x => x.StoryNodes)
                .ThenInclude(sn => sn.StoryDefinition)
            .Include(x => x.UnlockRules)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Data.StoryEpic> CreateAsync(Guid ownerUserId, string epicId, string name, CancellationToken ct = default)
    {
        var id = (epicId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Epic ID cannot be empty", nameof(epicId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Epic name cannot be empty", nameof(name));
        }

        // Check if epic already exists
        var exists = await ExistsAsync(id, ct);
        if (exists)
        {
            throw new InvalidOperationException($"Epic with ID '{id}' already exists");
        }

        var epic = new Data.StoryEpic
        {
            Id = id,
            Name = name.Trim(),
            OwnerUserId = ownerUserId,
            Status = "draft",
            IsDefault = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.StoryEpics.Add(epic);
        await _context.SaveChangesAsync(ct);
        return epic;
    }

    public async Task SaveAsync(Data.StoryEpic epic, CancellationToken ct = default)
    {
        if (epic == null)
        {
            throw new ArgumentNullException(nameof(epic));
        }

        epic.UpdatedAt = DateTime.UtcNow;

        // Check if epic exists
        var existing = await _context.StoryEpics
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == epic.Id, ct);

        if (existing == null)
        {
            // New epic - add to context
            if (epic.CreatedAt == default)
            {
                epic.CreatedAt = DateTime.UtcNow;
            }
            _context.StoryEpics.Add(epic);
        }
        else
        {
            // Existing epic - update
            _context.StoryEpics.Update(epic);
        }

        // Handle related entities
        // Regions
        foreach (var region in epic.Regions)
        {
            region.UpdatedAt = DateTime.UtcNow;
            if (region.CreatedAt == default)
            {
                region.CreatedAt = DateTime.UtcNow;
            }
            _context.StoryEpicRegions.Update(region);
        }

        // Story nodes
        foreach (var storyNode in epic.StoryNodes)
        {
            storyNode.UpdatedAt = DateTime.UtcNow;
            if (storyNode.CreatedAt == default)
            {
                storyNode.CreatedAt = DateTime.UtcNow;
            }
            _context.StoryEpicStoryNodes.Update(storyNode);
        }

        // Unlock rules
        foreach (var rule in epic.UnlockRules)
        {
            rule.UpdatedAt = DateTime.UtcNow;
            if (rule.CreatedAt == default)
            {
                rule.CreatedAt = DateTime.UtcNow;
            }
            _context.StoryEpicUnlockRules.Update(rule);
        }

        await _context.SaveChangesAsync(ct);
    }

    public Task<List<Data.StoryEpic>> ListByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
    {
        return _context.StoryEpics
            .Include(x => x.Regions)
            .Include(x => x.StoryNodes)
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task DeleteAsync(string epicId, CancellationToken ct = default)
    {
        var id = (epicId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return;

        var epic = await _context.StoryEpics
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (epic != null)
        {
            // Cascade delete will handle related entities
            _context.StoryEpics.Remove(epic);
            await _context.SaveChangesAsync(ct);
        }
    }

    public Task<bool> ExistsAsync(string epicId, CancellationToken ct = default)
    {
        var id = (epicId ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(id)) return Task.FromResult(false);

        return _context.StoryEpics
            .AnyAsync(x => x.Id == id, ct);
    }
}

