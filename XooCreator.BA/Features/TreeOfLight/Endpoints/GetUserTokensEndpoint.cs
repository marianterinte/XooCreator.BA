using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class GetUserTokensEndpoint
{
    private readonly ITreeOfLightService _service;
    private readonly IUserContextService _userContext;
    public GetUserTokensEndpoint(ITreeOfLightService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/tree-of-light/tokens")] // GET
    public static async Task<Results<Ok<UserTokensDto>, UnauthorizedHttpResult>> HandleGet([FromServices] GetUserTokensEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var tokens = await ep._service.GetUserTokensAsync(userId.Value);
        return TypedResults.Ok(tokens);
    }
}
