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
            // Use a transaction to ensure atomicity
            using var transaction = await ep._context.Database.BeginTransactionAsync(ct);
            
            try
            {
                // Get all progress entries to delete (for counting)
            var allProgressToDelete = await ep._context.UserStoryReadProgress
                .Where(p => p.UserId == userId.Value)
                .ToListAsync(ct);
            var readProgressCount = allProgressToDelete.Count;

            // Get all evaluation results to delete (for counting)
            var allEvaluationResultsToDelete = await ep._context.StoryEvaluationResults
                .Where(r => r.UserId == userId.Value)
                .ToListAsync(ct);
            var evaluationResultsCount = allEvaluationResultsToDelete.Count;

            // Get all quiz answers to delete (for counting)
            var allQuizAnswersToDelete = await ep._context.StoryQuizAnswers
                .Where(a => a.UserId == userId.Value)
                .ToListAsync(ct);
            var quizAnswersCount = allQuizAnswersToDelete.Count;

            // Get all history entries to delete (reset should delete history too)
            var allHistoryToDelete = await ep._context.UserStoryReadHistory
                .Where(h => h.UserId == userId.Value)
                .ToListAsync(ct);
            var historyCount = allHistoryToDelete.Count;

            // Delete all entries
            if (allProgressToDelete.Count > 0)
            {
                ep._context.UserStoryReadProgress.RemoveRange(allProgressToDelete);
            }
            if (allEvaluationResultsToDelete.Count > 0)
            {
                ep._context.StoryEvaluationResults.RemoveRange(allEvaluationResultsToDelete);
            }
            if (allQuizAnswersToDelete.Count > 0)
            {
                ep._context.StoryQuizAnswers.RemoveRange(allQuizAnswersToDelete);
            }
            if (allHistoryToDelete.Count > 0)
            {
                ep._context.UserStoryReadHistory.RemoveRange(allHistoryToDelete);
            }

                // Save all deletions
                await ep._context.SaveChangesAsync(ct);

                // Commit transaction
                await transaction.CommitAsync(ct);

                return TypedResults.Ok(new ResetAllProgressResponse
                {
                    Success = true,
                    ReadProgressDeleted = readProgressCount,
                    EvaluationResultsDeleted = evaluationResultsCount,
                    QuizAnswersDeleted = quizAnswersCount
                });
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
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

