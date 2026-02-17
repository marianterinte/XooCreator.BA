using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfHeroes.Services;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class DiscoverAlchimalianHeroEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;

    public DiscoverAlchimalianHeroEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-heroes/alchimalian-hero/discover")]
    [Authorize]
    public static async Task<Results<Ok<DiscoverAlchimalianHeroResponse>, BadRequest<DiscoverAlchimalianHeroResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] DiscoverAlchimalianHeroRequest request,
        [FromServices] DiscoverAlchimalianHeroEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        if (request == null)
            return TypedResults.BadRequest(new DiscoverAlchimalianHeroResponse { Success = false, ErrorMessage = "Request body is required" });
        var response = await ep._service.DiscoverAlchimalianHeroAsync(userId.Value, request, locale);
        if (!response.Success)
            return TypedResults.BadRequest(response);
        return TypedResults.Ok(response);
    }
}
