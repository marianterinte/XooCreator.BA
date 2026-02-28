using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public class StoryLikesRepository : IStoryLikesRepository
{
    private readonly XooDbContext _context;

    public StoryLikesRepository(XooDbContext context)
    {
        _context = context;
    }



    public async Task<bool> ToggleLikeAsync(Guid userId, string storyId, CancellationToken ct = default)
    {
        // Check if already liked
        var existing = await _context.StoryLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && 
                EF.Functions.ILike(l.StoryId, storyId), ct);

        if (existing != null)
        {
            // Unlike - remove the like
            _context.StoryLikes.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return false; // Return false to indicate not liked anymore
        }

        // Like - add new like
        var like = new StoryLike
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StoryId = storyId,
            LikedAt = DateTime.UtcNow
        };

        _context.StoryLikes.Add(like);
        await _context.SaveChangesAsync(ct);
        return true; // Return true to indicate liked
    }

    public async Task<bool> IsLikedAsync(Guid userId, string storyId)
    {
        return await _context.StoryLikes
            .AsNoTracking()
            .AnyAsync(l => l.UserId == userId && 
                EF.Functions.ILike(l.StoryId, storyId));
    }

    public Task<int> GetStoryLikesCountAsync(string storyId)
    {
        return _context.StoryLikes
            .AsNoTracking()
            .Where(l => EF.Functions.ILike(l.StoryId, storyId))
            .CountAsync();
    }

    /// <summary>
    /// Batch version of likes count; uses ILike for case-insensitive match (same as GetStoryLikesCountAsync).
    /// Dictionary keys use the casing from the input <paramref name="storyIds"/> for consistency with callers.
    /// </summary>
    public async Task<Dictionary<string, int>> GetStoryLikesCountsAsync(IReadOnlyList<string> storyIds)
    {
        if (storyIds == null || storyIds.Count == 0)
            return new Dictionary<string, int>();
        var list = storyIds.Distinct().ToList();
        // EF Core translates: list.Any(id => EF.Functions.ILike(l.StoryId, id)) -> server-side ILIKE OR chain (Npgsql).
        var counts = await _context.StoryLikes
            .AsNoTracking()
            .Where(l => list.Any(id => EF.Functions.ILike(l.StoryId, id)))
            .GroupBy(l => l.StoryId)
            .Select(g => new { StoryId = g.Key, Count = g.Count() })
            .ToListAsync();
        // Map DB casing back to caller's list casing so dictionary keys match requested storyIds.
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var x in counts)
        {
            var key = list.FirstOrDefault(id => string.Equals(id, x.StoryId, StringComparison.OrdinalIgnoreCase)) ?? x.StoryId;
            result[key] = x.Count;
        }
        return result;
    }

    public async Task<bool> RemoveLikeAsync(Guid userId, string storyId, CancellationToken ct = default)
    {
        var like = await _context.StoryLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && 
                EF.Functions.ILike(l.StoryId, storyId), ct);

        if (like == null)
            return false;

        _context.StoryLikes.Remove(like);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}

