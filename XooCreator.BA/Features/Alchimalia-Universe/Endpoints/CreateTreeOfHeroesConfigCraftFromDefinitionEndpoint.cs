using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class CreateTreeOfHeroesConfigCraftFromDefinitionEndpoint
{
    private readonly ITreeOfHeroesConfigCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateTreeOfHeroesConfigCraftFromDefinitionEndpoint> _logger;

    public CreateTreeOfHeroesConfigCraftFromDefinitionEndpoint(
        ITreeOfHeroesConfigCraftService service,
        IAuth0UserService auth0,
        ILogger<CreateTreeOfHeroesConfigCraftFromDefinitionEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    public record CreateCraftFromDefinitionResponse(string Id);

    [Route("/api/alchimalia-universe/tree-configs/from-definition/{definitionId}")]
    [Authorize]
    public static async Task<Results<Ok<CreateCraftFromDefinitionResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] Guid definitionId,
        [FromServices] CreateTreeOfHeroesConfigCraftFromDefinitionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("CreateTreeOfHeroesConfigCraftFromDefinition forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var allowAdminOverride = ep._auth0.HasRole(user, UserRole.Admin);
            var craft = await ep._service.CreateCraftFromDefinitionAsync(user.Id, definitionId, allowAdminOverride, ct);
            return TypedResults.Ok(new CreateCraftFromDefinitionResponse(craft.Id.ToString()));
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
