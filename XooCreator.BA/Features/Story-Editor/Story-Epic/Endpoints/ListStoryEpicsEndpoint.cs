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
public class ListStoryEpicsEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ListStoryEpicsEndpoint> _logger;

    public ListStoryEpicsEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        ILogger<ListStoryEpicsEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _logger = logger;
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics")]
    [Authorize]
    public static async Task<Results<Ok<List<StoryEpicListItemDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromServices] ListStoryEpicsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var epics = await ep._epicService.ListEpicsByOwnerAsync(user.Id, user.Id, ct);
        return TypedResults.Ok(epics);
    }
}

