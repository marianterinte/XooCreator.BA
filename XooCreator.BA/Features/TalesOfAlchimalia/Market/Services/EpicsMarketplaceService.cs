using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

using XooCreator.BA.Features.Subscription.Services;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

public interface IEpicsMarketplaceService
{
    Task<GetMarketplaceEpicsResponse> GetMarketplaceEpicsAsync(Guid userId, string locale, SearchEpicsRequest request);
    Task<EpicDetailsDto?> GetEpicDetailsAsync(string epicId, Guid userId, string locale);
}

public class EpicsMarketplaceService : IEpicsMarketplaceService
{
    private readonly EpicsMarketplaceRepository _repository;
    private readonly IExclusiveContentService _exclusiveContent;

    public EpicsMarketplaceService(EpicsMarketplaceRepository repository, IExclusiveContentService exclusiveContent)
    {
        _repository = repository;
        _exclusiveContent = exclusiveContent;
    }

    public async Task<GetMarketplaceEpicsResponse> GetMarketplaceEpicsAsync(
        Guid userId,
        string locale,
        SearchEpicsRequest request)
    {
        var (epics, totalCount, hasMore) = await _repository.GetMarketplaceEpicsWithPaginationAsync(
            userId,
            locale,
            request);
        var (_, exclusiveEpicIds) = await _exclusiveContent.GetAllExclusiveIdsAsync();
        var minimumTiers = await _exclusiveContent.GetMinimumTiersForExclusiveEpicsAsync();
        var (_, userEpicIds) = userId == Guid.Empty
            ? (Array.Empty<string>().AsReadOnly(), Array.Empty<string>().AsReadOnly())
            : await _exclusiveContent.GetUserExclusiveContentAsync(userId);
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
            TotalCount = totalCount,
            HasMore = hasMore
        };
    }

    public async Task<EpicDetailsDto?> GetEpicDetailsAsync(string epicId, Guid userId, string locale)
    {
        var dto = await _repository.GetEpicDetailsAsync(epicId, userId, locale);
        if (dto == null) return null;

        var isExclusive = await _exclusiveContent.IsEpicExclusiveAsync(epicId);
        var minimumTier = isExclusive ? await _exclusiveContent.GetMinimumTierForEpicAsync(epicId) : null;
        var hasExclusiveAccess = userId == Guid.Empty
            ? false
            : !isExclusive || await _exclusiveContent.HasAccessToEpicAsync(userId, epicId);

        return dto with { IsExclusive = isExclusive, MinimumTier = minimumTier, HasExclusiveAccess = hasExclusiveAccess };
    }
}

