using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class GetUniverseRegionsEndpoint
{
    private readonly IStoryRegionService _service;

    public GetUniverseRegionsEndpoint(IStoryRegionService service)
    {
        _service = service;
    }

    [Route("/api/alchimalia-universe/regions")]
    [Authorize]
    public static async Task<Results<Ok<List<StoryRegionListItemDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromServices] GetUniverseRegionsEndpoint ep,
        CancellationToken ct)
    {
        // Hardcoded filtering for "alchimalia_universe" topic as required for this page
        var regions = await ep._service.ListPublishedRegionsByTopicAsync("alchimalia_universe", ct);
        return TypedResults.Ok(regions);
    }
}
