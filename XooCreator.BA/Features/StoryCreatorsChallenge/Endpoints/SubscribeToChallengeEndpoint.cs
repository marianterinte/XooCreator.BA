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
public class SubscribeToChallengeEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;
    private readonly IAuth0UserService _auth0;

    public SubscribeToChallengeEndpoint(IStoryCreatorsChallengeService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/ccc/challenges/{challengeId}/subscribe")]
    [Authorize]
    public static async Task<Results<Ok<ChallengeSubscriptionDto>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] SubscribeToChallengeEndpoint ep,
        [FromRoute] string challengeId,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        try
        {
            var result = await ep._service.SubscribeToChallengeAsync(challengeId, user.Id, ct);
            return TypedResults.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
