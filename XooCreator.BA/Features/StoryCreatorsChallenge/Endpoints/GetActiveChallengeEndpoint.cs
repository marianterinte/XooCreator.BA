using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.DTOs;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

[Endpoint]
public class GetActiveChallengeEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;

    public GetActiveChallengeEndpoint(IStoryCreatorsChallengeService service)
    {
        _service = service;
    }

    [Route("/api/{locale}/ccc/active")]
    [AllowAnonymous]
    public static async Task<Results<Ok<PublicChallengeDto>, NotFound>> HandleGet(
        [FromServices] GetActiveChallengeEndpoint ep,
        [FromRoute] string locale,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(locale))
        {
            locale = "ro-ro";
        }

        var challenge = await ep._service.GetActiveChallengeAsync(locale, ct);

        if (challenge == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(challenge);
    }
}
