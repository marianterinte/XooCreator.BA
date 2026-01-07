namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;

public interface IMarketplaceCatalogCache
{
    Task<IReadOnlyList<StoryMarketplaceBaseItem>> GetStoriesBaseAsync(string locale, CancellationToken ct);
    Task<IReadOnlyDictionary<string, StoryMarketplaceStats>> GetStoryStatsAsync(CancellationToken ct);

    Task<IReadOnlyList<EpicMarketplaceBaseItem>> GetEpicsBaseAsync(string locale, CancellationToken ct);
    Task<IReadOnlyDictionary<string, EpicMarketplaceStats>> GetEpicStatsAsync(CancellationToken ct);
}

public interface IMarketplaceCacheInvalidator
{
    void ResetAll();
}

public interface IMarketplaceCacheControl : IMarketplaceCacheInvalidator
{
    bool IsEnabled { get; }
    void SetEnabled(bool enabled);
    MarketplaceCacheStatus GetStatus();
}

public sealed record MarketplaceCacheStatus(
    bool Enabled,
    int BaseTtlMinutes,
    int StatsTtlMinutes,
    bool HasRuntimeOverride);


