using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class CompleteEpicStoryEndpoint
{
    private readonly IStoryEpicProgressService _progressService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CompleteEpicStoryEndpoint> _logger;

    public CompleteEpicStoryEndpoint(
        IStoryEpicProgressService progressService,
        IAuth0UserService auth0,
        ILogger<CompleteEpicStoryEndpoint> logger)
    {
        _progressService = progressService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-epic/{epicId}/stories/{storyId}/complete")]
    [Authorize]
    public static async Task<Results<Ok<CompleteEpicStoryResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromRoute] string storyId,
        [FromBody] CompleteEpicStoryRequest? request,
        [FromServices] CompleteEpicStoryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var selectedAnswer = request?.SelectedAnswer;

        var result = await ep._progressService.CompleteStoryAsync(epicId, user.Id, storyId, selectedAnswer, ct);
        
        if (!result.Success)
        {
            ep._logger.LogWarning("CompleteEpicStory: Failed to complete story epicId={EpicId} storyId={StoryId} userId={UserId}", epicId, storyId, user.Id);
            return TypedResults.BadRequest("Failed to complete story. It may already be completed.");
        }

        var response = new CompleteEpicStoryResponse
        {
            Success = true,
            EpicId = epicId,
            StoryId = storyId,
            NewlyUnlockedRegions = result.NewlyUnlockedRegions,
            NewlyUnlockedHeroes = result.NewlyUnlockedHeroes,
            StoryCoverImageUrl = result.StoryCoverImageUrl
        };

        return TypedResults.Ok(response);
    }
}

public record CompleteEpicStoryRequest
{
    public string? SelectedAnswer { get; init; }
}

public record CompleteEpicStoryResponse
{
    public required bool Success { get; init; }
    public required string EpicId { get; init; }
    public required string StoryId { get; init; }
    public List<string> NewlyUnlockedRegions { get; init; } = new();
    public List<UnlockedHeroDto> NewlyUnlockedHeroes { get; init; } = new();
    public string? StoryCoverImageUrl { get; init; }
}

