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
public class CreateStoryHeroEndpoint
{
    private readonly IStoryHeroService _service;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateStoryHeroEndpoint> _logger;

    public CreateStoryHeroEndpoint(
        IStoryHeroService service,
        IAuth0UserService auth0,
        ILogger<CreateStoryHeroEndpoint> logger)
    {
        _service = service;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/alchimalia-universe/story-heroes")]
    [Route("/api/{locale}/alchimalia-universe/loi-heroes")]
    [Authorize]
    public static async Task<Results<Ok<StoryHeroDto>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] CreateStoryHeroEndpoint ep,
        [FromBody] CreateStoryHeroRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateStoryHero forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        try
        {
            var storyHero = await ep._service.CreateAsync(user.Id, req, ct);
            ep._logger.LogInformation("StoryHero created: {StoryHeroId} by {UserId}", storyHero.Id, user.Id);
            return TypedResults.Ok(storyHero);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
