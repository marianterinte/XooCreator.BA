using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

public class SetWinnerManuallyRequest
{
    public string StoryId { get; set; } = string.Empty;
}

[Endpoint]
public class SetWinnerManuallyEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;

    public SetWinnerManuallyEndpoint(IStoryCreatorsChallengeService service)
    {
        _service = service;
    }

    [Route("/api/{locale}/ccc/challenges/{challengeId}/set-winner")]
    [Authorize(Roles = "Admin")]
    public static async Task<Results<Ok, BadRequest<string>>> HandlePost(
        [FromServices] SetWinnerManuallyEndpoint ep,
        [FromRoute] string challengeId,
        [FromBody] SetWinnerManuallyRequest req,
        CancellationToken ct)
    {
        try
        {
            await ep._service.SetWinnerManuallyAsync(challengeId, req.StoryId, ct);
            return TypedResults.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}


