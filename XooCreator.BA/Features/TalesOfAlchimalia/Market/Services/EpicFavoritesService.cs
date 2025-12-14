using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

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

    public EpicFavoritesService(IEpicFavoritesRepository repository)
    {
        _repository = repository;
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

        return new GetMarketplaceEpicsResponse
        {
            Epics = epics,
            TotalCount = epics.Count,
            HasMore = false
        };
    }
}

