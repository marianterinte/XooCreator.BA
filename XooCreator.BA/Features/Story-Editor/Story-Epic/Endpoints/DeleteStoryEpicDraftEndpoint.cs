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
public class DeleteStoryEpicDraftEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<DeleteStoryEpicDraftEndpoint> _logger;

    public DeleteStoryEpicDraftEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        ILogger<DeleteStoryEpicDraftEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/epics/{epicId}/draft")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandleDelete(
        [FromRoute] string epicId,
        [FromServices] DeleteStoryEpicDraftEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);
        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isCreator && !isAdmin)
        {
            ep._logger.LogWarning("DeleteStoryEpicDraft forbidden: userId={UserId} roles={Roles}",
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        try
        {
            await ep._epicService.DeleteEpicDraftAsync(user.Id, epicId, allowAdminOverride: isAdmin, ct);
            ep._logger.LogInformation("DeleteStoryEpicDraft: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.Ok();
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning("DeleteStoryEpicDraft not found: {Error}", ex.Message);
            return TypedResults.NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning("DeleteStoryEpicDraft unauthorized: {Error}", ex.Message);
            return TypedResults.Forbid();
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "DeleteStoryEpicDraft error: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.BadRequest($"Failed to delete epic draft: {ex.Message}");
        }
    }
}
