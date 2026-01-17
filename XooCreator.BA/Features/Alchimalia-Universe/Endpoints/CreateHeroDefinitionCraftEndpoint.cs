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
public class CreateHeroDefinitionCraftEndpoint
{
    private readonly IHeroDefinitionCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateHeroDefinitionCraftEndpoint> _logger;

    public CreateHeroDefinitionCraftEndpoint(
        IHeroDefinitionCraftService service,
        IAuth0UserService auth0,
        ILogger<CreateHeroDefinitionCraftEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/hero-crafts")]
    [Authorize]
    public static async Task<Results<Ok<HeroDefinitionCraftDto>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] CreateHeroDefinitionCraftEndpoint ep,
        [FromBody] CreateHeroDefinitionCraftRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateHeroDefinitionCraft forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var hero = await ep._service.CreateAsync(user.Id, req, ct);
            return TypedResults.Ok(hero);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
