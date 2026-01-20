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
public class CreateAnimalEndpoint
{
    private readonly IAnimalService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateAnimalEndpoint> _logger;

    public CreateAnimalEndpoint(
        IAnimalService service,
        IAuth0UserService auth0,
        ILogger<CreateAnimalEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/animals")]
    [Authorize]
    public static async Task<Results<Ok<AnimalDto>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] CreateAnimalEndpoint ep,
        [FromBody] CreateAnimalRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateAnimal forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var animal = await ep._service.CreateAsync(user.Id, req, ct);
            ep._logger.LogInformation("Animal created: {AnimalId} by {UserId}", animal.Id, user.Id);
            return TypedResults.Ok(animal);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
