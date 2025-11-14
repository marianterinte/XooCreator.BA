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
public class RetractStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<RetractStoryEndpoint> _logger;

    public RetractStoryEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0, ILogger<RetractStoryEndpoint> logger)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _logger = logger;
    }

    public record RetractResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "Draft";
    }

    [Route("/api/stories/{storyId}/retract")]
    [Authorize]
    public static async Task<Results<Ok<RetractResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string storyId,
        [FromServices] RetractStoryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        if (craft.OwnerUserId != user.Id)
        {
            return TypedResults.Forbid();
        }

        var current = StoryStatusExtensions.FromDb(craft.Status);
        if (current != StoryStatus.SentForApproval && current != StoryStatus.InReview && current != StoryStatus.Approved)
        {
            return TypedResults.Conflict("Invalid state transition. Expected SentForApproval, InReview, or Approved.");
        }

        // Clear assignment and revert to Draft
        craft.Status = StoryStatus.Draft.ToDb();
        craft.AssignedReviewerUserId = null;
        craft.ReviewNotes = null;
        craft.ReviewStartedAt = null;
        craft.ReviewEndedAt = null;
        if (current == StoryStatus.Approved)
        {
            craft.ApprovedByUserId = null;
        }
        await ep._crafts.SaveAsync(craft, ct);
        ep._logger.LogInformation("Retract: storyId={StoryId}", storyId);
        return TypedResults.Ok(new RetractResponse());
    }
}

