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
public class ClaimStoryRegionReviewEndpoint
{
    private readonly IStoryRegionRepository _repository;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ClaimStoryRegionReviewEndpoint> _logger;

    public ClaimStoryRegionReviewEndpoint(
        IStoryRegionRepository repository,
        IAuth0UserService auth0,
        ILogger<ClaimStoryRegionReviewEndpoint> logger)
    {
        _repository = repository;
        _auth0 = auth0;
        _logger = logger;
    }

    public record ClaimRegionResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "in_review";
    }

    [Route("/api/story-editor/regions/{regionId}/claim")]
    [Authorize]
    public static async Task<Results<Ok<ClaimRegionResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string regionId,
        [FromServices] ClaimStoryRegionReviewEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Reviewer-only guard
        if (!ep._auth0.HasRole(user, UserRole.Reviewer) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("Region claim forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        var region = await ep._repository.GetAsync(regionId, ct);
        if (region == null) return TypedResults.NotFound();

        if (region.Status != "sent_for_approval")
        {
            ep._logger.LogWarning("Region claim invalid state: regionId={RegionId} state={State}", regionId, region.Status);
            return TypedResults.Conflict("Invalid state transition. Expected sent_for_approval.");
        }

        // Assign reviewer and move to in_review
        region.Status = "in_review";
        region.AssignedReviewerUserId = user.Id;
        region.ReviewStartedAt = DateTime.UtcNow;
        await ep._repository.SaveAsync(region, ct);
        
        ep._logger.LogInformation("Region claim: regionId={RegionId} reviewer={Reviewer}", regionId, user.Id);
        return TypedResults.Ok(new ClaimRegionResponse());
    }
}

