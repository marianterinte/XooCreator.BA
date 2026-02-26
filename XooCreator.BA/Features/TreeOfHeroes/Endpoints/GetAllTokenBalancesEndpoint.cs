using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfHeroes.Services;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetAllTokenBalancesEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;

    public GetAllTokenBalancesEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-heroes/tokens/all")]
    [Authorize]
    public static async Task<Results<Ok<List<TokenBalanceItemDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetAllTokenBalancesEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var list = await ep._service.GetAllTokenBalancesAsync(userId.Value);
        return TypedResults.Ok(list);
    }
}
