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
public class ReviewStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ReviewStoryEndpoint> _logger;

    public ReviewStoryEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0, ILogger<ReviewStoryEndpoint> logger)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _logger = logger;
    }

    public record ReviewRequest
    {
        public bool Approve { get; init; }
        public string? Notes { get; init; }
    }

    public record ReviewResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "Approved";
    }

    [Route("/api/{locale}/stories/{storyId}/review")]
    [Authorize]
    public static async Task<Results<Ok<ReviewResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ReviewStoryEndpoint ep,
        [FromBody] ReviewRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Reviewer))
        {
            ep._logger.LogWarning("Review forbidden: userId={UserId} roles={Roles}", user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        var current = StoryStatusExtensions.FromDb(craft.Status);
        if (current != StoryStatus.InReview)
        {
            ep._logger.LogWarning("Review invalid state: storyId={StoryId} state={State}", storyId, current);
            return TypedResults.Conflict("Invalid state transition. Expected InReview.");
        }

        // Apply decision
        var newStatus = req.Approve ? StoryStatus.Approved : StoryStatus.ChangesRequested;
        craft.Status = newStatus.ToDb();
        craft.ReviewNotes = string.IsNullOrWhiteSpace(req.Notes) ? craft.ReviewNotes : req.Notes;
        craft.ReviewEndedAt = DateTime.UtcNow;
        await ep._crafts.SaveAsync(craft, ct);
        ep._logger.LogInformation("Review decision: storyId={StoryId} to={To} notesPresent={Notes}", storyId, newStatus, !string.IsNullOrWhiteSpace(req.Notes));
        return TypedResults.Ok(new ReviewResponse { Status = req.Approve ? "Approved" : "ChangesRequested" });
    }
}

