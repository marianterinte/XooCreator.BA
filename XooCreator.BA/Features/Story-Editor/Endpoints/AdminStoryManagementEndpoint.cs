using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class AdminStoryManagementEndpoint
{
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<AdminStoryManagementEndpoint> _logger;

    public AdminStoryManagementEndpoint(
        XooDbContext context,
        IAuth0UserService auth0,
        ILogger<AdminStoryManagementEndpoint> logger)
    {
        _context = context;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/admin/stories/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<AdminStoryDetailsResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string storyId,
        [FromServices] AdminStoryManagementEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Admin))
            return TypedResults.Forbid();

        // Normalize story ID
        var normalizedStoryId = NormalizeStoryId(storyId);

        // Get story without IsActive filter (admin can see all stories)
        // Try exact match first, then case-insensitive search
        var story = await ep._context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Tiles)
            .FirstOrDefaultAsync(s => s.StoryId == normalizedStoryId, ct);

        // If not found with normalized ID, try case-insensitive search
        if (story == null)
        {
            var allStories = await ep._context.StoryDefinitions
                .Include(s => s.Translations)
                .Include(s => s.Tiles)
                .ToListAsync(ct);
            
            story = allStories
                .FirstOrDefault(s => string.Equals(s.StoryId, storyId, StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(s.StoryId, normalizedStoryId, StringComparison.OrdinalIgnoreCase));
        }

        if (story == null)
            return TypedResults.NotFound();

        // Get PublishedAt from UserCreatedStories if available
        var userCreatedStory = await ep._context.UserCreatedStories
            .FirstOrDefaultAsync(ucs => ucs.StoryDefinitionId == story.Id, ct);

        return TypedResults.Ok(new AdminStoryDetailsResponse
        {
            StoryId = story.StoryId,
            Title = story.Title,
            CoverImageUrl = story.CoverImageUrl,
            IsActive = story.IsActive,
            Status = story.Status.ToString(),
            StoryType = story.StoryType.ToString(),
            IsEvaluative = story.IsEvaluative,
            TotalTiles = story.Tiles?.Count ?? 0,
            CreatedAt = story.CreatedAt,
            UpdatedAt = story.UpdatedAt,
            PublishedAt = userCreatedStory?.PublishedAt
        });
    }

    [Route("/api/admin/stories/{storyId}/deactivate")]
    [Authorize]
    public static async Task<Results<Ok<AdminStoryActionResponse>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string storyId,
        [FromServices] AdminStoryManagementEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Admin))
            return TypedResults.Forbid();

        var story = await ep._context.StoryDefinitions
            .FirstOrDefaultAsync(s => s.StoryId == storyId, ct);

        if (story == null)
            return TypedResults.NotFound();

        story.IsActive = false;
        story.UpdatedAt = DateTime.UtcNow;
        story.UpdatedBy = user.Id;

        await ep._context.SaveChangesAsync(ct);

        ep._logger.LogInformation("Admin deactivated story: storyId={StoryId}, adminId={AdminId}", storyId, user.Id);

        return TypedResults.Ok(new AdminStoryActionResponse
        {
            Success = true,
            Message = "Story deactivated successfully"
        });
    }

    [Route("/api/admin/stories/{storyId}/delete")]
    [Authorize]
    public static async Task<Results<Ok<AdminStoryActionResponse>, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandleDelete(
        [FromRoute] string storyId,
        [FromBody] DeleteStoryRequest request,
        [FromServices] AdminStoryManagementEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Admin))
            return TypedResults.Forbid();

        if (request == null || string.IsNullOrWhiteSpace(request.ConfirmStoryId))
            return TypedResults.BadRequest("Confirmation story ID is required");

        if (!string.Equals(storyId, request.ConfirmStoryId, StringComparison.OrdinalIgnoreCase))
            return TypedResults.BadRequest("Story ID confirmation does not match");

        var story = await ep._context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Tokens)
            .FirstOrDefaultAsync(s => s.StoryId == storyId, ct);

        if (story == null)
            return TypedResults.NotFound();

        using var transaction = await ep._context.Database.BeginTransactionAsync(ct);
        try
        {
            // Delete related data first
            // Delete UserStoryReadProgress
            var readProgress = await ep._context.UserStoryReadProgress
                .Where(p => p.StoryId == storyId)
                .ExecuteDeleteAsync(ct);

            // Delete UserStoryReadHistory
            var readHistory = await ep._context.UserStoryReadHistory
                .Where(h => h.StoryId == storyId)
                .ExecuteDeleteAsync(ct);

            // Delete StoryEvaluationResults
            var evaluationResults = await ep._context.StoryEvaluationResults
                .Where(r => r.StoryId == storyId)
                .ExecuteDeleteAsync(ct);

            // Delete StoryQuizAnswers
            var quizAnswers = await ep._context.StoryQuizAnswers
                .Where(a => a.StoryId == storyId)
                .ExecuteDeleteAsync(ct);

            // Delete StoryPurchases
            var purchases = await ep._context.StoryPurchases
                .Where(sp => sp.StoryId == storyId)
                .ExecuteDeleteAsync(ct);

            // Delete StoryReviews (uses StoryId string, not StoryDefinitionId)
            var reviews = await ep._context.StoryReviews
                .Where(r => r.StoryId == storyId)
                .ExecuteDeleteAsync(ct);

            // Delete StoryFavorites
            var favorites = await ep._context.UserFavoriteStories
                .Where(f => f.StoryDefinitionId == story.Id)
                .ExecuteDeleteAsync(ct);

            // Delete UserOwnedStories
            var ownedStories = await ep._context.UserOwnedStories
                .Where(o => o.StoryDefinitionId == story.Id)
                .ExecuteDeleteAsync(ct);

            // Delete UserCreatedStories
            var createdStories = await ep._context.UserCreatedStories
                .Where(c => c.StoryDefinitionId == story.Id)
                .ExecuteDeleteAsync(ct);

            // Delete StoryPublicationAudits
            var audits = await ep._context.StoryPublicationAudits
                .Where(a => a.StoryDefinitionId == story.Id)
                .ExecuteDeleteAsync(ct);

            // Finally delete the story definition itself
            ep._context.StoryDefinitions.Remove(story);
            await ep._context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            ep._logger.LogWarning("Admin deleted story from DB: storyId={StoryId}, adminId={AdminId}, readProgress={ReadProgress}, readHistory={ReadHistory}, evaluationResults={EvaluationResults}, quizAnswers={QuizAnswers}",
                storyId, user.Id, readProgress, readHistory, evaluationResults, quizAnswers);

            return TypedResults.Ok(new AdminStoryActionResponse
            {
                Success = true,
                Message = $"Story deleted successfully. Removed: {readProgress} progress entries, {readHistory} history entries, {evaluationResults} evaluation results, {quizAnswers} quiz answers"
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            ep._logger.LogError(ex, "Error deleting story: storyId={StoryId}", storyId);
            return TypedResults.BadRequest($"Failed to delete story: {ex.Message}");
        }
    }

    private static string NormalizeStoryId(string storyId)
    {
        if (string.IsNullOrWhiteSpace(storyId))
            return storyId;
            
        if (string.Equals(storyId, "intro-puf-puf", StringComparison.OrdinalIgnoreCase))
            return "intro-pufpuf";
        return storyId;
    }
}

public record AdminStoryDetailsResponse
{
    public string StoryId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? CoverImageUrl { get; init; }
    public bool IsActive { get; init; }
    public string Status { get; init; } = string.Empty;
    public string StoryType { get; init; } = string.Empty;
    public bool IsEvaluative { get; init; }
    public int TotalTiles { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? PublishedAt { get; init; }
}

public record AdminStoryActionResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public record DeleteStoryRequest
{
    public string ConfirmStoryId { get; init; } = string.Empty;
}

