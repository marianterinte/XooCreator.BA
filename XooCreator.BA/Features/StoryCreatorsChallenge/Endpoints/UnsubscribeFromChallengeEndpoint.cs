using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

[Endpoint]
public class UnsubscribeFromChallengeEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;
    private readonly IAuth0UserService _auth0;

    public UnsubscribeFromChallengeEndpoint(IStoryCreatorsChallengeService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/ccc/challenges/{challengeId}/unsubscribe")]
    [Authorize]
    public static async Task<Results<NoContent, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] UnsubscribeFromChallengeEndpoint ep,
        [FromRoute] string challengeId,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var result = await ep._service.UnsubscribeFromChallengeAsync(challengeId, user.Id, ct);
        if (!result) return TypedResults.BadRequest("Not subscribed or challenge not found.");

        return TypedResults.NoContent();
    }
}
