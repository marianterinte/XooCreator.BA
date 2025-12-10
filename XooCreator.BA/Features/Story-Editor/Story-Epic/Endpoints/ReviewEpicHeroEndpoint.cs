using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

public record ReviewEpicHeroRequest
{
    public bool Approve { get; init; }
    public string? Notes { get; init; }
}

public record ReviewEpicHeroResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "Approved";
}

[Endpoint]
public class ReviewEpicHeroEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ReviewEpicHeroEndpoint> _logger;

    public ReviewEpicHeroEndpoint(
        IEpicHeroService heroService,
        IAuth0UserService auth0,
        ILogger<ReviewEpicHeroEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/heroes/{heroId}/review")]
    [Authorize]
    public static async Task<Results<Ok<ReviewEpicHeroResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string heroId,
        [FromBody] ReviewEpicHeroRequest req,
        [FromServices] ReviewEpicHeroEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Reviewer))
        {
            return TypedResults.Forbid();
        }

        try
        {
            await ep._heroService.ReviewAsync(user.Id, heroId, req.Approve, req.Notes, ct);
            return TypedResults.Ok(new ReviewEpicHeroResponse 
            { 
                Status = req.Approve ? "Approved" : "ChangesRequested" 
            });
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
    }
}

