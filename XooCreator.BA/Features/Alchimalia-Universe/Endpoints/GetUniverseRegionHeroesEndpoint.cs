using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using Microsoft.Extensions.Options;
using XooCreator.BA.Infrastructure.Caching;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class GetUniverseRegionHeroesEndpoint
{
    private readonly IEpicHeroService _service;
    private readonly IAppCache _cache;
    private readonly IOptionsMonitor<UniverseCachingOptions> _cachingOptions;

    public GetUniverseRegionHeroesEndpoint(
        IEpicHeroService service,
        IAppCache cache,
        IOptionsMonitor<UniverseCachingOptions> cachingOptions)
    {
        _service = service;
        _cache = cache;
        _cachingOptions = cachingOptions;
    }

    [Route("/api/alchimalia-universe/regions/{regionId}/heroes")]
    [Authorize]
    public static async Task<Results<Ok<List<UniverseHeroListItemDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string regionId,
        [FromServices] GetUniverseRegionHeroesEndpoint ep,
        CancellationToken ct)
    {
        // Hardcoded filtering for "alchimalia_universe" topic as required for this page
        var cfg = ep._cachingOptions.CurrentValue;
        var ttlMinutes = cfg.Enabled ? cfg.UniverseRegionHeroesMinutes : 0;
        var cacheKey = UniverseCachingOptions.GetUniverseRegionHeroesKey(regionId);

        var heroes = await ep._cache.GetOrCreateAsync(
            cacheKey,
            TimeSpan.FromMinutes(ttlMinutes),
            () => ep._service.ListPublishedHeroesByRegionAndTopicAsync(regionId, "alchimalia_universe", ct),
            ct);

        // Map to DTO that is compatible with frontend expectation (which expects heroId property)
        var mapped = heroes.Select(h => new UniverseHeroListItemDto
        {
            Id = h.Id,
            HeroId = h.Id, // EpicHero uses Id as the user-friendly ID (e.g. puf-puf)
            Name = h.Name,
            ImageUrl = h.ImageUrl,
            Status = h.Status
        }).ToList();

        return TypedResults.Ok(mapped);
    }

    public record UniverseHeroListItemDto
    {
        public required string Id { get; init; }
        public required string HeroId { get; init; }
        public required string Name { get; init; }
        public string? ImageUrl { get; init; }
        public string Status { get; init; } = "published";
    }
}
