using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.Stories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Endpoints;

[Endpoint]
public class TalesMarketEndpoint
{
    private readonly IStoriesService _storiesService;
    
    public TalesMarketEndpoint(IStoriesService storiesService) => _storiesService = storiesService;

    [Route("/api/{locale}/tales-of-alchimalia/market")]
    [Authorize]
    public static async Task<Ok<GetStoriesResponse>> HandleGet(
        [FromRoute] string locale,
        [FromServices] TalesMarketEndpoint ep)
    {
        var result = await ep._storiesService.GetAllStoriesAsync(locale);
        return TypedResults.Ok(result);
    }
}
