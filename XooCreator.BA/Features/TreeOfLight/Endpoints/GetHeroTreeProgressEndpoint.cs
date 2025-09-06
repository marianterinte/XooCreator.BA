using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class GetHeroTreeProgressEndpoint
{
    private readonly ITreeOfLightService _service;
    private readonly IUserContextService _userContext;
    public GetHeroTreeProgressEndpoint(ITreeOfLightService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/tree-of-light/hero-tree")] // GET
    public static async Task<Results<Ok<List<HeroTreeNodeDto>>, UnauthorizedHttpResult>> HandleGet([FromServices] GetHeroTreeProgressEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var nodes = await ep._service.GetHeroTreeProgressAsync(userId.Value);
        return TypedResults.Ok(nodes);
    }
}
