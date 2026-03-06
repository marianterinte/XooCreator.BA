using XooCreator.BA.Features.Stories.Configuration;

namespace XooCreator.BA.Features.Stories.Services.Caching;

/// <summary>
/// Dedicated cache for Welcome Flow: options (24h) and invalidation of story content for welcome story IDs on config update.
/// </summary>
public interface IWelcomeFlowCacheService
{
    /// <summary>
    /// Gets options from cache, or runs the loader once and caches the result (24h). Uses GetOrCreateAsync so concurrent requests after a miss share a single load.
    /// </summary>
    Task<WelcomeFlowOptions> GetOrSetOptionsAsync(Func<CancellationToken, Task<WelcomeFlowOptions>> loader, CancellationToken ct = default);

    /// <summary>
    /// Removes options from cache and invalidates story content cache for each welcome story ID (call after admin updates config).
    /// </summary>
    void InvalidateAfterUpdate(IEnumerable<string> welcomeStoryIds);
}
