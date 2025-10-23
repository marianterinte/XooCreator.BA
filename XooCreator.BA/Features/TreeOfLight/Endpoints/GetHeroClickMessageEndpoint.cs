using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class GetHeroClickMessageEndpoint
{
    private readonly ITreeOfLightService _treeOfLightService;
    private readonly IUserContextService _userContext;

    public GetHeroClickMessageEndpoint(ITreeOfLightService treeOfLightService, IUserContextService userContext)
    {
        _treeOfLightService = treeOfLightService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-light/hero-click-message/{heroId}")] // GET /api/{locale}/tree-of-light/hero-click-message/{heroId}
    [Authorize]
    public static async Task<Results<Ok<HeroClickMessageDto>, UnauthorizedHttpResult, NotFound>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetHeroClickMessageEndpoint ep,
        [FromRoute] string heroId)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var result = await ep._treeOfLightService.GetHeroClickMessageAsync(heroId);
        if (result == null) return TypedResults.NotFound();

        return TypedResults.Ok(result);
    }
}
