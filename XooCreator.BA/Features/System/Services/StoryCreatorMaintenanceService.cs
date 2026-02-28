using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.System.Services;

public class StoryCreatorMaintenanceService : IStoryCreatorMaintenanceService
{
    private const string SettingKey = "story-creator-disabled";
    private const string CacheKey = "story_creator_maintenance_disabled";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

    private readonly XooDbContext _context;
    private readonly IMemoryCache _cache;

    public StoryCreatorMaintenanceService(XooDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<bool> IsStoryCreatorDisabledAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKey, out bool cached))
            return cached;

        var setting = await _context.PlatformSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Key == SettingKey, ct);

        var disabled = setting?.BoolValue ?? false;
        _cache.Set(CacheKey, disabled, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl });
        return disabled;
    }

    public void InvalidateCache()
    {
        _cache.Remove(CacheKey);
    }
}
