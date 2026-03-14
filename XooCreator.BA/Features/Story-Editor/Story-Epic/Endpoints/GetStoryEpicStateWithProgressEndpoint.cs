using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Features.Subscription.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class GetStoryEpicStateWithProgressEndpoint
{
    private readonly IStoryEpicProgressService _progressService;
    private readonly IAuth0UserService _auth0;
    private readonly IExclusiveContentService _exclusiveContent;
    private readonly ILogger<GetStoryEpicStateWithProgressEndpoint> _logger;

    public GetStoryEpicStateWithProgressEndpoint(
        IStoryEpicProgressService progressService,
        IAuth0UserService auth0,
        IExclusiveContentService exclusiveContent,
        ILogger<GetStoryEpicStateWithProgressEndpoint> logger)
    {
        _progressService = progressService;
        _auth0 = auth0;
        _exclusiveContent = exclusiveContent;
        _logger = logger;
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics/{epicId}/state-with-progress")]
    [Authorize]
    public static async Task<Results<Ok<StoryEpicStateWithProgressDto>, NotFound, UnauthorizedHttpResult, ProblemHttpResult>> HandleGet(
        [FromRoute] string epicId,
        [FromServices] GetStoryEpicStateWithProgressEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isExclusive = await ep._exclusiveContent.IsEpicExclusiveAsync(epicId, ct);
        if (isExclusive && !await ep._exclusiveContent.HasAccessToEpicAsync(user.Id, epicId, ct))
            return TypedResults.Problem("Exclusive content. Supporter Pack required.", statusCode: StatusCodes.Status403Forbidden);

        var state = await ep._progressService.GetEpicStateWithProgressAsync(epicId, user.Id, ct);
        if (state == null)
        {
            ep._logger.LogWarning("GetStoryEpicStateWithProgress: Epic not found epicId={EpicId} userId={UserId}", epicId, user.Id);
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(state);
    }
}

