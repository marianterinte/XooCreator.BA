using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class GetTreeStoryProgressLegacyEndpoint
{
    private readonly ITreeOfLightService _service;
    private readonly IUserContextService _userContext;
    public GetTreeStoryProgressLegacyEndpoint(ITreeOfLightService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Obsolete("Use /api/{locale}/tree-of-light/user-progress")]
    [Route("/api/{locale}/tree-of-light/stories")] // GET (legacy with locale)
    public static async Task<Results<Ok<List<StoryProgressDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetTreeStoryProgressLegacyEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var stories = await ep._service.GetStoryProgressAsync(userId.Value);
        return TypedResults.Ok(stories);
    }
}
