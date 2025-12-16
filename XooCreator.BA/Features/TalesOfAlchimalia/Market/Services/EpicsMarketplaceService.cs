using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

public interface IEpicsMarketplaceService
{
    Task<GetMarketplaceEpicsResponse> GetMarketplaceEpicsAsync(Guid userId, string locale, SearchEpicsRequest request);
    Task<EpicDetailsDto?> GetEpicDetailsAsync(string epicId, Guid userId, string locale);
}

public class EpicsMarketplaceService : IEpicsMarketplaceService
{
    private readonly EpicsMarketplaceRepository _repository;

    public EpicsMarketplaceService(EpicsMarketplaceRepository repository)
    {
        _repository = repository;
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

        return new GetMarketplaceEpicsResponse
        {
            Epics = epics,
            TotalCount = totalCount,
            HasMore = hasMore
        };
    }

    public async Task<EpicDetailsDto?> GetEpicDetailsAsync(string epicId, Guid userId, string locale)
    {
        return await _repository.GetEpicDetailsAsync(epicId, userId, locale);
    }
}

