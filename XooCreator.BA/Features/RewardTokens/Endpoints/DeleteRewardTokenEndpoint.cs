using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.RewardTokens.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.RewardTokens.Endpoints;

[Endpoint]
public class DeleteRewardTokenEndpoint
{
    private readonly IRewardTokensService _service;
    private readonly IAuth0UserService _auth0;

    public DeleteRewardTokenEndpoint(IRewardTokensService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/admin/reward-tokens/{id:guid}")]
    [Authorize]
    public static async Task<Results<Ok<bool>, NotFound, ForbidHttpResult>> HandleDelete(
        [FromServices] DeleteRewardTokenEndpoint ep,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null || !user.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var deleted = await ep._service.DeleteAsync(id, ct);
        return deleted ? TypedResults.Ok(true) : TypedResults.NotFound();
    }
}
