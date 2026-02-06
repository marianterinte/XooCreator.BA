using Microsoft.Extensions.Caching.Memory;

namespace XooCreator.BA.Infrastructure.Caching;

/// <summary>
/// Small abstraction over IMemoryCache used for simple cache-aside patterns.
/// Keeps all cache access in one place so we can later swap to a distributed cache if needed.
/// </summary>
public interface IAppCache
{
    /// <summary>
    /// Get a value from cache by key or create it using the provided factory.
    /// The created value is stored with the given absolute TTL.
    /// </summary>
    Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan ttl,
        Func<Task<T>> factory,
        CancellationToken ct = default);

    /// <summary>
    /// Remove a key from cache (no-op if missing).
    /// </summary>
    void Remove(string key);
}

