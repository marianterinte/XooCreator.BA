using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Data.Enums;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class RetractEpicHeroEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<RetractEpicHeroEndpoint> _logger;

    public RetractEpicHeroEndpoint(
        IEpicHeroService heroService,
        IAuth0UserService auth0,
        ILogger<RetractEpicHeroEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    public record RetractEpicHeroResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "draft";
    }

    [Route("/api/story-editor/heroes/{heroId}/retract")]
    [Authorize]
    public static async Task<Results<Ok<RetractEpicHeroResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string heroId,
        [FromServices] RetractEpicHeroEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        try
        {
            await ep._heroService.RetractAsync(user.Id, heroId, ct);
            ep._logger.LogInformation("Hero retracted: heroId={HeroId}, userId={UserId}", heroId, user.Id);
            return TypedResults.Ok(new RetractEpicHeroResponse { Status = "draft" });
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning("Cannot retract hero: heroId={HeroId}, error={Error}", heroId, ex.Message);
            return TypedResults.Conflict(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Error retracting hero: heroId={HeroId}", heroId);
            return TypedResults.NotFound();
        }
    }
}

