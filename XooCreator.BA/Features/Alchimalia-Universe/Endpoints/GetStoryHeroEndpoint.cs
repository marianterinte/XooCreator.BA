using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class GetStoryHeroEndpoint
{
    private readonly IStoryHeroService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetStoryHeroEndpoint> _logger;

    public GetStoryHeroEndpoint(
        IStoryHeroService service,
        IAuth0UserService auth0,
        ILogger<GetStoryHeroEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/story-heroes/{storyHeroId}")]
    [Route("/api/{locale}/alchimalia-universe/loi-heroes/{storyHeroId}")]
    [Authorize]
    public static async Task<Results<Ok<StoryHeroDto>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] Guid storyHeroId,
        [FromQuery] string? language,
        [FromServices] GetStoryHeroEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("GetStoryHero forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var storyHero = await ep._service.GetAsync(storyHeroId, language, ct);
            return TypedResults.Ok(storyHero);
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
