using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class TransformToHeroEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;
    public TransformToHeroEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-heroes/transform-hero")] // POST
    public static async Task<Results<Ok<TransformToHeroResponse>, BadRequest<TransformToHeroResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] TransformToHeroEndpoint ep,
        [FromBody] TransformToHeroRequest request)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var result = await ep._service.TransformToHeroAsync(userId.Value, request);
        return result.Success ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}
