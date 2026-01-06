using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Services;
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
    private readonly IAuth0UserService _auth0;
    
    public GetUserCreatedStoriesEndpoint(XooDbContext context, IUserContextService userContextService, IAuth0UserService auth0)
    {
        _context = context;
        _userContextService = userContextService;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/stories/created")]
    [Authorize]
    public static async Task<Ok<GetUserCreatedStoriesResponse>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetUserCreatedStoriesEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Ok(new GetUserCreatedStoriesResponse { Stories = new List<CreatedStoryDto>(), TotalCount = 0 });
        
        var userId = user.Id;
        var langCode = LanguageCodeExtensions.FromTag(locale);
        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        
        // Get published/approved stories from UserCreatedStories
        // For admin: get all published stories, for regular user: get only their own
        var publishedStoriesQuery = ep._context.UserCreatedStories
            .Include(ucs => ucs.StoryDefinition)
            .ThenInclude(sd => sd.Translations.Where(t => t.LanguageCode == locale))
            .Include(ucs => ucs.User)
            .Where(ucs => ucs.IsPublished && ucs.StoryDefinition.IsActive);
        
        if (!isAdmin)
        {
            publishedStoriesQuery = publishedStoriesQuery.Where(ucs => ucs.UserId == userId);
        }
        
        var publishedStoriesData = await publishedStoriesQuery.ToListAsync(ct);
        
        // Build published stories DTOs with OwnerEmail
        var publishedStories = new List<CreatedStoryDto>();
        foreach (var ucs in publishedStoriesData)
        {
            var ownerEmail = ucs.User?.Email ?? "";
            publishedStories.Add(new CreatedStoryDto
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
                CreationNotes = ucs.CreationNotes,
                OwnerEmail = ownerEmail,
                IsOwnedByCurrentUser = ucs.UserId == userId
            });
        }

        // Get draft stories from StoryCrafts (for the requested language)
        // Only get drafts for current user (admins see all published, but drafts are still per-user)
        var drafts = await ep._context.StoryCrafts
            .Include(sc => sc.Translations.Where(t => t.LanguageCode == locale))
            .Where(sc => sc.OwnerUserId == userId)
            .ToListAsync(ct);

        var draftStories = new List<CreatedStoryDto>();
        var publishedStoryIds = new HashSet<string>(publishedStories.Select(s => s.StoryId), StringComparer.OrdinalIgnoreCase);

        // Get current user email for drafts
        var currentUserEmail = user.Email ?? "";
        
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
                CreationNotes = null,
                OwnerEmail = currentUserEmail,
                IsOwnedByCurrentUser = true // Drafts are always owned by current user
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
