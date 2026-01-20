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
public class GetAnimalEndpoint
{
    private readonly IAnimalService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetAnimalEndpoint> _logger;

    public GetAnimalEndpoint(
        IAnimalService service,
        IAuth0UserService auth0,
        ILogger<GetAnimalEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/animals/{animalId}")]
    [Authorize]
    public static async Task<Results<Ok<AnimalDto>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] Guid animalId,
        [FromQuery] string? language,
        [FromServices] GetAnimalEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("GetAnimal forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var animal = await ep._service.GetAsync(animalId, language, ct);
            return TypedResults.Ok(animal);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
