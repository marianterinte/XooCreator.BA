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
public class UpdateAnimalCraftEndpoint
{
    private readonly IAnimalCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UpdateAnimalCraftEndpoint> _logger;

    public UpdateAnimalCraftEndpoint(
        IAnimalCraftService service,
        IAuth0UserService auth0,
        ILogger<UpdateAnimalCraftEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/animal-crafts/{animalId}")]
    [Authorize]
    public static async Task<Results<Ok<AnimalCraftDto>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>> HandlePut(
        [FromRoute] Guid animalId,
        [FromServices] UpdateAnimalCraftEndpoint ep,
        [FromBody] UpdateAnimalCraftRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("UpdateAnimalCraft forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var allowAdminOverride = ep._auth0.HasRole(user, UserRole.Admin);
            var animal = await ep._service.UpdateAsync(user.Id, animalId, req, allowAdminOverride, ct);
            return TypedResults.Ok(animal);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
