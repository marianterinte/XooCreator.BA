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
public class ListTreeOfHeroesConfigDefinitionsEndpoint
{
    private readonly ITreeOfHeroesConfigCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ListTreeOfHeroesConfigDefinitionsEndpoint> _logger;

    public ListTreeOfHeroesConfigDefinitionsEndpoint(
        ITreeOfHeroesConfigCraftService service,
        IAuth0UserService auth0,
        ILogger<ListTreeOfHeroesConfigDefinitionsEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/tree-configs/definitions")]
    [Authorize]
    public static async Task<Results<Ok<ListTreeOfHeroesConfigDefinitionsResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromQuery] string? status,
        [FromServices] ListTreeOfHeroesConfigDefinitionsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("ListTreeOfHeroesConfigDefinitions forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        var response = await ep._service.ListDefinitionsAsync(status, ct);
        return TypedResults.Ok(response);
    }
}
