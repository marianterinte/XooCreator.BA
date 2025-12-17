using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Models;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class UnpublishRegionEndpoint
{
    private readonly IStoryRegionService _regionService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UnpublishRegionEndpoint> _logger;

    public UnpublishRegionEndpoint(
        IStoryRegionService regionService,
        IAuth0UserService auth0,
        ILogger<UnpublishRegionEndpoint> logger)
    {
        _regionService = regionService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/regions/{regionId}/unpublish")]
    [Authorize]
    public static async Task<Results<Ok<UnpublishRegionResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string regionId,
        [FromBody] UnpublishRegionRequest request,
        [FromServices] UnpublishRegionEndpoint ep,
        CancellationToken ct)
    {
        if (request == null)
        {
            return TypedResults.BadRequest("Request body is required.");
        }

        var trimmedRegionId = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmedRegionId))
        {
            return TypedResults.BadRequest("Invalid region id.");
        }

        if (!string.Equals(trimmedRegionId, request.ConfirmRegionId?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("Region id confirmation does not match.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return TypedResults.BadRequest("An unpublish reason is required.");
        }

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        try
        {
            await ep._regionService.UnpublishAsync(user.Id, trimmedRegionId, request.Reason.Trim(), ct);
            ep._logger.LogInformation("Region unpublished: regionId={RegionId} userId={UserId}", trimmedRegionId, user.Id);
            return TypedResults.Ok(new UnpublishRegionResponse());
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning(ex, "Cannot unpublish region: regionId={RegionId}", trimmedRegionId);
            return TypedResults.NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning(ex, "Unauthorized unpublish attempt: regionId={RegionId} userId={UserId}", trimmedRegionId, user.Id);
            return TypedResults.Unauthorized();
        }
    }
}

