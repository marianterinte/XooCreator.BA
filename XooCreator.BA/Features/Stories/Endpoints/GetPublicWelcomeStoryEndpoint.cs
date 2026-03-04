using System;
using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Features.Stories.Configuration;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Features.Stories.DTOs;

namespace XooCreator.BA.Features.Stories.Endpoints;

/// <summary>
/// Public endpoint for the welcome story (no authentication required).
/// Allowed story IDs and default are read from WelcomeFlow options.
/// </summary>
[Endpoint]
public class GetPublicWelcomeStoryEndpoint
{
    private readonly IStoriesService _storiesService;
    private readonly ILogger<GetPublicWelcomeStoryEndpoint> _logger;
    private readonly WelcomeFlowOptions _welcomeFlowOptions;

    public GetPublicWelcomeStoryEndpoint(
        IStoriesService storiesService,
        ILogger<GetPublicWelcomeStoryEndpoint> logger,
        IOptions<WelcomeFlowOptions> welcomeFlowOptions)
    {
        _storiesService = storiesService;
        _logger = logger;
        _welcomeFlowOptions = welcomeFlowOptions.Value;
    }

    [Route("/api/{locale}/stories/public/welcome")] // GET /api/{locale}/stories/public/welcome?storyId=...
    public static async Task<Results<Ok<GetStoryByIdResponse>, NotFound, BadRequest<string>>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? storyId,
        [FromServices] GetPublicWelcomeStoryEndpoint ep)
    {
        var allowedWelcomeStoryIds = ep._welcomeFlowOptions.GetAllowedStoryIds().ToList();
        var welcomeStoryId = !string.IsNullOrWhiteSpace(storyId)
            ? storyId!.Trim()
            : ep._welcomeFlowOptions.GetDefaultStoryId();

        if (string.IsNullOrWhiteSpace(welcomeStoryId) || !allowedWelcomeStoryIds.Any(id => string.Equals(id, welcomeStoryId, StringComparison.OrdinalIgnoreCase)))
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
