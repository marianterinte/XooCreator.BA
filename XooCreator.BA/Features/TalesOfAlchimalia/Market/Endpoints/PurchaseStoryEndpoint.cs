using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.Stories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class PurchaseStoryEndpoint
{
    private readonly IStoriesMarketplaceService _marketplaceService;
    private readonly IUserContextService _userContext;

    public PurchaseStoryEndpoint(IStoriesMarketplaceService marketplaceService, IUserContextService userContext)
    {
        _marketplaceService = marketplaceService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/purchase")]
    [Authorize]
    public static async Task<Results<Ok<PurchaseStoryResponse>, BadRequest<PurchaseStoryResponse>, UnauthorizedHttpResult>> HandlePurchaseStory(
        [FromRoute] string locale,
        [FromBody] PurchaseStoryRequest request,
        [FromServices] PurchaseStoryEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var result = await ep._marketplaceService.PurchaseStoryAsync(userId.Value, request);
        
        if (result.Success)
        {
            return TypedResults.Ok(result);
        }
        else
        {
            return TypedResults.BadRequest(result);
        }
    }
}


