using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.ParentDashboard.DTOs;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.ParentDashboard.Endpoints;

[Endpoint]
public class ResetAllProgressEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContext;

    public ResetAllProgressEndpoint(
        XooDbContext context,
        IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    [Route("/api/{locale}/parent-dashboard/reset-all-progress")]
    [Authorize]
    public static async Task<Results<Ok<ResetAllProgressResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] ResetAllProgressEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        try
        {
            // Get all progress entries before deletion to save to history
            var allProgress = await ep._context.UserStoryReadProgress
                .Where(p => p.UserId == userId.Value)
                .ToListAsync(ct);

            // Group progress by StoryId (case-insensitive)
            var progressByStory = allProgress
                .GroupBy(p => p.StoryId, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Save each story's progress to history before deletion
            foreach (var storyGroup in progressByStory)
            {
                var storyId = storyGroup.Key;
                var progressEntries = storyGroup.ToList();
                var totalTilesRead = progressEntries.Count;
                var lastReadAt = progressEntries.Max(e => e.ReadAt);

                // Normalize storyId for lookup (same as in other endpoints)
                var normalizedStoryId = NormalizeStoryId(storyId);

                // Get story definition to get total tiles
                var story = await ep._context.StoryDefinitions
                    .Include(s => s.Tiles)
                    .FirstOrDefaultAsync(s => s.StoryId == normalizedStoryId && s.IsActive, ct);

                var totalTiles = story?.Tiles?.Count ?? 0;
                var isCompleted = totalTiles > 0 && totalTilesRead >= totalTiles;

                // Check if history record exists (load all and filter in memory for case-insensitive match)
                var allHistory = await ep._context.UserStoryReadHistory
                    .Where(h => h.UserId == userId.Value)
                    .ToListAsync(ct);
                
                var existingHistory = allHistory
                    .FirstOrDefault(h => string.Equals(h.StoryId, storyId, StringComparison.OrdinalIgnoreCase));

                if (existingHistory != null)
                {
                    // Update existing history record
                    existingHistory.TotalTilesRead = totalTilesRead;
                    existingHistory.TotalTiles = totalTiles;
                    existingHistory.LastReadAt = lastReadAt;
                    if (isCompleted && !existingHistory.CompletedAt.HasValue)
                    {
                        existingHistory.CompletedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    // Create new history record
                    var history = new UserStoryReadHistory
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId.Value,
                        StoryId = storyId,
                        TotalTilesRead = totalTilesRead,
                        TotalTiles = totalTiles,
                        LastReadAt = lastReadAt,
                        CompletedAt = isCompleted ? DateTime.UtcNow : null
                    };
                    ep._context.UserStoryReadHistory.Add(history);
                }
            }

            // Save history changes
            await ep._context.SaveChangesAsync(ct);

            // Delete all UserStoryReadProgress entries for this user
            var readProgressCount = await ep._context.UserStoryReadProgress
                .Where(p => p.UserId == userId.Value)
                .ExecuteDeleteAsync(ct);

            // Delete all StoryEvaluationResult entries for this user
            var evaluationResultsCount = await ep._context.StoryEvaluationResults
                .Where(r => r.UserId == userId.Value)
                .ExecuteDeleteAsync(ct);

            // Delete all StoryQuizAnswer entries for this user
            var quizAnswersCount = await ep._context.StoryQuizAnswers
                .Where(a => a.UserId == userId.Value)
                .ExecuteDeleteAsync(ct);

            return TypedResults.Ok(new ResetAllProgressResponse
            {
                Success = true,
                ReadProgressDeleted = readProgressCount,
                EvaluationResultsDeleted = evaluationResultsCount,
                QuizAnswersDeleted = quizAnswersCount
            });
        }
        catch (Exception ex)
        {
            return TypedResults.Ok(new ResetAllProgressResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            });
        }
    }

    private static string NormalizeStoryId(string storyId)
    {
        if (string.Equals(storyId, "intro-puf-puf", StringComparison.OrdinalIgnoreCase))
            return "intro-pufpuf";
        return storyId;
    }
}

