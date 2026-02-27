using System;
using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Features.Stories.DTOs;

namespace XooCreator.BA.Features.Stories.Endpoints;

/// <summary>
/// Public endpoint for the welcome story (no authentication required)
/// Supports both welcome stories: ionelbacosca-s251212183034 (kindergarten) and learn-math-s1 (primary/older)
/// </summary>
[Endpoint]
public class GetPublicWelcomeStoryEndpoint
{
    private readonly IStoriesService _storiesService;
    private readonly ILogger<GetPublicWelcomeStoryEndpoint> _logger;
    
    public GetPublicWelcomeStoryEndpoint(IStoriesService storiesService, ILogger<GetPublicWelcomeStoryEndpoint> logger)
    {
        _storiesService = storiesService;
        _logger = logger;
    }

    [Route("/api/{locale}/stories/public/welcome")] // GET /api/{locale}/stories/public/welcome?storyId=learn-math-s1
    public static async Task<Results<Ok<GetStoryByIdResponse>, NotFound, BadRequest<string>>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? storyId,
        [FromServices] GetPublicWelcomeStoryEndpoint ep)
    {
        // List of allowed welcome story IDs (available without login)
        var allowedWelcomeStoryIds = new[] { "ionelbacosca-s251212183034", "learn-math-s1" };
        
        // Use provided storyId or default to kindergarten welcome story for backward compatibility
        var welcomeStoryId = !string.IsNullOrWhiteSpace(storyId) 
            ? storyId 
            : "ionelbacosca-s251212183034";
        
        // Validate that the storyId is one of the allowed welcome stories
        if (!allowedWelcomeStoryIds.Any(id => string.Equals(id, welcomeStoryId, StringComparison.OrdinalIgnoreCase)))
        {
            return TypedResults.BadRequest($"Invalid welcome story ID. Allowed IDs: {string.Join(", ", allowedWelcomeStoryIds)}");
        }
        
        ep._logger.LogInformation("[GetPublicWelcomeStory] Request received - locale: {Locale}, storyId: {StoryId}", locale, welcomeStoryId);
        
        // Use empty Guid since user is not authenticated - this will return story without progress
        var effectiveUserId = Guid.Empty;
        
        var result = await ep._storiesService.GetStoryByIdAsync(effectiveUserId, welcomeStoryId, locale);
        
        if (result.Story == null)
        {
            ep._logger.LogWarning("[GetPublicWelcomeStory] Story not found: {StoryId}", welcomeStoryId);
            return TypedResults.NotFound();
        }
        
        ep._logger.LogInformation("[GetPublicWelcomeStory] Story found - Title: {Title}, Tiles count: {TilesCount}", result.Story.Title, result.Story.Tiles?.Count ?? 0);
        
        // Ensure progress is empty for public access
        result = result with 
        { 
            UserProgress = new List<UserStoryProgressDto>(),
            IsCompleted = false,
            CompletedAt = null
        };
        
        return TypedResults.Ok(result);
    }
}
