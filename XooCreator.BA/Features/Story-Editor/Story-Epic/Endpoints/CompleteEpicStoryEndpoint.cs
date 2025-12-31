using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using TokenReward = XooCreator.BA.Features.TreeOfLight.DTOs.TokenReward;
using TokenFamily = XooCreator.BA.Features.TreeOfLight.DTOs.TokenFamily;

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
        
        // Map tokens from string type to TokenFamily enum
        List<TokenReward>? tokens = null;
        if (request?.Tokens != null && request.Tokens.Count > 0)
        {
            tokens = request.Tokens.Select(t => new TokenReward
            {
                Type = MapTokenFamily(t.Type),
                Value = t.Value,
                Quantity = t.Quantity
            }).ToList();
        }

        var result = await ep._progressService.CompleteStoryAsync(epicId, user.Id, storyId, selectedAnswer, tokens, ct);
        
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

    // Helper method to map string to TokenFamily enum
    private static TokenFamily MapTokenFamily(string type)
    {
        if (string.IsNullOrWhiteSpace(type)) return TokenFamily.Personality;
        return type.Trim() switch
        {
            "TreeOfHeroes" => TokenFamily.Personality,
            "Personality" => TokenFamily.Personality,
            "Alchemy" => TokenFamily.Alchemy,
            "Discovery" => TokenFamily.Discovery,
            "Generative" => TokenFamily.Generative,
            "Learning" => TokenFamily.Discovery,
            _ => Enum.TryParse<TokenFamily>(type, true, out var fam) ? fam : TokenFamily.Personality
        };
    }
}

public record CompleteEpicStoryRequest
{
    public string? SelectedAnswer { get; init; }
    public List<TokenRewardDto>? Tokens { get; init; }
}

// Intermediate DTO for deserialization (type as string)
public record TokenRewardDto
{
    public string Type { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public int Quantity { get; init; }
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

