using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.ParentDashboard.DTOs;
using XooCreator.BA.Features.Stories.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.ParentDashboard.Endpoints;

[Endpoint]
public class GetReadStoriesEndpoint
{
    private readonly XooDbContext _context;
    private readonly IStoriesRepository _storiesRepository;
    private readonly IUserContextService _userContext;

    public GetReadStoriesEndpoint(
        XooDbContext context,
        IStoriesRepository storiesRepository,
        IUserContextService userContext)
    {
        _context = context;
        _storiesRepository = storiesRepository;
        _userContext = userContext;
    }

    [Route("/api/{locale}/parent-dashboard/read-stories")]
    [Authorize]
    public static async Task<Results<Ok<ReadStoriesResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetReadStoriesEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        // Get all history entries for this user (permanent history)
        var allHistory = await ep._context.UserStoryReadHistory
            .Where(h => h.UserId == userId.Value)
            .ToListAsync(ct);

        // Get all active progress entries for this user (current reading progress)
        var allProgress = await ep._context.UserStoryReadProgress
            .Where(p => p.UserId == userId.Value)
            .OrderBy(p => p.ReadAt)
            .ToListAsync(ct);

        // Group progress by StoryId (case-insensitive)
        var progressByStory = allProgress
            .GroupBy(p => p.StoryId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        // Combine history and progress: use progress if exists, otherwise use history
        var allStoryIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var history in allHistory)
        {
            allStoryIds.Add(history.StoryId);
        }
        foreach (var progressKey in progressByStory.Keys)
        {
            allStoryIds.Add(progressKey);
        }

        var readStories = new List<ReadStoryDto>();

        foreach (var storyId in allStoryIds)
        {
            // Normalize storyId for lookup
            var normalizedStoryId = NormalizeStoryId(storyId);

            // Get story definition to get title, cover image, and total tiles
            var story = await ep._storiesRepository.GetStoryDefinitionByIdAsync(normalizedStoryId);
            if (story == null || !story.IsActive)
                continue; // Skip if story doesn't exist or is inactive

            // Get translation for the requested locale
            var translation = story.Translations
                .FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase))
                ?? story.Translations.FirstOrDefault();

            var title = translation?.Title ?? story.Title;
            var totalTiles = story.Tiles?.Count ?? 0;

            // Check if we have active progress (preferred) or history
            int totalTilesRead;
            DateTime? lastReadAt;
            bool isCompleted;

            if (progressByStory.TryGetValue(storyId, out var progressEntries) && progressEntries.Count > 0)
            {
                // Use active progress
                totalTilesRead = progressEntries.Count;
                lastReadAt = progressEntries.Max(p => p.ReadAt);
                isCompleted = totalTiles > 0 && totalTilesRead >= totalTiles;
            }
            else
            {
                // Use history
                var history = allHistory
                    .FirstOrDefault(h => string.Equals(h.StoryId, storyId, StringComparison.OrdinalIgnoreCase));
                
                if (history != null)
                {
                    totalTilesRead = history.TotalTilesRead;
                    lastReadAt = history.LastReadAt;
                    isCompleted = history.CompletedAt.HasValue || (totalTiles > 0 && totalTilesRead >= totalTiles);
                }
                else
                {
                    continue; // Skip if no history or progress
                }
            }

            var progressPercentage = totalTiles > 0
                ? (int)Math.Round((double)totalTilesRead / totalTiles * 100)
                : 0;

            readStories.Add(new ReadStoryDto
            {
                StoryId = story.StoryId, // Use normalized storyId from database
                Title = title,
                CoverImageUrl = story.CoverImageUrl,
                TotalTilesRead = totalTilesRead,
                TotalTiles = totalTiles,
                ProgressPercentage = progressPercentage,
                LastReadAt = lastReadAt,
                IsCompleted = isCompleted,
                IsPartOfEpic = story.IsPartOfEpic
            });
        }

        // Sort by last read date (most recent first)
        readStories = readStories
            .OrderByDescending(s => s.LastReadAt ?? DateTime.MinValue)
            .ToList();

        return TypedResults.Ok(new ReadStoriesResponse
        {
            Stories = readStories,
            TotalCount = readStories.Count
        });
    }

    private static string NormalizeStoryId(string storyId)
    {
        if (string.Equals(storyId, "intro-puf-puf", StringComparison.OrdinalIgnoreCase))
            return "intro-pufpuf";
        return storyId;
    }
}
