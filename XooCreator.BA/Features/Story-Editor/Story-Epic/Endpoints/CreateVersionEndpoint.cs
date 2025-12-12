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
public class CreateVersionEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateVersionEndpoint> _logger;

    public CreateVersionEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        ILogger<CreateVersionEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _logger = logger;
    }

    public record CreateEpicVersionResponse
    {
        public required string EpicId { get; init; }
        public int BaseVersion { get; init; }
    }

    [Route("/api/story-editor/epics/{epicId}/create-version")]
    [Authorize]
    public static async Task<
        Results<
            Ok<CreateEpicVersionResponse>,
            BadRequest<string>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult,
            Conflict<string>>> HandlePost(
        [FromRoute] string epicId,
        [FromServices] CreateVersionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Creator-only guard
        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("Create version forbidden: userId={UserId} epicId={EpicId}", user.Id, epicId);
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(epicId) || epicId.Equals("new", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("epicId is required and cannot be 'new'");
        }

        try
        {
            // Get published epic to verify it exists and is published
            var publishedEpic = await ep._epicService.GetEpicAsync(epicId, ct);
            if (publishedEpic == null)
            {
                ep._logger.LogWarning("Create version failed. Epic not found: {EpicId}", epicId);
                return TypedResults.NotFound();
            }

            if (publishedEpic.Status != "published")
            {
                return TypedResults.BadRequest($"Epic is not published (status: {publishedEpic.Status})");
            }

            // Create version synchronously (no job) - returns base version
            var baseVersion = await ep._epicService.CreateVersionFromPublishedAsync(user.Id, epicId, ct);

            ep._logger.LogInformation(
                "Create version completed: userId={UserId} epicId={EpicId} baseVersion={BaseVersion}",
                user.Id,
                epicId,
                baseVersion);

            return TypedResults.Ok(new CreateEpicVersionResponse
            {
                EpicId = epicId,
                BaseVersion = baseVersion
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            ep._logger.LogWarning("Create version unauthorized: {Message}", ex.Message);
            return TypedResults.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("already exists"))
            {
                return TypedResults.Conflict(ex.Message);
            }
            ep._logger.LogWarning("Create version failed: {Message}", ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Create version error: epicId={EpicId}", epicId);
            return TypedResults.BadRequest($"Failed to create version: {ex.Message}");
        }
    }
}

