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



    public async Task<bool> ToggleLikeAsync(Guid userId, string storyId)
    {


        // Check if already liked
        var existing = await _context.StoryLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && 
                EF.Functions.ILike(l.StoryId, storyId));

        if (existing != null)
        {
            // Unlike - remove the like
            _context.StoryLikes.Remove(existing);
            await _context.SaveChangesAsync();
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
        await _context.SaveChangesAsync();
        return true; // Return true to indicate liked
    }

    public async Task<bool> IsLikedAsync(Guid userId, string storyId)
    {
        return await _context.StoryLikes
            .AnyAsync(l => l.UserId == userId && 
                EF.Functions.ILike(l.StoryId, storyId));
    }

    public Task<int> GetStoryLikesCountAsync(string storyId)
    {
        return _context.StoryLikes
            .Where(l => EF.Functions.ILike(l.StoryId, storyId))
            .CountAsync();
    }

    public async Task<bool> RemoveLikeAsync(Guid userId, string storyId)
    {
        var like = await _context.StoryLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && 
                EF.Functions.ILike(l.StoryId, storyId));

        if (like == null)
            return false;

        _context.StoryLikes.Remove(like);
        await _context.SaveChangesAsync();
        return true;
    }
}

