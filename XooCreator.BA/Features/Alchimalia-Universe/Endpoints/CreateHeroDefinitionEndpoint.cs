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
public class CreateHeroDefinitionEndpoint
{
    private readonly IHeroDefinitionService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateHeroDefinitionEndpoint> _logger;

    public CreateHeroDefinitionEndpoint(
        IHeroDefinitionService service,
        IAuth0UserService auth0,
        ILogger<CreateHeroDefinitionEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/hero-definitions")]
    [Authorize]
    public static async Task<Results<Ok<HeroDefinitionDto>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] CreateHeroDefinitionEndpoint ep,
        [FromBody] CreateHeroDefinitionRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateHeroDefinition forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var hero = await ep._service.CreateAsync(user.Id, req, ct);
            ep._logger.LogInformation("HeroDefinition created: {HeroId} by {UserId}", hero.Id, user.Id);
            return TypedResults.Ok(hero);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
