using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ClaimStoryReviewEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ClaimStoryReviewEndpoint> _logger;

    public ClaimStoryReviewEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0, ILogger<ClaimStoryReviewEndpoint> logger)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _logger = logger;
    }

    public record ClaimResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "InReview";
    }

    [Route("/api/{locale}/stories/{storyId}/claim")]
    [Authorize]
    public static async Task<Results<Ok<ClaimResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ClaimStoryReviewEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Reviewer-only guard
        if (user.Role != Data.Enums.UserRole.Reviewer)
        {
            ep._logger.LogWarning("Claim forbidden: userId={UserId} role={Role}", user?.Id, user?.Role);
            return TypedResults.Forbid();
        }

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");
        var lang = LanguageCodeExtensions.FromTag(langTag);

        var craft = await ep._crafts.GetAsync(storyId, lang, ct);
        if (craft == null) return TypedResults.NotFound();

        var current = StoryStatusExtensions.FromDb(craft.Status);
        if (current != StoryStatus.SentForApproval)
        {
            ep._logger.LogWarning("Claim invalid state: storyId={StoryId} state={State}", storyId, current);
            return TypedResults.Conflict("Invalid state transition. Expected SentForApproval.");
        }

        // Assign reviewer and move to InReview
        craft.Status = StoryStatus.InReview.ToDb();
        craft.AssignedReviewerUserId = user.Id;
        craft.ReviewStartedAt = DateTime.UtcNow;
        await ep._crafts.SaveAsync(craft, ct);
        ep._logger.LogInformation("Claim: storyId={StoryId} lang={Lang} reviewer={Reviewer}", storyId, langTag, user.Id);
        return TypedResults.Ok(new ClaimResponse());
    }
}

