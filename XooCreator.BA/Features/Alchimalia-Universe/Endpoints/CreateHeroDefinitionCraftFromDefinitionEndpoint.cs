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
public class CreateHeroDefinitionCraftFromDefinitionEndpoint
{
    private readonly IHeroDefinitionCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateHeroDefinitionCraftFromDefinitionEndpoint> _logger;

    public CreateHeroDefinitionCraftFromDefinitionEndpoint(
        IHeroDefinitionCraftService service,
        IAuth0UserService auth0,
        ILogger<CreateHeroDefinitionCraftFromDefinitionEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/hero-definitions/from-definition/{definitionId}")]
    [Authorize]
    public static async Task<Results<Ok<HeroDefinitionCraftDto>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string definitionId,
        [FromServices] CreateHeroDefinitionCraftFromDefinitionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateHeroDefinitionCraftFromDefinition forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var hero = await ep._service.CreateCraftFromDefinitionAsync(user.Id, definitionId, ct);
            return TypedResults.Ok(hero);
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
