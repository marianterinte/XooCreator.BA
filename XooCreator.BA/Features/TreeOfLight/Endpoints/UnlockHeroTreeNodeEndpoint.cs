using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class UnlockHeroTreeNodeEndpoint
{
    private readonly ITreeOfLightService _service;
    private readonly IUserContextService _userContext;
    public UnlockHeroTreeNodeEndpoint(ITreeOfLightService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/tree-of-light/unlock-hero-tree-node")] // POST
    public static async Task<Results<Ok<UnlockHeroTreeNodeResponse>, BadRequest<UnlockHeroTreeNodeResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] UnlockHeroTreeNodeEndpoint ep,
        [FromBody] UnlockHeroTreeNodeRequest request)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var result = await ep._service.UnlockHeroTreeNodeAsync(userId.Value, request);
        return result.Success ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}
