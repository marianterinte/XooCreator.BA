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
public class GetUserSubmissionsEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;
    private readonly IAuth0UserService _auth0;

    public GetUserSubmissionsEndpoint(IStoryCreatorsChallengeService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/ccc/my-submissions/{challengeId}")]
    [Authorize]
    public static async Task<Results<Ok<List<ChallengeSubmissionDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string challengeId,
        [FromServices] GetUserSubmissionsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var result = await ep._service.GetUserSubmissionsAsync(challengeId, user.Id, ct);
        return TypedResults.Ok(result);
    }
}
