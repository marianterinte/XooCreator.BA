using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.User.DTOs;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.User.Endpoints;

[Endpoint]
public class OverrideCreatorTokenEndpoint
{
    private readonly ICreatorTokenService _tokenService;
    private readonly Auth0UserService _auth0UserService;

    public OverrideCreatorTokenEndpoint(ICreatorTokenService tokenService, Auth0UserService auth0UserService)
    {
        _tokenService = tokenService;
        _auth0UserService = auth0UserService;
    }

    [Route("/api/admin/creator-tokens/{userId}/override")]
    [Authorize]
    public static async Task<Ok<CreatorTokenBalanceDto>> HandleOverride(
        [FromRoute] Guid userId,
        [FromBody] OverrideCreatorTokenRequest request,
        [FromServices] OverrideCreatorTokenEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0UserService.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
        {
            throw new UnauthorizedAccessException("Admin access required");
        }

        var result = await ep._tokenService.OverrideTokenAsync(userId, request, admin.Id, ct);
        return TypedResults.Ok(result);
    }
}
