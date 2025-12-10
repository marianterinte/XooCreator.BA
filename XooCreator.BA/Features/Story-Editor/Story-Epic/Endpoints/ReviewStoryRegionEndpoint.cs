using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

public record ReviewStoryRegionRequest
{
    public bool Approve { get; init; }
    public string? Notes { get; init; }
}

public record ReviewStoryRegionResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "Approved";
}

[Endpoint]
public class ReviewStoryRegionEndpoint
{
    private readonly IStoryRegionService _regionService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ReviewStoryRegionEndpoint> _logger;

    public ReviewStoryRegionEndpoint(
        IStoryRegionService regionService,
        IAuth0UserService auth0,
        ILogger<ReviewStoryRegionEndpoint> logger)
    {
        _regionService = regionService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/regions/{regionId}/review")]
    [Authorize]
    public static async Task<Results<Ok<ReviewStoryRegionResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string regionId,
        [FromBody] ReviewStoryRegionRequest req,
        [FromServices] ReviewStoryRegionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Reviewer))
        {
            return TypedResults.Forbid();
        }

        try
        {
            await ep._regionService.ReviewAsync(user.Id, regionId, req.Approve, req.Notes, ct);
            return TypedResults.Ok(new ReviewStoryRegionResponse 
            { 
                Status = req.Approve ? "Approved" : "ChangesRequested" 
            });
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
    }
}

