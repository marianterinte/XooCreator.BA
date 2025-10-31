using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

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

    [Route("/api/{locale}/tales-of-alchimalia/market/details/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<StoryDetailsDto>, NotFound, UnauthorizedHttpResult>> HandleGet(
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


