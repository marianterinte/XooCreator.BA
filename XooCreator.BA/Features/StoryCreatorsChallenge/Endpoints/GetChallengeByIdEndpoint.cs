using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.DTOs;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

[Endpoint]
public class GetChallengeByIdEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;

    public GetChallengeByIdEndpoint(IStoryCreatorsChallengeService service)
    {
        _service = service;
    }

    [Route("/api/admin/ccc/challenges/{challengeId}")]
    [Authorize]
    public static async Task<Results<Ok<StoryCreatorsChallengeDto>, NotFound>> HandleGet(
        [FromServices] GetChallengeByIdEndpoint ep,
        [FromRoute] string challengeId,
        [FromQuery] string? locale,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(challengeId))
        {
            return TypedResults.NotFound();
        }

        var challenge = await ep._service.GetChallengeByIdAsync(challengeId, locale, ct);

        if (challenge == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(challenge);
    }
}
