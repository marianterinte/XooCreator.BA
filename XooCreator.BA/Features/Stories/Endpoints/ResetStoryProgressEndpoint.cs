using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.Stories.DTOs;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.Stories.Endpoints;

[Endpoint]
public class ResetStoryProgressEndpoint
{
    private readonly IStoriesService _storiesService;
    private readonly IUserContextService _userContext;

    public ResetStoryProgressEndpoint(IStoriesService storiesService, IUserContextService userContext)
    {
        _storiesService = storiesService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/stories/reset-progress")]
    [Authorize]
    public static async Task<Results<Ok<ResetStoryProgressResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] ResetStoryProgressEndpoint ep,
        [FromBody] ResetStoryProgressRequest request)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
        {
            return TypedResults.Unauthorized();
        }

        var result = await ep._storiesService.ResetStoryProgressAsync(userId.Value, request);
        return TypedResults.Ok(result);
    }
}

