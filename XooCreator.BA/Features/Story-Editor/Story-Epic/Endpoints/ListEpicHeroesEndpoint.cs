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
public class ListEpicHeroesEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ListEpicHeroesEndpoint> _logger;

    public ListEpicHeroesEndpoint(
        IEpicHeroService heroService,
        IAuth0UserService auth0,
        ILogger<ListEpicHeroesEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/heroes")]
    [Authorize]
    public static async Task<Results<Ok<List<EpicHeroListItemDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromQuery] string? status,
        [FromServices] ListEpicHeroesEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var heroes = isAdmin 
            ? await ep._heroService.ListAllHeroesAsync(user.Id, status, ct)
            : await ep._heroService.ListHeroesForEditorAsync(user.Id, status, ct);
        
        return TypedResults.Ok(heroes);
    }
}

