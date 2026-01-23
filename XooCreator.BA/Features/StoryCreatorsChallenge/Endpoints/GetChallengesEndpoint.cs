using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.DTOs;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

[Endpoint]
public class GetChallengesEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;

    public GetChallengesEndpoint(IStoryCreatorsChallengeService service)
    {
        _service = service;
    }

    [Route("/api/admin/ccc/challenges")]
    [Authorize]
    public static async Task<Ok<List<StoryCreatorsChallengeListItemDto>>> HandleGet(
        [FromServices] GetChallengesEndpoint ep,
        [FromQuery] string? locale,
        CancellationToken ct)
    {
        var challenges = await ep._service.GetAllChallengesAsync(locale, ct);
        return TypedResults.Ok(challenges);
    }
}
