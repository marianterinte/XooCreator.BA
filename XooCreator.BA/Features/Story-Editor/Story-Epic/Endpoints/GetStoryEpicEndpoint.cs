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
public class GetStoryEpicEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetStoryEpicEndpoint> _logger;

    public GetStoryEpicEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        ILogger<GetStoryEpicEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _logger = logger;
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics/{epicId}")]
    [Authorize]
    public static async Task<Results<Ok<StoryEpicDto>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string epicId,
        [FromServices] GetStoryEpicEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var epic = await ep._epicService.GetEpicAsync(epicId, ct);
        if (epic == null)
        {
            ep._logger.LogWarning("GetStoryEpic: Epic not found epicId={EpicId} userId={UserId}", epicId, user.Id);
            return TypedResults.NotFound();
        }

        // TODO: Add ownership check if needed (only owner can view draft epics, but published can be viewed by anyone)
        // For now, allow any authenticated user to view

        return TypedResults.Ok(epic);
    }
}

