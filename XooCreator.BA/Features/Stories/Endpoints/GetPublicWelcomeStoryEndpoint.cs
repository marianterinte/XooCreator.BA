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
/// Supports both welcome stories: ionelbacosca-s251212183034 (kindergarten) and marianterinte-s260124120317 (primary/older)
/// </summary>
[Endpoint]
public class GetPublicWelcomeStoryEndpoint
{
    private readonly IStoriesService _storiesService;
    
    public GetPublicWelcomeStoryEndpoint(IStoriesService storiesService)
    {
        _storiesService = storiesService;
    }

    [Route("/api/{locale}/stories/public/welcome")] // GET /api/{locale}/stories/public/welcome?storyId=marianterinte-s260124120317
    public static async Task<Results<Ok<GetStoryByIdResponse>, NotFound, BadRequest<string>>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? storyId,
        [FromServices] GetPublicWelcomeStoryEndpoint ep)
    {
        // List of allowed welcome story IDs
        var allowedWelcomeStoryIds = new[] { "ionelbacosca-s251212183034", "marianterinte-s260124120317" };
        
        // Use provided storyId or default to kindergarten welcome story for backward compatibility
        var welcomeStoryId = !string.IsNullOrWhiteSpace(storyId) 
            ? storyId 
            : "ionelbacosca-s251212183034";
        
        // Validate that the storyId is one of the allowed welcome stories
        if (!allowedWelcomeStoryIds.Any(id => string.Equals(id, welcomeStoryId, StringComparison.OrdinalIgnoreCase)))
        {
            return TypedResults.BadRequest($"Invalid welcome story ID. Allowed IDs: {string.Join(", ", allowedWelcomeStoryIds)}");
        }
        
        Console.WriteLine($"[GetPublicWelcomeStory] Request received - locale: {locale}, storyId: {welcomeStoryId}");
        
        // Use empty Guid since user is not authenticated - this will return story without progress
        var effectiveUserId = Guid.Empty;
        
        var result = await ep._storiesService.GetStoryByIdAsync(effectiveUserId, welcomeStoryId, locale);
        
        if (result.Story == null)
        {
            Console.WriteLine($"[GetPublicWelcomeStory] Story not found: {welcomeStoryId}");
            return TypedResults.NotFound();
        }
        
        Console.WriteLine($"[GetPublicWelcomeStory] Story found - Title: {result.Story.Title}, Tiles count: {result.Story.Tiles?.Count ?? 0}");
        
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
