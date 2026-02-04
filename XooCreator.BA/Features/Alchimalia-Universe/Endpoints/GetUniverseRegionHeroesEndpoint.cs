using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class GetUniverseRegionHeroesEndpoint
{
    private readonly IEpicHeroService _service;

    public GetUniverseRegionHeroesEndpoint(IEpicHeroService service)
    {
        _service = service;
    }

    [Route("/api/alchimalia-universe/regions/{regionId}/heroes")]
    [Authorize]
    public static async Task<Results<Ok<List<UniverseHeroListItemDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string regionId,
        [FromServices] GetUniverseRegionHeroesEndpoint ep,
        CancellationToken ct)
    {
        // Hardcoded filtering for "alchimalia_universe" topic as required for this page
        var heroes = await ep._service.ListPublishedHeroesByRegionAndTopicAsync(regionId, "alchimalia_universe", ct);
        
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
