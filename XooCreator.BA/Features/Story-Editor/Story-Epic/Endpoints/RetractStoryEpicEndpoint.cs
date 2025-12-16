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
public class RetractStoryEpicEndpoint
{
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<RetractStoryEpicEndpoint> _logger;

    public RetractStoryEpicEndpoint(
        XooDbContext context,
        IAuth0UserService auth0,
        ILogger<RetractStoryEpicEndpoint> logger)
    {
        _context = context;
        _auth0 = auth0;
        _logger = logger;
    }

    public record RetractStoryEpicResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "draft";
    }

    [Route("/api/story-editor/epics/{epicId}/retract")]
    [Authorize]
    public static async Task<Results<Ok<RetractStoryEpicResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromServices] RetractStoryEpicEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("Retract epic forbidden: userId={UserId} roles={Roles}",
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        // Get epic craft (draft)
        var epic = await ep._context.StoryEpicCrafts
            .FirstOrDefaultAsync(e => e.Id == epicId, ct);
        
        if (epic == null) return TypedResults.NotFound();

        if (epic.OwnerUserId != user.Id)
        {
            ep._logger.LogWarning("Retract epic not owner: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.Forbid();
        }

        var current = StoryStatusExtensions.FromDb(epic.Status);
        if (current != StoryStatus.SentForApproval && current != StoryStatus.InReview && current != StoryStatus.Approved)
        {
            ep._logger.LogWarning("Retract epic invalid state: epicId={EpicId} state={State}", epicId, current);
            return TypedResults.Conflict("Invalid state transition. Expected SentForApproval, InReview, or Approved.");
        }

        // Clear all review-related fields and revert to Draft
        epic.Status = StoryStatus.Draft.ToDb();
        epic.AssignedReviewerUserId = null;
        epic.ReviewNotes = null;
        epic.ReviewStartedAt = null;
        epic.ReviewEndedAt = null;
        if (current == StoryStatus.Approved)
        {
            epic.ApprovedByUserId = null;
        }
        epic.ReviewedByUserId = null;
        epic.UpdatedAt = DateTime.UtcNow;
        
        await ep._context.SaveChangesAsync(ct);
        ep._logger.LogInformation("Retract epic: epicId={EpicId}", epicId);
        return TypedResults.Ok(new RetractStoryEpicResponse());
    }
}

