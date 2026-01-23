using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryCreatorsChallenge.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Endpoints;

[Endpoint]
public class DeleteChallengeEndpoint
{
    private readonly IStoryCreatorsChallengeService _service;

    public DeleteChallengeEndpoint(IStoryCreatorsChallengeService service)
    {
        _service = service;
    }

    [Route("/api/admin/ccc/challenges/{challengeId}")]
    [Authorize]
    public static async Task<Results<NoContent, NotFound>> HandleDelete(
        [FromServices] DeleteChallengeEndpoint ep,
        [FromRoute] string challengeId,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(challengeId))
        {
            return TypedResults.NotFound();
        }

        var result = await ep._service.DeleteChallengeAsync(challengeId, ct);

        if (!result)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }
}
