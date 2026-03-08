using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

using XooCreator.BA.Features.Subscription.Services;

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
    private readonly IExclusiveContentService _exclusiveContent;

    public FavoritesService(IFavoritesRepository repository, IExclusiveContentService exclusiveContent)
    {
        _repository = repository;
        _exclusiveContent = exclusiveContent;
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
        var (exclusiveStoryIds, _) = await _exclusiveContent.GetAllExclusiveIdsAsync();
        var enrichedStories = stories.Select(s => s with { IsExclusive = exclusiveStoryIds.Contains(s.Id) }).ToList();

        return new GetMarketplaceStoriesResponse
        {
            Stories = enrichedStories,
            TotalCount = enrichedStories.Count,
            HasMore = false
        };
    }
}

