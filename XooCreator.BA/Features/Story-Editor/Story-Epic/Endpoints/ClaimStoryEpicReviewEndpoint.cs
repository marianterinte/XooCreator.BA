using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class ClaimStoryEpicReviewEndpoint
{
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ClaimStoryEpicReviewEndpoint> _logger;

    public ClaimStoryEpicReviewEndpoint(
        XooDbContext context,
        IAuth0UserService auth0,
        ILogger<ClaimStoryEpicReviewEndpoint> logger)
    {
        _context = context;
        _auth0 = auth0;
        _logger = logger;
    }

    public record ClaimEpicResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "in_review";
    }

    [Route("/api/story-editor/epics/{epicId}/claim")]
    [Authorize]
    public static async Task<Results<Ok<ClaimEpicResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromServices] ClaimStoryEpicReviewEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Reviewer-only guard
        if (!ep._auth0.HasRole(user, UserRole.Reviewer) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("Epic claim forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        // Get epic craft (draft)
        var epic = await ep._context.StoryEpicCrafts
            .FirstOrDefaultAsync(e => e.Id == epicId, ct);
        
        if (epic == null) return TypedResults.NotFound();

        var currentStatus = StoryStatusExtensions.FromDb(epic.Status);
        if (currentStatus != StoryStatus.SentForApproval)
        {
            ep._logger.LogWarning("Epic claim invalid state: epicId={EpicId} state={State}", epicId, currentStatus);
            return TypedResults.Conflict("Invalid state transition. Expected SentForApproval.");
        }

        // Assign reviewer and move to InReview
        epic.Status = StoryStatus.InReview.ToDb();
        epic.AssignedReviewerUserId = user.Id;
        epic.ReviewStartedAt = DateTime.UtcNow;
        epic.UpdatedAt = DateTime.UtcNow;
        await ep._context.SaveChangesAsync(ct);
        
        ep._logger.LogInformation("Epic claim: epicId={EpicId} reviewer={Reviewer}", epicId, user.Id);
        return TypedResults.Ok(new ClaimEpicResponse());
    }
}

