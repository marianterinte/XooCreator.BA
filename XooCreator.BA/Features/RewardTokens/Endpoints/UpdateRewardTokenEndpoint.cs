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
public class UpdateRewardTokenEndpoint
{
    private readonly IRewardTokensService _service;
    private readonly IAuth0UserService _auth0;

    public UpdateRewardTokenEndpoint(IRewardTokensService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/admin/reward-tokens/{id:guid}")]
    [Authorize]
    public static async Task<Results<Ok<RewardTokenDto>, NotFound, ForbidHttpResult, BadRequest<string>>> HandlePut(
        [FromServices] UpdateRewardTokenEndpoint ep,
        [FromRoute] Guid id,
        [FromBody] RewardTokenCreateOrUpdateDto req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null || !user.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        if (string.IsNullOrWhiteSpace(req.Type) || string.IsNullOrWhiteSpace(req.Value) || string.IsNullOrWhiteSpace(req.DisplayNameKey))
            return TypedResults.BadRequest("Type, Value and DisplayNameKey are required.");

        var updated = await ep._service.UpdateAsync(id, req, ct);
        return updated == null ? TypedResults.NotFound() : TypedResults.Ok(updated);
    }
}
