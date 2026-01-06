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
public class ListStoryRegionsEndpoint
{
    private readonly IStoryRegionService _regionService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ListStoryRegionsEndpoint> _logger;

    public ListStoryRegionsEndpoint(
        IStoryRegionService regionService,
        IAuth0UserService auth0,
        ILogger<ListStoryRegionsEndpoint> logger)
    {
        _regionService = regionService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/regions")]
    [Authorize]
    public static async Task<Results<Ok<List<StoryRegionListItemDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromQuery] string? status,
        [FromServices] ListStoryRegionsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var regions = isAdmin 
            ? await ep._regionService.ListAllRegionsAsync(user.Id, status, ct)
            : await ep._regionService.ListRegionsForEditorAsync(user.Id, status, ct);
        
        return TypedResults.Ok(regions);
    }
}

