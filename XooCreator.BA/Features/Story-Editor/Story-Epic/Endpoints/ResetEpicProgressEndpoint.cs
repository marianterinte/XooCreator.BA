using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class ResetEpicProgressEndpoint
{
    private readonly IStoryEpicProgressService _progressService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ResetEpicProgressEndpoint> _logger;

    public ResetEpicProgressEndpoint(
        IStoryEpicProgressService progressService,
        IAuth0UserService auth0,
        ILogger<ResetEpicProgressEndpoint> logger)
    {
        _progressService = progressService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-epic/{epicId}/reset-progress")]
    [Authorize]
    public static async Task<Results<Ok<ResetEpicProgressResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromServices] ResetEpicProgressEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var result = await ep._progressService.ResetProgressAsync(epicId, user.Id, ct);
        
        if (!result.Success)
        {
            ep._logger.LogWarning("ResetEpicProgress: Failed to reset progress epicId={EpicId} userId={UserId}", epicId, user.Id);
            return TypedResults.BadRequest(result.ErrorMessage ?? "Failed to reset progress");
        }

        var response = new ResetEpicProgressResponse
        {
            Success = true,
            EpicId = epicId,
            Message = "Progress reset successfully"
        };

        return TypedResults.Ok(response);
    }
}

public record ResetEpicProgressResponse
{
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public required string EpicId { get; init; }
    public string Message { get; init; } = string.Empty;
}

