using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.Stories.Endpoints;

[Endpoint]
public class MarkTileAsReadEndpoint
{
    private readonly IStoriesService _storiesService;
    private readonly IUserContextService _userContext;
    public MarkTileAsReadEndpoint(IStoriesService storiesService, IUserContextService userContext)
    {
        _storiesService = storiesService;
        _userContext = userContext;
    }

    [Route("/api/stories/mark-tile-read")] // POST /api/stories/mark-tile-read
    public static async Task<Results<Ok<MarkTileAsReadResponse>, BadRequest<MarkTileAsReadResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] MarkTileAsReadEndpoint ep,
        [FromBody] MarkTileAsReadRequest request)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var result = await ep._storiesService.MarkTileAsReadAsync(userId.Value, request);
        if (!result.Success) return TypedResults.BadRequest(result);
        return TypedResults.Ok(result);
    }
}
