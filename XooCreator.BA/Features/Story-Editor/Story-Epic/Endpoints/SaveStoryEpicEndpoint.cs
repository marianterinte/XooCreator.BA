using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class SaveStoryEpicEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<SaveStoryEpicEndpoint> _logger;

    public SaveStoryEpicEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        ILogger<SaveStoryEpicEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _logger = logger;
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics/{epicId}")]
    [Authorize]
    public static async Task<Results<Ok, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePut(
        [FromRoute] string epicId,
        [FromServices] SaveStoryEpicEndpoint ep,
        [FromBody] StoryEpicDto dto,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Creator-only guard
        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("SaveStoryEpic forbidden: userId={UserId} roles={Roles}", 
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        // Validate epicId matches
        if (dto.Id != epicId)
        {
            return TypedResults.BadRequest("Epic ID in URL does not match Epic ID in body.");
        }

        try
        {
            await ep._epicService.SaveEpicAsync(user.Id, epicId, dto, ct);
            ep._logger.LogInformation("SaveStoryEpic: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning("SaveStoryEpic unauthorized: {Error}", ex.Message);
            return TypedResults.Forbid();
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "SaveStoryEpic error: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.BadRequest($"Failed to save epic: {ex.Message}");
        }
    }
}

