using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfHeroes.Services;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetAlchimalianHeroProfileEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;

    public GetAlchimalianHeroProfileEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-heroes/alchimalian-hero/profile")]
    [Authorize]
    public static async Task<Results<Ok<AlchimalianHeroProfileDto>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetAlchimalianHeroProfileEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var profile = await ep._service.GetAlchimalianHeroProfileAsync(userId.Value, locale);
        return TypedResults.Ok(profile);
    }
}
