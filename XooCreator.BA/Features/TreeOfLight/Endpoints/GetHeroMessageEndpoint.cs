using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class GetHeroMessageEndpoint
{
    private readonly ITreeOfLightService _treeOfLightService;
    private readonly IUserContextService _userContext;

    public GetHeroMessageEndpoint(ITreeOfLightService treeOfLightService, IUserContextService userContext)
    {
        _treeOfLightService = treeOfLightService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-light/hero-message/{heroId}/{regionId}")] // GET /api/{locale}/tree-of-light/hero-message/{heroId}/{regionId}
    [Authorize]
    public static async Task<Results<Ok<HeroMessageDto>, UnauthorizedHttpResult, NotFound>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetHeroMessageEndpoint ep,
        [FromRoute] string heroId,
        [FromRoute] string regionId)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var result = await ep._treeOfLightService.GetHeroMessageAsync(heroId, regionId);
        if (result == null) return TypedResults.NotFound();

        return TypedResults.Ok(result);
    }
}
