using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetStoryEditEndpoint
{
    private readonly IStoriesService _storiesService;
    private readonly IUserContextService _userContext;

    public GetStoryEditEndpoint(IStoriesService storiesService, IUserContextService userContext)
    {
        _storiesService = storiesService;
        _userContext = userContext;
    }

    // GET editable story (craft-first)
    [Route("/api/{locale}/stories/{storyId}/edit")]
    [Authorize]
    public static async Task<Results<Ok<EditableStoryDto>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] GetStoryEditEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var editable = await ep._storiesService.GetStoryForEditAsync(storyId, locale);
        if (editable == null) return TypedResults.NotFound();
        return TypedResults.Ok(editable);
    }
}
