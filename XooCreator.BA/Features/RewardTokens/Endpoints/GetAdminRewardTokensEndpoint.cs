using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.RewardTokens.DTOs;
using XooCreator.BA.Features.RewardTokens.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.RewardTokens.Endpoints;

[Endpoint]
public class GetAdminRewardTokensEndpoint
{
    private readonly IRewardTokensService _service;
    private readonly IAuth0UserService _auth0;

    public GetAdminRewardTokensEndpoint(IRewardTokensService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/admin/reward-tokens")]
    [Authorize]
    public static async Task<Results<Ok<List<RewardTokenDto>>, ForbidHttpResult>> HandleGet(
        [FromServices] GetAdminRewardTokensEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null || !user.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var tokens = await ep._service.GetAllTokensAsync(ct);
        return TypedResults.Ok(tokens);
    }
}
