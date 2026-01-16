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
public class UpdateAnimalEndpoint
{
    private readonly IAnimalService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UpdateAnimalEndpoint> _logger;

    public UpdateAnimalEndpoint(
        IAnimalService service,
        IAuth0UserService auth0,
        ILogger<UpdateAnimalEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/animals/{animalId}")]
    [Authorize]
    public static async Task<Results<Ok<AnimalDto>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
        [FromRoute] string locale,
        [FromRoute] Guid animalId,
        [FromServices] UpdateAnimalEndpoint ep,
        [FromBody] UpdateAnimalRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("UpdateAnimal forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var animal = await ep._service.UpdateAsync(user.Id, animalId, req, ct);
            return TypedResults.Ok(animal);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
