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
public class UpdateTreeOfHeroesConfigCraftEndpoint
{
    private readonly ITreeOfHeroesConfigCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UpdateTreeOfHeroesConfigCraftEndpoint> _logger;

    public UpdateTreeOfHeroesConfigCraftEndpoint(
        ITreeOfHeroesConfigCraftService service,
        IAuth0UserService auth0,
        ILogger<UpdateTreeOfHeroesConfigCraftEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/tree-configs/crafts/{id}")]
    [Authorize]
    public static async Task<Results<Ok<TreeOfHeroesConfigCraftDto>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
        [FromRoute] Guid id,
        [FromServices] UpdateTreeOfHeroesConfigCraftEndpoint ep,
        [FromBody] UpdateTreeOfHeroesConfigCraftRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("UpdateTreeOfHeroesConfigCraft forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var config = await ep._service.UpdateCraftAsync(user.Id, id, req, ct);
            return TypedResults.Ok(config);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
