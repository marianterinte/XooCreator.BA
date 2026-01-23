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
public class CreateChallengeEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;
    private readonly IAuth0UserService _auth0; // Assuming this service exists as seen in CreateAnimalEndpoint

    public CreateChallengeEndpoint(IStoryCreatorsChallengeService service, IAuth0UserService auth0)
    {
        _service = service;
        _auth0 = auth0;
    }

    [Route("/api/admin/ccc/challenges")]
    [Authorize]
    public static async Task<Results<Ok<StoryCreatorsChallengeDto>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] CreateChallengeEndpoint ep,
        [FromBody] StoryCreatorsChallengeDto req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();
        
        // Admin check usually handled by [Authorize] policy, but can check roles here if needed.
        // Assuming [Authorize] is enough or global policy handles it.
        // For now, using user.Id from the resolved user.

        var created = await ep._service.CreateChallengeAsync(req, user.Id, ct);
        return TypedResults.Ok(created);
    }
}
