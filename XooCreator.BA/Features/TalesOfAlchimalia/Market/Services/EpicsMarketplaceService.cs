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
        var enrichedEpics = epics.Select(e => e with { IsExclusive = exclusiveEpicIds.Contains(e.Id) }).ToList();

        return new GetMarketplaceEpicsResponse
        {
            Epics = enrichedEpics,
            TotalCount = totalCount,
            HasMore = hasMore
        };
    }

    public async Task<EpicDetailsDto?> GetEpicDetailsAsync(string epicId, Guid userId, string locale)
    {
        return await _repository.GetEpicDetailsAsync(epicId, userId, locale);
    }
}

