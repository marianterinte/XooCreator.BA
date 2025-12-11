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

        // EF Core will track the epic and its related entities automatically
        // since they were loaded via GetFullAsync with Include()
        // Just ensure timestamps are set properly
        
        foreach (var region in epic.Regions)
        {
            region.UpdatedAt = DateTime.UtcNow;
            if (region.CreatedAt == default)
            {
                region.CreatedAt = DateTime.UtcNow;
            }
            // Let EF Core determine if it's new (Added) or existing (Modified)
            // based on its tracking state
        }

        foreach (var storyNode in epic.StoryNodes)
        {
            storyNode.UpdatedAt = DateTime.UtcNow;
            if (storyNode.CreatedAt == default)
            {
                storyNode.CreatedAt = DateTime.UtcNow;
            }
        }

        foreach (var rule in epic.UnlockRules)
        {
            rule.UpdatedAt = DateTime.UtcNow;
            if (rule.CreatedAt == default)
            {
                rule.CreatedAt = DateTime.UtcNow;
            }
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

