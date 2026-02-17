using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfHeroes.Services;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class UpdateAlchimalianHeroProfileEndpoint
{
    private readonly ITreeOfHeroesService _service;
    private readonly IUserContextService _userContext;

    public UpdateAlchimalianHeroProfileEndpoint(ITreeOfHeroesService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-heroes/alchimalian-hero/profile")]
    [Authorize]
    public static async Task<Results<Ok<UpdateAlchimalianHeroProfileResponse>, BadRequest<UpdateAlchimalianHeroProfileResponse>, UnauthorizedHttpResult>> HandlePut(
        [FromRoute] string locale,
        [FromBody] UpdateAlchimalianHeroProfileRequest request,
        [FromServices] UpdateAlchimalianHeroProfileEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var response = await ep._service.UpdateAlchimalianHeroProfileAsync(userId.Value, request ?? new UpdateAlchimalianHeroProfileRequest(), locale);
        if (!response.Success)
            return TypedResults.BadRequest(response);
        return TypedResults.Ok(response);
    }
}
