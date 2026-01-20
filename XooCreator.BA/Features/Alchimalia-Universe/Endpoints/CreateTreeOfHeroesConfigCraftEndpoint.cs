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
public class CreateTreeOfHeroesConfigCraftEndpoint
{
    private readonly ITreeOfHeroesConfigCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateTreeOfHeroesConfigCraftEndpoint> _logger;

    public CreateTreeOfHeroesConfigCraftEndpoint(
        ITreeOfHeroesConfigCraftService service,
        IAuth0UserService auth0,
        ILogger<CreateTreeOfHeroesConfigCraftEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/tree-configs/crafts")]
    [Authorize]
    public static async Task<Results<Ok<TreeOfHeroesConfigCraftDto>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] CreateTreeOfHeroesConfigCraftEndpoint ep,
        [FromBody] CreateTreeOfHeroesConfigCraftRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateTreeOfHeroesConfigCraft forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var config = await ep._service.CreateCraftAsync(user.Id, req, ct);
            return TypedResults.Ok(config);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
