using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Models;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class UnpublishHeroEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UnpublishHeroEndpoint> _logger;

    public UnpublishHeroEndpoint(
        IEpicHeroService heroService,
        IAuth0UserService auth0,
        ILogger<UnpublishHeroEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/heroes/{heroId}/unpublish")]
    [Authorize]
    public static async Task<Results<Ok<UnpublishHeroResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string heroId,
        [FromBody] UnpublishHeroRequest request,
        [FromServices] UnpublishHeroEndpoint ep,
        CancellationToken ct)
    {
        if (request == null)
        {
            return TypedResults.BadRequest("Request body is required.");
        }

        var trimmedHeroId = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmedHeroId))
        {
            return TypedResults.BadRequest("Invalid hero id.");
        }

        if (!string.Equals(trimmedHeroId, request.ConfirmHeroId?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("Hero id confirmation does not match.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return TypedResults.BadRequest("An unpublish reason is required.");
        }

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        try
        {
            await ep._heroService.UnpublishAsync(user.Id, trimmedHeroId, request.Reason.Trim(), ct);
            ep._logger.LogInformation("Hero unpublished: heroId={HeroId} userId={UserId}", trimmedHeroId, user.Id);
            return TypedResults.Ok(new UnpublishHeroResponse());
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning(ex, "Cannot unpublish hero: heroId={HeroId}", trimmedHeroId);
            return TypedResults.NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning(ex, "Unauthorized unpublish attempt: heroId={HeroId} userId={UserId}", trimmedHeroId, user.Id);
            return TypedResults.Unauthorized();
        }
    }
}

