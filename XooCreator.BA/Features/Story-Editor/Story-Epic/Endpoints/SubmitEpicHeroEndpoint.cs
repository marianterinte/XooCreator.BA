using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

public record SubmitEpicHeroResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "SentForApproval";
}

[Endpoint]
public class SubmitEpicHeroEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<SubmitEpicHeroEndpoint> _logger;

    public SubmitEpicHeroEndpoint(
        IEpicHeroService heroService,
        IAuth0UserService auth0,
        ILogger<SubmitEpicHeroEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/heroes/{heroId}/submit")]
    [Authorize]
    public static async Task<Results<Ok<SubmitEpicHeroResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string heroId,
        [FromServices] SubmitEpicHeroEndpoint ep,
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
            await ep._heroService.SubmitForReviewAsync(user.Id, heroId, ct);
            return TypedResults.Ok(new SubmitEpicHeroResponse());
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }
}

