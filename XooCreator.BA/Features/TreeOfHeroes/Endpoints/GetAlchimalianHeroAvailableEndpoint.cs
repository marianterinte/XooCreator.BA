using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfHeroes.Services;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetAlchimalianHeroAvailableEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;

    public GetAlchimalianHeroAvailableEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-heroes/alchimalian-hero/available")]
    [Authorize]
    public static async Task<Results<Ok<AlchimalianHeroAvailableResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetAlchimalianHeroAvailableEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var response = await ep._service.GetAlchimalianHeroAvailableAsync(userId.Value, locale);
        return TypedResults.Ok(response);
    }
}
