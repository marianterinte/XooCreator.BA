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
public class CreateAnimalCraftFromDefinitionEndpoint
{
    private readonly IAnimalCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateAnimalCraftFromDefinitionEndpoint> _logger;

    public CreateAnimalCraftFromDefinitionEndpoint(
        IAnimalCraftService service,
        IAuth0UserService auth0,
        ILogger<CreateAnimalCraftFromDefinitionEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/animal-crafts/from-definition/{definitionId}")]
    [Authorize]
    public static async Task<Results<Ok<AnimalCraftDto>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] Guid definitionId,
        [FromServices] CreateAnimalCraftFromDefinitionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateAnimalCraftFromDefinition forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var animal = await ep._service.CreateCraftFromDefinitionAsync(user.Id, definitionId, ct);
            return TypedResults.Ok(animal);
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
