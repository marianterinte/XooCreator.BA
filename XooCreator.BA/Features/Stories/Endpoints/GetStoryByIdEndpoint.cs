using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.Stories.Endpoints;

[Endpoint]
public class GetStoryByIdEndpoint
{
    private readonly IStoriesService _storiesService;
    private readonly IUserContextService _userContext;
    public GetStoryByIdEndpoint(IStoriesService storiesService, IUserContextService userContext)
    {
        _storiesService = storiesService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/stories/{storyId}")] // GET /api/{locale}/stories/{storyId}
    [Authorize]
    public static async Task<Results<Ok<GetStoryByIdResponse>, UnauthorizedHttpResult, NotFound>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetStoryByIdEndpoint ep,
        [FromRoute] string storyId)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var result = await ep._storiesService.GetStoryByIdAsync(userId.Value, storyId, locale);
        if (result.Story == null) return TypedResults.NotFound();
        return TypedResults.Ok(result);
    }

    [Route("/api/{locale}/stories/{storyId}/edit")] // GET editable story
    [Authorize]
    public static async Task<Results<Ok<EditableStoryDto>, NotFound, UnauthorizedHttpResult>> HandleGetEditable(
        [FromRoute] string locale,
        [FromServices] GetStoryByIdEndpoint ep,
        [FromRoute] string storyId)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var editable = await ep._storiesService.GetStoryForEditAsync(storyId, locale);
        if (editable == null) return TypedResults.NotFound();
        return TypedResults.Ok(editable);
    }

}
