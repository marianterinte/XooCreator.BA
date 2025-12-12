using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class PublishStoryEpicEndpoint
{
    private readonly IStoryEpicPublishingService _publishingService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<PublishStoryEpicEndpoint> _logger;

    public PublishStoryEpicEndpoint(
        IStoryEpicPublishingService publishingService,
        IAuth0UserService auth0,
        ILogger<PublishStoryEpicEndpoint> logger)
    {
        _publishingService = publishingService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/epics/{epicId}/publish")]
    [Authorize]
    public static async Task<Results<Ok<StoryEpicPublishResponse>, UnauthorizedHttpResult, ForbidHttpResult, BadRequest<string>, Conflict<string>>> HandlePost(
        [FromRoute] string epicId,
        [FromServices] PublishStoryEpicEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("PublishStoryEpic forbidden: userId={UserId} roles={Roles}",
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);

        try
        {
            var publishedAt = await ep._publishingService.PublishAsync(user.Id, epicId, isAdmin, ct);
            ep._logger.LogInformation("PublishStoryEpic succeeded: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.Ok(new StoryEpicPublishResponse
            {
                EpicId = epicId,
                Status = "published",
                PublishedAtUtc = publishedAt
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning("PublishStoryEpic unauthorized: {Error}", ex.Message);
            return TypedResults.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            // Validation errors (unpublished dependencies, etc.)
            ep._logger.LogWarning("PublishStoryEpic validation failed: {Error}", ex.Message);
            return TypedResults.Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "PublishStoryEpic error: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.BadRequest($"Failed to publish epic: {ex.Message}");
        }
    }
}
