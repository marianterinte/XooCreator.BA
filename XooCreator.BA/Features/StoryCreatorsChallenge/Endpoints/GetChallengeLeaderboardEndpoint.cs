using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.DTOs;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

[Endpoint]
public class GetChallengeLeaderboardEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;
    private readonly IAuth0UserService _auth0;

    public GetChallengeLeaderboardEndpoint(IStoryCreatorsChallengeService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/ccc/challenges/{challengeId}/leaderboard")]
    [AllowAnonymous]
    public static async Task<Results<Ok<ChallengeLeaderboardDto>, NotFound>> HandleGet(
        [FromServices] GetChallengeLeaderboardEndpoint ep,
        [FromRoute] string challengeId,
        CancellationToken ct)
    {
        // Try to get user if authenticated, but don't fail if not
        Guid? userId = null;
        try {
             var user = await ep._auth0.GetCurrentUserAsync(ct);
             userId = user?.Id;
        } catch { /* Ignore auth errors for public view */ }

        var result = await ep._service.GetChallengeLeaderboardAsync(challengeId, userId, ct);
        if (result == null) return TypedResults.NotFound(); // Should probably return empty leaderboard instead of null, but service returns new(), so just in case.

        return TypedResults.Ok(result);
    }
}
