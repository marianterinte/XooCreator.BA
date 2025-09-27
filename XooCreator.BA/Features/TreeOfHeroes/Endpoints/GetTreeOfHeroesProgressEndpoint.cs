using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetTreeOfHeroesProgressEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;
    public GetTreeOfHeroesProgressEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-heroes")] // GET
    public static async Task<Results<Ok<List<HeroTreeNodeDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetTreeOfHeroesProgressEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var nodes = await ep._service.GetHeroTreeProgressAsync(userId.Value);
        return TypedResults.Ok(nodes);
    }
}
