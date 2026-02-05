using Microsoft.Extensions.Caching.Memory;

namespace XooCreator.BA.Infrastructure.Caching;

/// <summary>
/// Default in-process implementation of <see cref="IAppCache"/> based on IMemoryCache.
/// </summary>
public sealed class MemoryAppCache : IAppCache
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryAppCache> _logger;

    public MemoryAppCache(IMemoryCache cache, ILogger<MemoryAppCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan ttl,
        Func<Task<T>> factory,
        CancellationToken ct = default)
    {
        if (ttl <= TimeSpan.Zero)
        {
            // TTL disabled or misconfigured: just call the factory without caching.
            _logger.LogDebug("AppCache disabled for key {Key} because TTL is non-positive.", key);
            return await factory();
        }

        if (_cache.TryGetValue(key, out T? cached) && cached is not null)
        {
            _logger.LogDebug("AppCache HIT for key {Key}.", key);
            return cached;
        }

        _logger.LogDebug("AppCache MISS for key {Key}. Loading value...", key);

        // Use IMemoryCache to store value with absolute expiration.
        var created = await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = ttl;
            return await factory();
        });

        return created!;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _logger.LogDebug("AppCache REMOVE for key {Key}.", key);
    }
}

