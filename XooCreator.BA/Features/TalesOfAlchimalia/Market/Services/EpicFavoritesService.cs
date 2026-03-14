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
        var minimumTiers = await _exclusiveContent.GetMinimumTiersForExclusiveEpicsAsync();
        var (_, userEpicIds) = await _exclusiveContent.GetUserExclusiveContentAsync(userId);
        var enrichedEpics = epics.Select(e =>
        {
            var isExclusive = exclusiveEpicIds.Contains(e.Id);
            var minimumTier = isExclusive && minimumTiers.TryGetValue(e.Id, out var tier) ? tier : null;
            var hasExclusiveAccess = !isExclusive || userEpicIds.Contains(e.Id);
            return e with { IsExclusive = isExclusive, MinimumTier = minimumTier, HasExclusiveAccess = hasExclusiveAccess };
        }).ToList();

        return new GetMarketplaceEpicsResponse
        {
            Epics = enrichedEpics,
            TotalCount = enrichedEpics.Count,
            HasMore = false
        };
    }
}

