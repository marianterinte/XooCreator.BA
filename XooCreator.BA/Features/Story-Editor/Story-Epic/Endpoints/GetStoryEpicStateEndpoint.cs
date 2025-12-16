using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class GetStoryEpicStateEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetStoryEpicStateEndpoint> _logger;

    public GetStoryEpicStateEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        ILogger<GetStoryEpicStateEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _logger = logger;
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics/{epicId}/state")]
    [Authorize]
    public static async Task<Results<Ok<StoryEpicStateDto>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string epicId,
        [FromServices] GetStoryEpicStateEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var state = await ep._epicService.GetEpicStateAsync(epicId, ct);
        if (state == null)
        {
            ep._logger.LogWarning("GetStoryEpicState: Epic not found epicId={EpicId} userId={UserId}", epicId, user.Id);
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(state);
    }
}

