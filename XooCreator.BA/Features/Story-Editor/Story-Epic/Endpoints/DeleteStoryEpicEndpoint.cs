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
public class DeleteStoryEpicEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<DeleteStoryEpicEndpoint> _logger;

    public DeleteStoryEpicEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        ILogger<DeleteStoryEpicEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _logger = logger;
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics/{epicId}")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandleDelete(
        [FromRoute] string epicId,
        [FromServices] DeleteStoryEpicEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Creator-only guard
        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("DeleteStoryEpic forbidden: userId={UserId} roles={Roles}", 
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        try
        {
            await ep._epicService.DeleteEpicAsync(user.Id, epicId, ct);
            ep._logger.LogInformation("DeleteStoryEpic: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.Ok();
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning("DeleteStoryEpic not found: {Error}", ex.Message);
            return TypedResults.NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning("DeleteStoryEpic unauthorized: {Error}", ex.Message);
            return TypedResults.Forbid();
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "DeleteStoryEpic error: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.BadRequest($"Failed to delete epic: {ex.Message}");
        }
    }
}

