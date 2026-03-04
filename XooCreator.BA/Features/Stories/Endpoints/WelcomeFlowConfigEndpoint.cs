using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Stories.DTOs;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.Stories.Endpoints;

/// <summary>
/// Public endpoint for Welcome Flow config (no auth). Used by FE.
/// </summary>
[Endpoint]
public class WelcomeFlowConfigEndpoint
{
    private readonly IWelcomeFlowConfigService _configService;

    public WelcomeFlowConfigEndpoint(IWelcomeFlowConfigService configService)
    {
        _configService = configService;
    }

    [Route("/api/{locale}/stories/public/welcome-flow-config")]
    public static async Task<Results<Ok<WelcomeFlowConfigDto>, BadRequest<string>>> HandleGet(
        [FromRoute] string locale,
        [FromServices] WelcomeFlowConfigEndpoint ep,
        CancellationToken ct)
    {
        var dto = await ep._configService.GetConfigDtoAsync(ct);
        return TypedResults.Ok(dto);
    }
}

/// <summary>
/// Admin endpoints for Welcome Flow config (GET/PUT).
/// </summary>
[Endpoint]
public class WelcomeFlowConfigAdminEndpoint
{
    private readonly IWelcomeFlowConfigService _configService;
    private readonly IAuth0UserService _auth0;

    public WelcomeFlowConfigAdminEndpoint(IWelcomeFlowConfigService configService, IAuth0UserService auth0)
    {
        _configService = configService;
        _auth0 = auth0;
    }

    [Route("/api/admin/welcome-flow-config")]
    [Authorize]
    public static async Task<Results<Ok<WelcomeFlowConfigDto>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromServices] WelcomeFlowConfigAdminEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();
        if (!ep._auth0.HasRole(user, UserRole.Admin))
            return TypedResults.Forbid();

        var dto = await ep._configService.GetConfigDtoAsync(ct);
        return TypedResults.Ok(dto);
    }

    [Route("/api/admin/welcome-flow-config")]
    [Authorize]
    public static async Task<Results<Ok<WelcomeFlowConfigDto>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
        [FromBody] WelcomeFlowConfigDto dto,
        [FromServices] WelcomeFlowConfigAdminEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();
        if (!ep._auth0.HasRole(user, UserRole.Admin))
            return TypedResults.Forbid();

        if (dto == null)
            return TypedResults.BadRequest("Request body is required.");
        if (string.IsNullOrWhiteSpace(dto.EntryPointStoryId))
            return TypedResults.BadRequest("EntryPointStoryId is required.");

        var updated = await ep._configService.UpdateAsync(dto, ct);
        return TypedResults.Ok(updated);
    }
}
