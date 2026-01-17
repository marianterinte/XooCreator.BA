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
public class ListHeroDefinitionCraftsEndpoint
{
    private readonly IHeroDefinitionCraftService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ListHeroDefinitionCraftsEndpoint> _logger;

    public ListHeroDefinitionCraftsEndpoint(
        IHeroDefinitionCraftService service,
        IAuth0UserService auth0,
        ILogger<ListHeroDefinitionCraftsEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/hero-crafts")]
    [Authorize]
    public static async Task<Results<Ok<ListHeroDefinitionCraftsResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromQuery] string? status,
        [FromQuery] string? type,
        [FromQuery] string? search,
        [FromQuery] string? language,
        [FromServices] ListHeroDefinitionCraftsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("ListHeroDefinitionCrafts forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        var response = await ep._service.ListAsync(status, type, search, language, ct);
        return TypedResults.Ok(response);
    }
}
