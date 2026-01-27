using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Features.Stories.DTOs;

namespace XooCreator.BA.Features.Stories.Endpoints;

/// <summary>
/// Public endpoint for the welcome story (no authentication required)
/// Only works for the hardcoded welcome story ID: ionelbacosca-s251209175610
/// </summary>
[Endpoint]
public class GetPublicWelcomeStoryEndpoint
{
    private readonly IStoriesService _storiesService;
    
    public GetPublicWelcomeStoryEndpoint(IStoriesService storiesService)
    {
        _storiesService = storiesService;
    }

    [Route("/api/{locale}/stories/public/welcome")] // GET /api/{locale}/stories/public/welcome
    public static async Task<Results<Ok<GetStoryByIdResponse>, NotFound>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetPublicWelcomeStoryEndpoint ep)
    {
        // Hardcoded welcome story ID
        const string welcomeStoryId = "ionelbacosca-s251209175610";
        
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
