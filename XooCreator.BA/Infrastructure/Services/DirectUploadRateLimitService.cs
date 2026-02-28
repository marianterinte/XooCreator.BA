using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace XooCreator.BA.Infrastructure.Services;

/// <summary>
/// Rate limit for direct-to-blob request-upload endpoints: max N requests per user per hour.
/// </summary>
public interface IDirectUploadRateLimitService
{
    /// <summary>
    /// Returns true if the user is allowed to make a request-upload call; false if rate limit exceeded (caller should return 429).
    /// </summary>
    bool TryAcquire(Guid userId, CancellationToken ct = default);
}

public class DirectUploadRateLimitService : IDirectUploadRateLimitService
{
    private readonly IMemoryCache _cache;
    private readonly int _maxRequestsPerUserPerHour;
    private static readonly TimeSpan Window = TimeSpan.FromHours(1);

    public DirectUploadRateLimitService(IMemoryCache cache, IConfiguration configuration)
    {
        _cache = cache;
        _maxRequestsPerUserPerHour = configuration.GetValue<int?>("StoryEditor:DirectUpload:MaxRequestsPerUserPerHour") ?? 10;
    }

    public bool TryAcquire(Guid userId, CancellationToken ct = default)
    {
        var windowKey = DateTime.UtcNow.ToString("yyyyMMddHH", System.Globalization.CultureInfo.InvariantCulture);
        var cacheKey = $"DirectUpload:RateLimit:{userId}:{windowKey}";

        lock (GetLock(cacheKey))
        {
            var count = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = Window;
                return 0;
            });
            if (count >= _maxRequestsPerUserPerHour)
                return false;
            _cache.Set(cacheKey, count + 1, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = Window });
            return true;
        }
    }

    private const int LockPoolSize = 256;
    private static readonly object[] LockPool = Enumerable.Range(0, LockPoolSize).Select(_ => new object()).ToArray();
    private static object GetLock(string key)
    {
        var hash = StringComparer.OrdinalIgnoreCase.GetHashCode(key);
        return LockPool[Math.Abs(unchecked(hash % LockPoolSize))];
    }
}
