using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public class EpicLikesRepository : IEpicLikesRepository
{
    private readonly XooDbContext _context;

    public EpicLikesRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ToggleLikeAsync(Guid userId, string epicId, CancellationToken ct = default)
    {
        var existing = await _context.EpicLikes
            .FirstOrDefaultAsync(l => l.UserId == userId &&
                EF.Functions.ILike(l.EpicId, epicId), ct);

        if (existing != null)
        {
            _context.EpicLikes.Remove(existing);
            await _context.SaveChangesAsync(ct);
            return false;
        }

        var like = new EpicLike
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EpicId = epicId,
            LikedAt = DateTime.UtcNow
        };

        _context.EpicLikes.Add(like);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> IsLikedAsync(Guid userId, string epicId)
    {
        return await _context.EpicLikes
            .AnyAsync(l => l.UserId == userId &&
                EF.Functions.ILike(l.EpicId, epicId));
    }

    public Task<int> GetEpicLikesCountAsync(string epicId)
    {
        return _context.EpicLikes
            .Where(l => EF.Functions.ILike(l.EpicId, epicId))
            .CountAsync();
    }

    public async Task<Dictionary<string, int>> GetEpicLikesCountsAsync(IReadOnlyList<string> epicIds)
    {
        if (epicIds == null || epicIds.Count == 0)
            return new Dictionary<string, int>();
        var list = epicIds.Distinct().ToList();
        var counts = await _context.EpicLikes
            .AsNoTracking()
            .Where(l => list.Any(id => EF.Functions.ILike(l.EpicId, id)))
            .GroupBy(l => l.EpicId)
            .Select(g => new { EpicId = g.Key, Count = g.Count() })
            .ToListAsync();
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var x in counts)
        {
            var key = list.FirstOrDefault(id => string.Equals(id, x.EpicId, StringComparison.OrdinalIgnoreCase)) ?? x.EpicId;
            result[key] = x.Count;
        }
        return result;
    }
}
