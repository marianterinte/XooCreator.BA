using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfHeroes.Services;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetUserTokensEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;
    public GetUserTokensEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-heroes/tokens")] // GET
    [Authorize]
    public static async Task<Results<Ok<UserTokensDto>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetUserTokensEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var tokens = await ep._service.GetUserTokensAsync(userId.Value);
        return TypedResults.Ok(tokens);
    }
}
