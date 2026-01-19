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
public class ListAnimalCraftsEndpoint
{
    private readonly IAnimalCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ListAnimalCraftsEndpoint> _logger;

    public ListAnimalCraftsEndpoint(
        IAnimalCraftService service,
        IAuth0UserService auth0,
        ILogger<ListAnimalCraftsEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/animal-crafts")]
    [Authorize]
    public static async Task<Results<Ok<ListAnimalCraftsResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromQuery] string? status,
        [FromQuery] Guid? regionId,
        [FromQuery] bool? isHybrid,
        [FromQuery] string? search,
        [FromQuery] string? language,
        [FromServices] ListAnimalCraftsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) &&
            !ep._auth0.HasRole(user, UserRole.Reviewer) &&
            !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("ListAnimalCrafts forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        var response = await ep._service.ListAsync(user.Id, status, regionId, isHybrid, search, language, ct);
        return TypedResults.Ok(response);
    }
}
