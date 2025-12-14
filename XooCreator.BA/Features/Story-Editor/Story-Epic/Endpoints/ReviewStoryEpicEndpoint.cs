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
public class ReviewStoryEpicEndpoint
{
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ReviewStoryEpicEndpoint> _logger;

    public ReviewStoryEpicEndpoint(
        XooDbContext context,
        IAuth0UserService auth0,
        ILogger<ReviewStoryEpicEndpoint> logger)
    {
        _context = context;
        _auth0 = auth0;
        _logger = logger;
    }

    public record ReviewStoryEpicRequest
    {
        public bool Approve { get; init; }
        public string? Notes { get; init; }
    }

    public record ReviewStoryEpicResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "approved";
    }

    [Route("/api/story-editor/epics/{epicId}/review")]
    [Authorize]
    public static async Task<Results<Ok<ReviewStoryEpicResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromServices] ReviewStoryEpicEndpoint ep,
        [FromBody] ReviewStoryEpicRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Reviewer))
        {
            ep._logger.LogWarning("Review epic forbidden: userId={UserId} roles={Roles}",
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        // Get epic craft (draft)
        var epic = await ep._context.StoryEpicCrafts
            .FirstOrDefaultAsync(e => e.Id == epicId, ct);
        
        if (epic == null) return TypedResults.NotFound();

        var current = StoryStatusExtensions.FromDb(epic.Status);
        if (current != StoryStatus.InReview && current != StoryStatus.SentForApproval)
        {
            ep._logger.LogWarning("Review epic invalid state: epicId={EpicId} state={State}", epicId, current);
            return TypedResults.Conflict("Invalid state transition. Expected InReview or SentForApproval.");
        }

        // If not already in review, assign reviewer
        if (current == StoryStatus.SentForApproval)
        {
            epic.Status = StoryStatus.InReview.ToDb();
            epic.AssignedReviewerUserId = user.Id;
            epic.ReviewStartedAt = DateTime.UtcNow;
        }

        // Apply decision
        var newStatus = req.Approve ? StoryStatus.Approved : StoryStatus.ChangesRequested;
        epic.Status = newStatus.ToDb();
        epic.ReviewNotes = string.IsNullOrWhiteSpace(req.Notes) ? epic.ReviewNotes : req.Notes;
        epic.ReviewEndedAt = DateTime.UtcNow;
        epic.ReviewedByUserId = user.Id;
        if (req.Approve)
        {
            epic.ApprovedByUserId = user.Id;
        }
        epic.UpdatedAt = DateTime.UtcNow;
        
        await ep._context.SaveChangesAsync(ct);
        ep._logger.LogInformation("Review epic decision: epicId={EpicId} to={To} notesPresent={Notes}", epicId, newStatus, !string.IsNullOrWhiteSpace(req.Notes));
        return TypedResults.Ok(new ReviewStoryEpicResponse { Status = req.Approve ? "approved" : "changes_requested" });
    }
}

