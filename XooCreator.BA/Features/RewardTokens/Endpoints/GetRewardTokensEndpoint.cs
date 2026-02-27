using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.RewardTokens.DTOs;
using XooCreator.BA.Features.RewardTokens.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.RewardTokens.Endpoints;

[Endpoint]
public class GetRewardTokensEndpoint
{
    private readonly IRewardTokensService _service;

    public GetRewardTokensEndpoint(IRewardTokensService service)
    {
        _service = service;
    }

    [Route("/api/reward-tokens")]
    [Authorize]
    public static async Task<Ok<List<RewardTokenDto>>> HandleGet(
        [FromServices] GetRewardTokensEndpoint ep,
        CancellationToken ct)
    {
        var tokens = await ep._service.GetActiveTokensAsync(ct);
        return TypedResults.Ok(tokens);
    }
}
