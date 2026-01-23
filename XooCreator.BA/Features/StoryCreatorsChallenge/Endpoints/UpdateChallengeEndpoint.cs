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
public class UpdateChallengeEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;
    private readonly IAuth0UserService _auth0; 

    public UpdateChallengeEndpoint(IStoryCreatorsChallengeService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/admin/ccc/challenges/{challengeId}")]
    [Authorize]
    public static async Task<Results<Ok<StoryCreatorsChallengeDto>, BadRequest<string>, UnauthorizedHttpResult>> HandlePut(
        [FromServices] UpdateChallengeEndpoint ep,
        [FromRoute] string challengeId,
        [FromBody] StoryCreatorsChallengeDto req,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(challengeId))
        {
             return TypedResults.BadRequest("ChallengeId is required");
        }
        
        if (req.ChallengeId != challengeId)
        {
             return TypedResults.BadRequest("ChallengeId mismatch");
        }

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();
        
        var updated = await ep._service.UpdateChallengeAsync(challengeId, req, user.Id, ct);
        return TypedResults.Ok(updated);
    }
}
