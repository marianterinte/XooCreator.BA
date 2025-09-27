using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class GetUserStoryProgressEndpoint
{
    private readonly ITreeOfLightService _service;
    private readonly IUserContextService _userContext;
    public GetUserStoryProgressEndpoint(ITreeOfLightService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-light/user-progress")] // GET
    public static async Task<Results<Ok<List<StoryProgressDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetUserStoryProgressEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var stories = await ep._service.GetStoryProgressAsync(userId.Value);
        return TypedResults.Ok(stories);
    }
}
