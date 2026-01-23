using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

[Endpoint]
public class RemoveSubmissionEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;
    private readonly IAuth0UserService _auth0;

    public RemoveSubmissionEndpoint(IStoryCreatorsChallengeService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/ccc/challenges/{challengeId}/submissions/{storyId}")]
    [Authorize]
    public static async Task<Results<NoContent, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandleDelete(
        [FromServices] RemoveSubmissionEndpoint ep,
        [FromRoute] string challengeId,
        [FromRoute] string storyId,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        try
        {
            var result = await ep._service.RemoveSubmissionAsync(challengeId, storyId, user.Id, ct);
            if (!result) return TypedResults.BadRequest("Submission not found.");
            
            return TypedResults.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }
}
