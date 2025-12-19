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

        var heroCraft = await ep._repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null) return TypedResults.NotFound();

        if (heroCraft.Status != "sent_for_approval")
        {
            ep._logger.LogWarning("Hero claim invalid state: heroId={HeroId} state={State}", heroId, heroCraft.Status);
            return TypedResults.Conflict("Invalid state transition. Expected sent_for_approval.");
        }

        // Assign reviewer and move to in_review
        heroCraft.Status = "in_review";
        heroCraft.AssignedReviewerUserId = user.Id;
        heroCraft.ReviewStartedAt = DateTime.UtcNow;
        await ep._repository.SaveCraftAsync(heroCraft, ct);
        
        ep._logger.LogInformation("Hero claim: heroId={HeroId} reviewer={Reviewer}", heroId, user.Id);
        return TypedResults.Ok(new ClaimHeroResponse());
    }
}
