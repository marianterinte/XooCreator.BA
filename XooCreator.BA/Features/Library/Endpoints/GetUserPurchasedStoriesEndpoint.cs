using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market;
using XooCreator.BA.Features.Stories;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.Library.Endpoints;

[Endpoint]
public class GetUserPurchasedStoriesEndpoint
{
    private readonly IStoriesMarketplaceService _marketplaceService;
    private readonly IUserContextService _userContext;

    public GetUserPurchasedStoriesEndpoint(IStoriesMarketplaceService marketplaceService, IUserContextService userContext)
    {
        _marketplaceService = marketplaceService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/stories/marketplace/purchased")]
    [Authorize]
    public static async Task<Ok<GetUserPurchasedStoriesResponse>> HandleGetPurchasedStories(
        [FromRoute] string locale,
        [FromServices] GetUserPurchasedStoriesEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) throw new UnauthorizedAccessException("User not found");

        var result = await ep._marketplaceService.GetUserPurchasedStoriesAsync(userId.Value, locale);
        return TypedResults.Ok(result);
    }
}
