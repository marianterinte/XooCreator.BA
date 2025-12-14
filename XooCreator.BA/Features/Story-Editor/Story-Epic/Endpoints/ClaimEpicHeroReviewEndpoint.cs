using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class ClaimEpicHeroReviewEndpoint
{
    private readonly IEpicHeroRepository _repository;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ClaimEpicHeroReviewEndpoint> _logger;

    public ClaimEpicHeroReviewEndpoint(
        IEpicHeroRepository repository,
        IAuth0UserService auth0,
        ILogger<ClaimEpicHeroReviewEndpoint> logger)
    {
        _repository = repository;
        _auth0 = auth0;
        _logger = logger;
    }

    public record ClaimHeroResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "in_review";
    }

    [Route("/api/story-editor/heroes/{heroId}/claim")]
    [Authorize]
    public static async Task<Results<Ok<ClaimHeroResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string heroId,
        [FromServices] ClaimEpicHeroReviewEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Reviewer-only guard
        if (!ep._auth0.HasRole(user, UserRole.Reviewer) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("Hero claim forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        var hero = await ep._repository.GetAsync(heroId, ct);
        if (hero == null) return TypedResults.NotFound();

        if (hero.Status != "sent_for_approval")
        {
            ep._logger.LogWarning("Hero claim invalid state: heroId={HeroId} state={State}", heroId, hero.Status);
            return TypedResults.Conflict("Invalid state transition. Expected sent_for_approval.");
        }

        // Assign reviewer and move to in_review
        hero.Status = "in_review";
        hero.AssignedReviewerUserId = user.Id;
        hero.ReviewStartedAt = DateTime.UtcNow;
        await ep._repository.SaveAsync(hero, ct);
        
        ep._logger.LogInformation("Hero claim: heroId={HeroId} reviewer={Reviewer}", heroId, user.Id);
        return TypedResults.Ok(new ClaimHeroResponse());
    }
}
