using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public interface IEpicFavoritesRepository
{
    Task<bool> AddFavoriteAsync(Guid userId, string epicId);
    Task<bool> RemoveFavoriteAsync(Guid userId, string epicId);
    Task<bool> IsFavoriteAsync(Guid userId, string epicId);
    Task<List<EpicMarketplaceItemDto>> GetFavoriteEpicsAsync(Guid userId, string locale);
}

public class EpicFavoritesRepository : IEpicFavoritesRepository
{
    private readonly XooDbContext _context;
    private readonly EpicsMarketplaceRepository _epicsRepository;

    public EpicFavoritesRepository(XooDbContext context, EpicsMarketplaceRepository epicsRepository)
    {
        _context = context;
        _epicsRepository = epicsRepository;
    }

    public async Task<bool> AddFavoriteAsync(Guid userId, string epicId)
    {
        var epic = await _context.StoryEpicDefinitions
            .FirstOrDefaultAsync(e => e.Id == epicId && e.Status == "published");

        if (epic == null)
            return false;

        // Check if already favorited
        var existing = await _context.UserFavoriteEpics
            .FirstOrDefaultAsync(f => f.UserId == userId && f.EpicId == epicId);

        if (existing != null)
            return true; // Already favorited

        var favorite = new UserFavoriteEpics
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EpicId = epicId,
            AddedAt = DateTime.UtcNow
        };

        _context.UserFavoriteEpics.Add(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFavoriteAsync(Guid userId, string epicId)
    {
        var favorite = await _context.UserFavoriteEpics
            .FirstOrDefaultAsync(f => f.UserId == userId && f.EpicId == epicId);

        if (favorite == null)
            return false;

        _context.UserFavoriteEpics.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, string epicId)
    {
        return await _context.UserFavoriteEpics
            .AnyAsync(f => f.UserId == userId && f.EpicId == epicId);
    }

    public async Task<List<EpicMarketplaceItemDto>> GetFavoriteEpicsAsync(Guid userId, string locale)
    {
        // Get favorite epic IDs for this user
        var favoriteEpicIds = await _context.UserFavoriteEpics
            .Where(f => f.UserId == userId)
            .Select(f => f.EpicId)
            .ToListAsync();

        if (!favoriteEpicIds.Any())
            return new List<EpicMarketplaceItemDto>();

        // Verify that these epics are published (use StoryEpicDefinitions)
        var publishedEpicIds = await _context.StoryEpicDefinitions
            .Where(e => favoriteEpicIds.Contains(e.Id) && e.Status == "published")
            .Select(e => e.Id)
            .ToListAsync();

        if (!publishedEpicIds.Any())
            return new List<EpicMarketplaceItemDto>();

        // Get all marketplace epics and filter to favorites
        var (allEpics, _, _) = await _epicsRepository.GetMarketplaceEpicsWithPaginationAsync(
            userId,
            locale,
            new SearchEpicsRequest
            {
                Page = 1,
                PageSize = 10000 // Get all to filter favorites
            });

        // Filter to only favorites
        var favoriteEpics = allEpics
            .Where(e => publishedEpicIds.Contains(e.Id))
            .ToList();

        return favoriteEpics;
    }
}

