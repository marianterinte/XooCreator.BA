using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class UnlockTreeOfHeroesNodeEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;
    public UnlockTreeOfHeroesNodeEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/tree-of-heroes/unlock-node")] // POST
    public static async Task<Results<Ok<UnlockHeroTreeNodeResponse>, BadRequest<UnlockHeroTreeNodeResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] UnlockTreeOfHeroesNodeEndpoint ep,
        [FromBody] UnlockHeroTreeNodeRequest request)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var result = await ep._service.UnlockHeroTreeNodeAsync(userId.Value, request);
        return result.Success ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}
