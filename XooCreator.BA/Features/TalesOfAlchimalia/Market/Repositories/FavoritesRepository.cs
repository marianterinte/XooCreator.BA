using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public interface IFavoritesRepository
{
    Task<bool> AddFavoriteAsync(Guid userId, string storyId, CancellationToken ct = default);
    Task<bool> RemoveFavoriteAsync(Guid userId, string storyId, CancellationToken ct = default);
    Task<bool> IsFavoriteAsync(Guid userId, string storyId);
    Task<List<StoryMarketplaceItemDto>> GetFavoriteStoriesAsync(Guid userId, string locale);
}

public class FavoritesRepository : IFavoritesRepository
{
    private readonly XooDbContext _context;
    private readonly IStoriesMarketplaceRepository _marketplaceRepository;

    public FavoritesRepository(XooDbContext context, IStoriesMarketplaceRepository marketplaceRepository)
    {
        _context = context;
        _marketplaceRepository = marketplaceRepository;
    }

    public async Task<bool> AddFavoriteAsync(Guid userId, string storyId, CancellationToken ct = default)
    {
        var storyDef = await _context.StoryDefinitions
            .FirstOrDefaultAsync(s => s.StoryId == storyId && s.IsActive);

        if (storyDef == null)
            return false;

        // Check if already favorited
        var existing = await _context.UserFavoriteStories
            .FirstOrDefaultAsync(f => f.UserId == userId && f.StoryDefinitionId == storyDef.Id);

        if (existing != null)
            return true; // Already favorited

        var favorite = new UserFavoriteStories
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StoryDefinitionId = storyDef.Id,
            AddedAt = DateTime.UtcNow
        };

        _context.UserFavoriteStories.Add(favorite);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveFavoriteAsync(Guid userId, string storyId, CancellationToken ct = default)
    {
        var storyDef = await _context.StoryDefinitions
            .FirstOrDefaultAsync(s => s.StoryId == storyId);

        if (storyDef == null)
            return false;

        var favorite = await _context.UserFavoriteStories
            .FirstOrDefaultAsync(f => f.UserId == userId && f.StoryDefinitionId == storyDef.Id);

        if (favorite == null)
            return false;

        _context.UserFavoriteStories.Remove(favorite);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, string storyId)
    {
        var storyDef = await _context.StoryDefinitions
            .FirstOrDefaultAsync(s => s.StoryId == storyId);

        if (storyDef == null)
            return false;

        return await _context.UserFavoriteStories
            .AnyAsync(f => f.UserId == userId && f.StoryDefinitionId == storyDef.Id);
    }

    public async Task<List<StoryMarketplaceItemDto>> GetFavoriteStoriesAsync(Guid userId, string locale)
    {
        var favoriteStoryIds = await _context.UserFavoriteStories
            .Include(f => f.StoryDefinition)
            .Where(f => f.UserId == userId && f.StoryDefinition.IsActive)
            .Select(f => f.StoryDefinition.StoryId)
            .ToListAsync();

        if (!favoriteStoryIds.Any())
            return new List<StoryMarketplaceItemDto>();

        // Get all marketplace stories and filter to favorites
        var allStories = await _marketplaceRepository.GetMarketplaceStoriesAsync(userId, locale, new SearchStoriesRequest
        {
            Page = 1,
            PageSize = 10000 // Get all to filter favorites
        });

        // Filter to only favorites
        var favoriteStories = allStories
            .Where(s => favoriteStoryIds.Contains(s.Id))
            .ToList();

        return favoriteStories;
    }
}

