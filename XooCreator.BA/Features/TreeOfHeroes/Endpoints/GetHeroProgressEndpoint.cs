using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetHeroProgressEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;
    public GetHeroProgressEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-heroes/heroes")] // GET
    public static async Task<Results<Ok<List<HeroDto>>, UnauthorizedHttpResult>> HandleGet([FromServices] GetHeroProgressEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var heroes = await ep._service.GetHeroProgressAsync(userId.Value);
        return TypedResults.Ok(heroes);
    }
}
