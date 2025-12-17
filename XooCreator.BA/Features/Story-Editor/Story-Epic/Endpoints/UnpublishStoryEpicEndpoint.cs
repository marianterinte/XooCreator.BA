using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Models;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class UnpublishStoryEpicEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<UnpublishStoryEpicEndpoint> _logger;

    public UnpublishStoryEpicEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        ILogger<UnpublishStoryEpicEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/epics/{epicId}/unpublish")]
    [Authorize]
    public static async Task<Results<Ok<UnpublishStoryEpicResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromBody] UnpublishStoryEpicRequest request,
        [FromServices] UnpublishStoryEpicEndpoint ep,
        CancellationToken ct)
    {
        if (request == null)
        {
            return TypedResults.BadRequest("Request body is required.");
        }

        var trimmedEpicId = (epicId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmedEpicId))
        {
            return TypedResults.BadRequest("Invalid epic id.");
        }

        if (!string.Equals(trimmedEpicId, request.ConfirmEpicId?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("Epic id confirmation does not match.");
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
            await ep._epicService.UnpublishAsync(user.Id, trimmedEpicId, request.Reason.Trim(), ct);
            ep._logger.LogInformation("Epic unpublished: epicId={EpicId} userId={UserId}", trimmedEpicId, user.Id);
            return TypedResults.Ok(new UnpublishStoryEpicResponse());
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning(ex, "Cannot unpublish epic: epicId={EpicId}", trimmedEpicId);
            return TypedResults.NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning(ex, "Unauthorized unpublish attempt: epicId={EpicId} userId={UserId}", trimmedEpicId, user.Id);
            return TypedResults.Unauthorized();
        }
    }
}

