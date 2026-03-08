using Microsoft.Extensions.Caching.Memory;
using XooCreator.BA.Features.Stories.Configuration;
using XooCreator.BA.Features.Stories.Repositories;

namespace XooCreator.BA.Features.Stories.Services.Caching;

/// <summary>
/// Dedicated cache for Welcome Flow options (24h TTL) and invalidation of story content for welcome story IDs.
/// Uses GetOrCreateAsync so that after a cache miss only one request runs the loader; concurrent requests wait and get the same value.
/// </summary>
public sealed class WelcomeFlowCacheService : IWelcomeFlowCacheService
{
    private const string OptionsCacheKey = "WelcomeFlow:Options";
    private static readonly TimeSpan OptionsTtl = TimeSpan.FromHours(24);

    private readonly IMemoryCache _cache;
    private readonly IStoriesRepository _storiesRepository;

    public WelcomeFlowCacheService(IMemoryCache cache, IStoriesRepository storiesRepository)
    {
        _cache = cache;
        _storiesRepository = storiesRepository;
    }

    /// <inheritdoc />
    public async Task<WelcomeFlowOptions> GetOrSetOptionsAsync(Func<CancellationToken, Task<WelcomeFlowOptions>> loader, CancellationToken ct = default)
    {
        var result = await _cache.GetOrCreateAsync(OptionsCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = OptionsTtl;
            return await loader(ct);
        });
        return result!;
    }

    /// <inheritdoc />
    public void InvalidateAfterUpdate(IEnumerable<string> welcomeStoryIds)
    {
        _cache.Remove(OptionsCacheKey);
        foreach (var id in welcomeStoryIds)
        {
            if (string.IsNullOrWhiteSpace(id)) continue;
            _storiesRepository.InvalidateStoryContentCache(id.Trim());
        }
    }
}
