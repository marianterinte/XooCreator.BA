using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class UpdateEpicHeroTopicsEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UpdateEpicHeroTopicsEndpoint> _logger;

    public UpdateEpicHeroTopicsEndpoint(
        IEpicHeroService heroService,
        IAuth0UserService auth0,
        ILogger<UpdateEpicHeroTopicsEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    public record UpdateHeroTopicsRequest
    {
        public List<string> TopicIds { get; init; } = new();
    }

    [Route("/api/story-editor/heroes/{heroId}/topics")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
        [FromRoute] string heroId,
        [FromBody] UpdateHeroTopicsRequest request,
        [FromServices] UpdateEpicHeroTopicsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();
        if (!ep._auth0.HasRole(user, UserRole.Creator))
            return TypedResults.Forbid();
        var topicIds = request?.TopicIds ?? new List<string>();
        try
        {
            await ep._heroService.SaveHeroTopicsAsync(user.Id, heroId, topicIds, ct);
            ep._logger.LogInformation("UpdateEpicHeroTopics: userId={UserId} heroId={HeroId}", user.Id, heroId);
            return TypedResults.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }
}
