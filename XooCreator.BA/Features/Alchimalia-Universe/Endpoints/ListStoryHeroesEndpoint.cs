using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class ListStoryHeroesEndpoint
{
    private readonly IStoryHeroService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ListStoryHeroesEndpoint> _logger;

    public ListStoryHeroesEndpoint(
        IStoryHeroService service,
        IAuth0UserService auth0,
        ILogger<ListStoryHeroesEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/story-heroes")]
    [Authorize]
    public static async Task<Results<Ok<ListStoryHeroesResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? status,
        [FromQuery] string? search,
        [FromQuery] string? language,
        [FromServices] ListStoryHeroesEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("ListStoryHeroes forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        var response = await ep._service.ListAsync(status, search, language, ct);
        return TypedResults.Ok(response);
    }
}
