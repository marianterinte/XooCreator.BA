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
        var langCode = NormalizeLocale(locale);
        
        // Get published/approved stories from UserCreatedStories
        var publishedStories = await ep._context.UserCreatedStories
            .Include(ucs => ucs.StoryDefinition)
            .ThenInclude(sd => sd.Translations.Where(t => t.LanguageCode == locale))
            .Where(ucs => ucs.UserId == userId)
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

        // Get draft stories from StoryCrafts
        var drafts = await ep._context.StoryCrafts
            .Where(sc => sc.OwnerUserId == userId && sc.Lang == langCode)
            .ToListAsync();

        var draftStories = new List<CreatedStoryDto>();
        var publishedStoryIds = new HashSet<string>(publishedStories.Select(s => s.StoryId), StringComparer.OrdinalIgnoreCase);

        foreach (var draft in drafts)
        {
            // Skip if this story is already published (exists in UserCreatedStories)
            if (publishedStoryIds.Contains(draft.StoryId))
                continue;

            try
            {
                // Parse JSON to extract story metadata
                var jsonDoc = JsonDocument.Parse(draft.Json);
                var root = jsonDoc.RootElement;

                var storyDto = new CreatedStoryDto
                {
                    Id = draft.Id, // Use StoryCraft ID
                    StoryId = draft.StoryId,
                    Title = root.TryGetProperty("title", out var titleProp) 
                        ? titleProp.GetString() ?? draft.StoryId 
                        : draft.StoryId,
                    CoverImageUrl = root.TryGetProperty("coverImageUrl", out var coverProp) 
                        ? coverProp.GetString() 
                        : null,
                    StoryTopic = null, // Not in draft JSON
                    StoryType = root.TryGetProperty("storyType", out var typeProp) && typeProp.ValueKind == JsonValueKind.Number
                        ? (StoryType)typeProp.GetInt32()
                        : StoryType.AlchimaliaEpic,
                    Status = draft.Status switch
                    {
                        "draft" => StoryStatus.Draft,
                        "in_review" => StoryStatus.Draft, // In review is still considered draft
                        "approved" => StoryStatus.Draft, // Approved but not yet published is still draft
                        "published" => StoryStatus.Published,
                        "archived" => StoryStatus.Retreated, // Archived is treated as retreated
                        _ => StoryStatus.Draft
                    },
                    CreatedAt = draft.UpdatedAt, // Use UpdatedAt as CreatedAt for drafts
                    PublishedAt = null,
                    IsPublished = false,
                    CreationNotes = null
                };

                draftStories.Add(storyDto);
            }
            catch (JsonException)
            {
                // If JSON parsing fails, create a minimal DTO
                draftStories.Add(new CreatedStoryDto
                {
                    Id = draft.Id,
                    StoryId = draft.StoryId,
                    Title = draft.StoryId,
                    CoverImageUrl = null,
                    StoryTopic = null,
                    StoryType = StoryType.AlchimaliaEpic,
                    Status = StoryStatus.Draft,
                    CreatedAt = draft.UpdatedAt,
                    PublishedAt = null,
                    IsPublished = false,
                    CreationNotes = null
                });
            }
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

    private static LanguageCode NormalizeLocale(string locale)
    {
        var normalized = (locale ?? "ro-ro").ToLowerInvariant();
        return normalized switch
        {
            "en-us" => LanguageCode.EnUs,
            "hu-hu" => LanguageCode.HuHu,
            _ => LanguageCode.RoRo
        };
    }
}
