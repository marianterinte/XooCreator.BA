using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Data.Enums;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class RetractStoryRegionEndpoint
{
    private readonly IStoryRegionService _regionService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<RetractStoryRegionEndpoint> _logger;

    public RetractStoryRegionEndpoint(
        IStoryRegionService regionService,
        IAuth0UserService auth0,
        ILogger<RetractStoryRegionEndpoint> logger)
    {
        _regionService = regionService;
        _auth0 = auth0;
        _logger = logger;
    }

    public record RetractStoryRegionResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "draft";
    }

    [Route("/api/story-editor/regions/{regionId}/retract")]
    [Authorize]
    public static async Task<Results<Ok<RetractStoryRegionResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string regionId,
        [FromServices] RetractStoryRegionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        try
        {
            await ep._regionService.RetractAsync(user.Id, regionId, ct);
            ep._logger.LogInformation("Region retracted: regionId={RegionId}, userId={UserId}", regionId, user.Id);
            return TypedResults.Ok(new RetractStoryRegionResponse { Status = "draft" });
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning("Cannot retract region: regionId={RegionId}, error={Error}", regionId, ex.Message);
            return TypedResults.Conflict(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Error retracting region: regionId={RegionId}", regionId);
            return TypedResults.NotFound();
        }
    }
}

