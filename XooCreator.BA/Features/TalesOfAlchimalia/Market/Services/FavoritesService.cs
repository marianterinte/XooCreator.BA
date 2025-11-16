using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

public interface IFavoritesService
{
    Task<bool> AddFavoriteAsync(Guid userId, string storyId);
    Task<bool> RemoveFavoriteAsync(Guid userId, string storyId);
    Task<bool> IsFavoriteAsync(Guid userId, string storyId);
    Task<GetMarketplaceStoriesResponse> GetFavoriteStoriesAsync(Guid userId, string locale);
}

public class FavoritesService : IFavoritesService
{
    private readonly IFavoritesRepository _repository;

    public FavoritesService(IFavoritesRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> AddFavoriteAsync(Guid userId, string storyId)
    {
        return await _repository.AddFavoriteAsync(userId, storyId);
    }

    public async Task<bool> RemoveFavoriteAsync(Guid userId, string storyId)
    {
        return await _repository.RemoveFavoriteAsync(userId, storyId);
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, string storyId)
    {
        return await _repository.IsFavoriteAsync(userId, storyId);
    }

    public async Task<GetMarketplaceStoriesResponse> GetFavoriteStoriesAsync(Guid userId, string locale)
    {
        var stories = await _repository.GetFavoriteStoriesAsync(userId, locale);
        
        // Filter out purchased stories (as per requirement)
        var unpurchasedStories = stories.Where(s => !s.IsOwned).ToList();

        return new GetMarketplaceStoriesResponse
        {
            Stories = unpurchasedStories,
            FeaturedStories = new List<StoryMarketplaceItemDto>(),
            AvailableRegions = new List<string>(),
            AvailableAgeRatings = new List<string>(),
            AvailableCharacters = new List<string>(),
            TotalCount = unpurchasedStories.Count,
            HasMore = false
        };
    }
}

