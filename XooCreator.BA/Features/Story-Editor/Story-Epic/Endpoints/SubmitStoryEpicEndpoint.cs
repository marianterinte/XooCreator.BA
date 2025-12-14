using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class SubmitStoryEpicEndpoint
{
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<SubmitStoryEpicEndpoint> _logger;

    public SubmitStoryEpicEndpoint(
        XooDbContext context,
        IAuth0UserService auth0,
        ILogger<SubmitStoryEpicEndpoint> logger)
    {
        _context = context;
        _auth0 = auth0;
        _logger = logger;
    }

    public record SubmitStoryEpicResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "sent_for_approval";
    }

    [Route("/api/story-editor/epics/{epicId}/submit")]
    [Authorize]
    public static async Task<Results<Ok<SubmitStoryEpicResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromServices] SubmitStoryEpicEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("Submit epic forbidden: userId={UserId} roles={Roles}",
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        // Get epic craft (draft)
        var epic = await ep._context.StoryEpicCrafts
            .FirstOrDefaultAsync(e => e.Id == epicId, ct);
        
        if (epic == null) return TypedResults.NotFound();

        if (epic.OwnerUserId != user.Id)
        {
            ep._logger.LogWarning("Submit epic not owner: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.Forbid();
        }

        var current = StoryStatusExtensions.FromDb(epic.Status);
        if (current != StoryStatus.Draft && current != StoryStatus.ChangesRequested)
        {
            ep._logger.LogWarning("Submit epic invalid state: epicId={EpicId} state={State}", epicId, current);
            return TypedResults.Conflict("Invalid state transition. Expected Draft or ChangesRequested.");
        }

        epic.Status = StoryStatus.SentForApproval.ToDb();
        epic.AssignedReviewerUserId = null;
        epic.ReviewNotes = null;
        epic.ReviewStartedAt = null;
        epic.ReviewEndedAt = null;
        epic.UpdatedAt = DateTime.UtcNow;
        
        await ep._context.SaveChangesAsync(ct);
        ep._logger.LogInformation("Submit epic: epicId={EpicId} from={From} to={To}", epicId, current, StoryStatus.SentForApproval);
        return TypedResults.Ok(new SubmitStoryEpicResponse());
    }
}

