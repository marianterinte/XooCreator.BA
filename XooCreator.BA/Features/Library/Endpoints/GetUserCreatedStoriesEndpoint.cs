using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Data;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Library.DTOs;
using System.Text.Json;
using XooCreator.BA.Data.Entities;
using System.Text.Json.Serialization;

namespace XooCreator.BA.Features.Library.Endpoints;

[Endpoint]
public class GetUserCreatedStoriesEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContextService;
    
    public GetUserCreatedStoriesEndpoint(XooDbContext context, IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    [Route("/api/{locale}/stories/created")]
    [Authorize]
    public static async Task<Ok<GetUserCreatedStoriesResponse>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetUserCreatedStoriesEndpoint ep)
    {
        var userId = ep._userContextService.GetCurrentUserId();
        var langCode = LanguageCodeExtensions.FromTag(locale);
        
        // Get published/approved stories from UserCreatedStories
        var publishedStories = await ep._context.UserCreatedStories
            .Include(ucs => ucs.StoryDefinition)
            .ThenInclude(sd => sd.Translations.Where(t => t.LanguageCode == locale))
            .Where(ucs => ucs.UserId == userId && ucs.IsPublished && ucs.StoryDefinition.IsActive)
            .Select(ucs => new CreatedStoryDto
            {
                Id = ucs.StoryDefinition.Id,
                StoryId = ucs.StoryDefinition.StoryId,
                Title = ucs.StoryDefinition.Translations
                    .Where(t => t.LanguageCode == locale)
                    .Select(t => t.Title)
                    .FirstOrDefault() ?? ucs.StoryDefinition.Title,
                CoverImageUrl = ucs.StoryDefinition.CoverImageUrl,
                StoryTopic = ucs.StoryDefinition.StoryTopic,
                StoryType = ucs.StoryDefinition.StoryType,
                Status = ucs.StoryDefinition.Status,
                CreatedAt = ucs.CreatedAt,
                PublishedAt = ucs.PublishedAt,
                IsPublished = ucs.IsPublished,
                CreationNotes = ucs.CreationNotes
            })
            .ToListAsync();

        // Get draft stories from StoryCrafts (for the requested language)
        var drafts = await ep._context.StoryCrafts
            .Include(sc => sc.Translations.Where(t => t.LanguageCode == locale))
            .Where(sc => sc.OwnerUserId == userId)
            .ToListAsync();

        var draftStories = new List<CreatedStoryDto>();
        var publishedStoryIds = new HashSet<string>(publishedStories.Select(s => s.StoryId), StringComparer.OrdinalIgnoreCase);

        foreach (var draft in drafts)
        {
            // Skip if this story is already published (exists in UserCreatedStories)
            if (publishedStoryIds.Contains(draft.StoryId))
                continue;

            // Get translation for the requested locale
            var draftTranslation = draft.Translations.FirstOrDefault(t => t.LanguageCode == locale)
                ?? draft.Translations.FirstOrDefault();

            var storyDto = new CreatedStoryDto
            {
                Id = draft.Id, // Use StoryCraft ID
                StoryId = draft.StoryId,
                Title = draftTranslation?.Title ?? draft.StoryId,
                CoverImageUrl = draft.CoverImageUrl,
                StoryTopic = null, // Not in draft
                StoryType = draft.StoryType,
                // Use the same mapping logic as StoriesService.GetStoryForEditAsync
                // Convert database string status (e.g., "sent_for_approval") to StoryStatus enum
                Status = StoryStatusExtensions.FromDb(draft.Status),
                CreatedAt = draft.CreatedAt,
                PublishedAt = null,
                IsPublished = false,
                CreationNotes = null
            };

            draftStories.Add(storyDto);
        }

        // Combine published and draft stories, remove duplicates by StoryId
        var allStories = publishedStories
            .Concat(draftStories)
            .GroupBy(s => s.StoryId, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderByDescending(s => s.IsPublished).ThenByDescending(s => s.CreatedAt).First())
            .OrderByDescending(s => s.CreatedAt)
            .ToList();

        var response = new GetUserCreatedStoriesResponse
        {
            Stories = allStories,
            TotalCount = allStories.Count
        };

        return TypedResults.Ok(response);
    }
}
