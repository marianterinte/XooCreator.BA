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
public class GetEvaluativeStoriesEndpoint
{
    private readonly XooDbContext _context;
    private readonly IStoriesRepository _storiesRepository;
    private readonly IUserContextService _userContext;

    public GetEvaluativeStoriesEndpoint(
        XooDbContext context,
        IStoriesRepository storiesRepository,
        IUserContextService userContext)
    {
        _context = context;
        _storiesRepository = storiesRepository;
        _userContext = userContext;
    }

    [Route("/api/{locale}/parent-dashboard/evaluative-stories")]
    [Authorize]
    public static async Task<Results<Ok<EvaluativeStoriesResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetEvaluativeStoriesEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        // Get all evaluation results for this user (load once, filter in memory)
        var allUserResults = await ep._context.StoryEvaluationResults
            .Where(r => r.UserId == userId.Value)
            .ToListAsync(ct);

        // Get all progress entries for this user (active progress)
        var allProgress = await ep._context.UserStoryReadProgress
            .Where(p => p.UserId == userId.Value)
            .ToListAsync(ct);

        // Get all history entries for this user (permanent history)
        var allHistory = await ep._context.UserStoryReadHistory
            .Where(h => h.UserId == userId.Value)
            .ToListAsync(ct);

        // Get unique story IDs that user has started (from progress or history) or has evaluation results
        var startedStoryIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        // Add story IDs from progress
        foreach (var progress in allProgress)
        {
            startedStoryIds.Add(progress.StoryId);
        }
        
        // Add story IDs from history
        foreach (var history in allHistory)
        {
            startedStoryIds.Add(history.StoryId);
        }
        
        // Add story IDs from evaluation results
        foreach (var allUserResult in allUserResults)
        {
            startedStoryIds.Add(allUserResult.StoryId);
        }

        // Get evaluative stories that user has started (IsEvaluative == true and IsActive == true)
        // Only include stories that user has started or has evaluation results for
        var allEvaluativeStories = await ep._context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Tiles)
            .Where(s => s.IsEvaluative && s.IsActive)
            .ToListAsync(ct);

        // Filter to only include stories that user has started
        var evaluativeStories = allEvaluativeStories
            .Where(s => startedStoryIds.Contains(s.StoryId))
            .OrderBy(s => s.SortOrder)
            .ToList();

        var result = new List<EvaluativeStoryDto>();

        foreach (var story in evaluativeStories)
        {
            // Get translation for the requested locale
            var translation = story.Translations
                .FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase))
                ?? story.Translations.FirstOrDefault();

            var title = translation?.Title ?? story.Title;
            var totalQuizzes = story.Tiles?.Count(t => t.Type == "quiz") ?? 0;

            // Get latest evaluation result for this user and story (filter in memory for case-insensitive match)
            var latestResult = allUserResults
                .Where(r => string.Equals(r.StoryId, story.StoryId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.CompletedAt)
                .FirstOrDefault();

            EvaluationResultDto? resultDto = null;
            if (latestResult != null)
            {
                resultDto = new EvaluationResultDto
                {
                    ScorePercentage = latestResult.ScorePercentage,
                    CorrectAnswers = latestResult.CorrectAnswers,
                    TotalQuizzes = latestResult.TotalQuizzes,
                    CompletedAt = latestResult.CompletedAt
                };
            }

            result.Add(new EvaluativeStoryDto
            {
                StoryId = story.StoryId,
                Title = title,
                CoverImageUrl = story.CoverImageUrl,
                TotalQuizzes = totalQuizzes,
                HasEvaluationResult = latestResult != null,
                LatestResult = resultDto
            });
        }

        // Sort by completion date (most recent first), then by title
        result = result
            .OrderByDescending(s => s.LatestResult?.CompletedAt ?? DateTime.MinValue)
            .ThenBy(s => s.Title)
            .ToList();

        return TypedResults.Ok(new EvaluativeStoriesResponse
        {
            Stories = result,
            TotalCount = result.Count
        });
    }
}
