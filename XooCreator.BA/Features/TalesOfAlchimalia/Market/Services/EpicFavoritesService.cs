using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

using XooCreator.BA.Features.Subscription.Services;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

public interface IEpicFavoritesService
{
    Task<bool> AddFavoriteAsync(Guid userId, string epicId);
    Task<bool> RemoveFavoriteAsync(Guid userId, string epicId);
    Task<bool> IsFavoriteAsync(Guid userId, string epicId);
    Task<GetMarketplaceEpicsResponse> GetFavoriteEpicsAsync(Guid userId, string locale);
}

public class EpicFavoritesService : IEpicFavoritesService
{
    private readonly IEpicFavoritesRepository _repository;
    private readonly IExclusiveContentService _exclusiveContent;

    public EpicFavoritesService(IEpicFavoritesRepository repository, IExclusiveContentService exclusiveContent)
    {
        _repository = repository;
        _exclusiveContent = exclusiveContent;
    }

    public async Task<bool> AddFavoriteAsync(Guid userId, string epicId)
    {
        return await _repository.AddFavoriteAsync(userId, epicId);
    }

    public async Task<bool> RemoveFavoriteAsync(Guid userId, string epicId)
    {
        return await _repository.RemoveFavoriteAsync(userId, epicId);
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, string epicId)
    {
        return await _repository.IsFavoriteAsync(userId, epicId);
    }

    public async Task<GetMarketplaceEpicsResponse> GetFavoriteEpicsAsync(Guid userId, string locale)
    {
        var epics = await _repository.GetFavoriteEpicsAsync(userId, locale);
        var (_, exclusiveEpicIds) = await _exclusiveContent.GetAllExclusiveIdsAsync();
        var enrichedEpics = epics.Select(e => e with { IsExclusive = exclusiveEpicIds.Contains(e.Id) }).ToList();

        return new GetMarketplaceEpicsResponse
        {
            Epics = enrichedEpics,
            TotalCount = enrichedEpics.Count,
            HasMore = false
        };
    }
}

