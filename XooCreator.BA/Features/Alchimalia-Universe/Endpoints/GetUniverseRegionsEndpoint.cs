using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using Microsoft.Extensions.Options;
using XooCreator.BA.Infrastructure.Caching;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class GetUniverseRegionsEndpoint
{
    private readonly IStoryRegionService _service;
    private readonly IAppCache _cache;
    private readonly IOptionsMonitor<UniverseCachingOptions> _cachingOptions;

    public GetUniverseRegionsEndpoint(
        IStoryRegionService service,
        IAppCache cache,
        IOptionsMonitor<UniverseCachingOptions> cachingOptions)
    {
        _service = service;
        _cache = cache;
        _cachingOptions = cachingOptions;
    }

    [Route("/api/alchimalia-universe/regions")]
    [Authorize]
    public static async Task<Results<Ok<List<StoryRegionListItemDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromServices] GetUniverseRegionsEndpoint ep,
        CancellationToken ct)
    {
        // Hardcoded filtering for "alchimalia_universe" topic as required for this page
        var cfg = ep._cachingOptions.CurrentValue;
        var ttlMinutes = cfg.Enabled ? cfg.UniverseRegionsMinutes : 0;
        var cacheKey = UniverseCachingOptions.GetUniverseRegionsKey();

        var regions = await ep._cache.GetOrCreateAsync(
            cacheKey,
            TimeSpan.FromMinutes(ttlMinutes),
            () => ep._service.ListPublishedRegionsByTopicAsync("alchimalia_universe", ct),
            ct);
        return TypedResults.Ok(regions);
    }
}
