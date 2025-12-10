using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

public record PublishEpicHeroResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "Published";
    public DateTime? PublishedAtUtc { get; init; }
}

[Endpoint]
public class PublishEpicHeroEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<PublishEpicHeroEndpoint> _logger;

    public PublishEpicHeroEndpoint(
        IEpicHeroService heroService,
        IAuth0UserService auth0,
        ILogger<PublishEpicHeroEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/heroes/{heroId}/publish")]
    [Authorize]
    public static async Task<Results<Ok<PublishEpicHeroResponse>, NotFound, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string heroId,
        [FromServices] PublishEpicHeroEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            return TypedResults.BadRequest("User email is required.");
        }

        try
        {
            await ep._heroService.PublishAsync(user.Id, heroId, email.Trim(), ct);
            var hero = await ep._heroService.GetHeroAsync(heroId, ct);
            return TypedResults.Ok(new PublishEpicHeroResponse 
            { 
                PublishedAtUtc = hero?.PublishedAtUtc 
            });
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

