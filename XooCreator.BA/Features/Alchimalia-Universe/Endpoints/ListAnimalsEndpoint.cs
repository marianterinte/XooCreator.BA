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
public class ListAnimalsEndpoint
{
    private readonly IAnimalService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ListAnimalsEndpoint> _logger;

    public ListAnimalsEndpoint(
        IAnimalService service,
        IAuth0UserService auth0,
        ILogger<ListAnimalsEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/animals")]
    [Authorize]
    public static async Task<Results<Ok<ListAnimalsResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? status,
        [FromQuery] Guid? regionId,
        [FromQuery] bool? isHybrid,
        [FromQuery] string? search,
        [FromServices] ListAnimalsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("ListAnimals forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        var response = await ep._service.ListAsync(status, regionId, isHybrid, search, ct);
        return TypedResults.Ok(response);
    }
}
