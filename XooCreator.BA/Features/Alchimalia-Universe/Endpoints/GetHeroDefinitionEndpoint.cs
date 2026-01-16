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
public class GetHeroDefinitionEndpoint
{
    private readonly IHeroDefinitionService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetHeroDefinitionEndpoint> _logger;

    public GetHeroDefinitionEndpoint(
        IHeroDefinitionService service,
        IAuth0UserService auth0,
        ILogger<GetHeroDefinitionEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/hero-definitions/{heroId}")]
    [Authorize]
    public static async Task<Results<Ok<HeroDefinitionDto>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string heroId,
        [FromQuery] string? language,
        [FromServices] GetHeroDefinitionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("GetHeroDefinition forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var hero = await ep._service.GetAsync(heroId, language, ct);
            return TypedResults.Ok(hero);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
