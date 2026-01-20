namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;

public sealed class MarketplaceCacheOptions
{
    public const string SectionName = "MarketplaceCache";

    /// <summary>
    /// Global kill-switch. When false, marketplace catalog/stats are NOT cached (fresh data per request).
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// TTL for "base catalog" data (stories/epics + translations + tags + age groups).
    /// </summary>
    public int BaseTtlMinutes { get; set; } = 720;

    /// <summary>
    /// TTL for volatile aggregates like readers count and ratings/reviews.
    /// </summary>
    public int StatsTtlMinutes { get; set; } = 10;
}


