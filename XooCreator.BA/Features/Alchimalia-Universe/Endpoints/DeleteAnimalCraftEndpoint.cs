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
public class DeleteAnimalCraftEndpoint
{
    private readonly IAnimalCraftService _animalService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<DeleteAnimalCraftEndpoint> _logger;

    public DeleteAnimalCraftEndpoint(
        IAnimalCraftService animalService,
        IAuth0UserService auth0,
        ILogger<DeleteAnimalCraftEndpoint> logger)
    {
        _animalService = animalService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/animal-crafts/{animalId}")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandleDelete(
        [FromRoute] Guid animalId,
        [FromServices] DeleteAnimalCraftEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        try
        {
            await ep._animalService.DeleteAsync(user.Id, animalId, ct);
            ep._logger.LogInformation("DeleteAnimalCraft: userId={UserId} animalId={AnimalId}", user.Id, animalId);
            return TypedResults.Ok();
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }
}
