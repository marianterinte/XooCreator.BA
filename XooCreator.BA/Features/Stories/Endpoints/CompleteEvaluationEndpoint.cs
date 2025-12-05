using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.Stories.Repositories;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.Stories.Endpoints;

public record CompleteEvaluationRequest
{
    public required Guid SessionId { get; init; } // Session ID from quiz answers
}

public record QuizAnswerDetail
{
    public string TileId { get; init; } = string.Empty;
    public string Question { get; init; } = string.Empty;
    public string SelectedAnswerId { get; init; } = string.Empty;
    public string SelectedAnswerText { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
    public string? CorrectAnswerId { get; init; } // Only if incorrect
    public string? CorrectAnswerText { get; init; } // Only if incorrect
}

public record CompleteEvaluationResponse
{
    public bool Success { get; init; }
    public int TotalQuizzes { get; init; }
    public int CorrectAnswers { get; init; }
    public int ScorePercentage { get; init; } // 0-100
    public List<QuizAnswerDetail> QuizDetails { get; init; } = new(); // Breakdown per quiz
    public string? ErrorMessage { get; init; }
}

[Endpoint]
public class CompleteEvaluationEndpoint
{
    private readonly XooDbContext _context;
    private readonly IStoriesRepository _repository;
    private readonly IUserContextService _userContext;

    public CompleteEvaluationEndpoint(
        XooDbContext context,
        IStoriesRepository repository,
        IUserContextService userContext)
    {
        _context = context;
        _repository = repository;
        _userContext = userContext;
    }

    [Route("/api/{locale}/stories/{storyId}/complete-evaluation")]
    [Authorize]
    public static async Task<Results<Ok<CompleteEvaluationResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] CompleteEvaluationRequest request,
        [FromServices] CompleteEvaluationEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        // Load story (use current story version at completion time)
        var story = await ep._repository.GetStoryDefinitionByIdAsync(storyId);
        if (story == null)
            return TypedResults.BadRequest("Story not found");

        if (!story.IsEvaluative)
            return TypedResults.BadRequest("Story is not evaluative");

        // Count total quizzes
        var totalQuizzes = story.Tiles.Count(t => t.Type == "quiz");
        if (totalQuizzes == 0)
            return TypedResults.BadRequest("Story has no quizzes");

        // Get all answers for this session
        var answers = await ep._context.StoryQuizAnswers
            .Where(a => a.UserId == userId.Value
                     && a.StoryId == storyId
                     && a.SessionId == request.SessionId)
            .ToListAsync(ct);

        // Count correct answers
        var correctAnswers = answers.Count(a => a.IsCorrect);

        // Calculate score
        var scorePercentage = totalQuizzes > 0
            ? (int)Math.Round((double)correctAnswers / totalQuizzes * 100)
            : 0;

        // Build quiz details breakdown
        var quizDetails = new List<QuizAnswerDetail>();
        var quizTiles = story.Tiles.Where(t => t.Type == "quiz").ToList();

        foreach (var quizTile in quizTiles)
        {
            var answer = answers.FirstOrDefault(a => a.TileId == quizTile.TileId);
            var selectedAnswer = answer != null
                ? quizTile.Answers.FirstOrDefault(a => a.AnswerId == answer.SelectedAnswerId)
                : null;

            var correctAnswer = quizTile.Answers.FirstOrDefault(a => a.IsCorrect);

            // Get question text from translations or base
            var questionText = quizTile.Translations
                .FirstOrDefault(t => t.LanguageCode == locale.ToLowerInvariant())?.Question
                ?? quizTile.Question
                ?? string.Empty;

            // Get answer texts from translations or base
            var selectedAnswerText = selectedAnswer != null
                ? (selectedAnswer.Translations
                    .FirstOrDefault(t => t.LanguageCode == locale.ToLowerInvariant())?.Text
                    ?? selectedAnswer.Text)
                : string.Empty;

            var correctAnswerText = correctAnswer != null
                ? (correctAnswer.Translations
                    .FirstOrDefault(t => t.LanguageCode == locale.ToLowerInvariant())?.Text
                    ?? correctAnswer.Text)
                : null;

            quizDetails.Add(new QuizAnswerDetail
            {
                TileId = quizTile.TileId,
                Question = questionText,
                SelectedAnswerId = answer?.SelectedAnswerId ?? string.Empty,
                SelectedAnswerText = selectedAnswerText,
                IsCorrect = answer?.IsCorrect ?? false,
                CorrectAnswerId = answer?.IsCorrect == false ? correctAnswer?.AnswerId : null,
                CorrectAnswerText = answer?.IsCorrect == false ? correctAnswerText : null
            });
        }

        // Create evaluation result
        var result = new StoryEvaluationResult
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            StoryId = storyId,
            SessionId = request.SessionId,
            TotalQuizzes = totalQuizzes,
            CorrectAnswers = correctAnswers,
            ScorePercentage = scorePercentage,
            CompletedAt = DateTime.UtcNow
        };

        ep._context.StoryEvaluationResults.Add(result);
        await ep._context.SaveChangesAsync(ct);

        return TypedResults.Ok(new CompleteEvaluationResponse
        {
            Success = true,
            TotalQuizzes = totalQuizzes,
            CorrectAnswers = correctAnswers,
            ScorePercentage = scorePercentage,
            QuizDetails = quizDetails
        });
    }
}

