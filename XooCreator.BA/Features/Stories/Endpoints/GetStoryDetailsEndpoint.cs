using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.Stories.Endpoints;

[Endpoint]
public class GetStoryDetailsEndpoint
{
    private readonly IStoriesMarketplaceService _marketplaceService;
    private readonly IUserContextService _userContext;

    public GetStoryDetailsEndpoint(IStoriesMarketplaceService marketplaceService, IUserContextService userContext)
    {
        _marketplaceService = marketplaceService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/stories/marketplace/details/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<StoryDetailsDto>, NotFound, UnauthorizedHttpResult>> HandleGetStoryDetails(
        [FromRoute] string storyId,
        [FromRoute] string locale,
        [FromServices] GetStoryDetailsEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var result = await ep._marketplaceService.GetStoryDetailsAsync(storyId, userId.Value, locale);
        if (result == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(result);
    }
}
