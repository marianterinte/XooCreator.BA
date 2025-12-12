using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class GetStoryEpicStateWithProgressEndpoint
{
    private readonly IStoryEpicProgressService _progressService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetStoryEpicStateWithProgressEndpoint> _logger;

    public GetStoryEpicStateWithProgressEndpoint(
        IStoryEpicProgressService progressService,
        IAuth0UserService auth0,
        ILogger<GetStoryEpicStateWithProgressEndpoint> logger)
    {
        _progressService = progressService;
        _auth0 = auth0;
        _logger = logger;
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics/{epicId}/state-with-progress")]
    [Authorize]
    public static async Task<Results<Ok<StoryEpicStateWithProgressDto>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string epicId,
        [FromServices] GetStoryEpicStateWithProgressEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var state = await ep._progressService.GetEpicStateWithProgressAsync(epicId, user.Id, ct);
        if (state == null)
        {
            ep._logger.LogWarning("GetStoryEpicStateWithProgress: Epic not found epicId={EpicId} userId={UserId}", epicId, user.Id);
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(state);
    }
}

