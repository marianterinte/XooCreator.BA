using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.Stories.Repositories;
using XooCreator.BA.Features.Stories.DTOs;

namespace XooCreator.BA.Features.Stories.Endpoints;

[Endpoint]
public class CheckStoryIdEndpoint
{
    private readonly IStoriesRepository _repository;
    private readonly IUserContextService _userContext;

    public CheckStoryIdEndpoint(IStoriesRepository repository, IUserContextService userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    [Route("/api/{locale}/stories/editor/check-id/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<CheckStoryIdResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] CheckStoryIdEndpoint ep,
        [FromRoute] string storyId)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var exists = await ep._repository.StoryIdExistsAsync(storyId);
        return TypedResults.Ok(new CheckStoryIdResponse
        {
            Exists = exists
        });
    }
}

