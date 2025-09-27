using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.Stories.Endpoints;

[Endpoint]
public class GetStoriesEndpoint
{
    private readonly IStoriesService _storiesService;
    public GetStoriesEndpoint(IStoriesService storiesService) => _storiesService = storiesService;

    [Route("/api/{locale}/stories")]
    public static async Task<Ok<GetStoriesResponse>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetStoriesEndpoint ep)
    {
        var result = await ep._storiesService.GetAllStoriesAsync();
        return TypedResults.Ok(result);
    }
}
