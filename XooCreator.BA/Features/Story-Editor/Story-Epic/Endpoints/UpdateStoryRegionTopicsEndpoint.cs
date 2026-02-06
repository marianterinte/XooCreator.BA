using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class UpdateStoryRegionTopicsEndpoint
{
    private readonly IStoryRegionService _regionService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UpdateStoryRegionTopicsEndpoint> _logger;

    public UpdateStoryRegionTopicsEndpoint(
        IStoryRegionService regionService,
        IAuth0UserService auth0,
        ILogger<UpdateStoryRegionTopicsEndpoint> logger)
    {
        _regionService = regionService;
        _auth0 = auth0;
        _logger = logger;
    }

    public record UpdateRegionTopicsRequest
    {
        public List<string> TopicIds { get; init; } = new();
    }

    [Route("/api/story-editor/regions/{regionId}/topics")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
        [FromRoute] string regionId,
        [FromBody] UpdateRegionTopicsRequest request,
        [FromServices] UpdateStoryRegionTopicsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();
        if (!ep._auth0.HasRole(user, UserRole.Creator))
            return TypedResults.Forbid();
        var topicIds = request?.TopicIds ?? new List<string>();
        try
        {
            await ep._regionService.SaveRegionTopicsAsync(user.Id, regionId, topicIds, ct);
            ep._logger.LogInformation("UpdateStoryRegionTopics: userId={UserId} regionId={RegionId}", user.Id, regionId);
            return TypedResults.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }
}
