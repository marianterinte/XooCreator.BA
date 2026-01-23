using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

[Endpoint]
public class DetermineWinnerEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;

    public DetermineWinnerEndpoint(IStoryCreatorsChallengeService service)
    {
        _service = service;
    }

    [Route("/api/{locale}/ccc/challenges/{challengeId}/determine-winner")]
    [Authorize(Roles = "Admin")]
    public static async Task<Results<Ok, BadRequest<string>>> HandlePost(
        [FromServices] DetermineWinnerEndpoint ep,
        [FromRoute] string challengeId,
        CancellationToken ct)
    {
        try
        {
            await ep._service.DetermineWinnerAsync(challengeId, ct);
            return TypedResults.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}


