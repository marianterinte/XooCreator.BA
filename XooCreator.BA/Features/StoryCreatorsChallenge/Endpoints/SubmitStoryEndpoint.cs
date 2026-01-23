using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.DTOs;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

public record SubmitStoryRequest(string StoryId);

[Endpoint]
public class SubmitStoryEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;
    private readonly IAuth0UserService _auth0;

    public SubmitStoryEndpoint(IStoryCreatorsChallengeService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/ccc/challenges/{challengeId}/submit")]
    [Authorize]
    public static async Task<Results<Ok<ChallengeSubmissionDto>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] SubmitStoryEndpoint ep,
        [FromRoute] string challengeId,
        [FromBody] SubmitStoryRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        try
        {
            var result = await ep._service.SubmitStoryToChallengeAsync(challengeId, req.StoryId, user.Id, ct);
            return TypedResults.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
