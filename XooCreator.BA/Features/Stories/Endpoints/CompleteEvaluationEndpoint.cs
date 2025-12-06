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
    private readonly ILogger<CompleteEvaluationEndpoint> _logger;

    public CompleteEvaluationEndpoint(
        XooDbContext context,
        IStoriesRepository repository,
        IUserContextService userContext,
        ILogger<CompleteEvaluationEndpoint> logger)
    {
        _context = context;
        _repository = repository;
        _userContext = userContext;
        _logger = logger;
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
        // CRITICAL: Force fresh load to avoid any caching issues
        var story = await ep._repository.GetStoryDefinitionByIdAsync(storyId);
        if (story == null)
            return TypedResults.BadRequest("Story not found");
        
        // Debug: Log story version info
        ep._logger.LogInformation(
            "CompleteEvaluation: Loaded story {StoryId} with {TileCount} tiles, {QuizCount} quizzes",
            storyId, story.Tiles.Count, story.Tiles.Count(t => t.Type == "quiz"));

        if (!story.IsEvaluative)
            return TypedResults.BadRequest("Story is not evaluative");

        // Count total quizzes
        var totalQuizzes = story.Tiles.Count(t => t.Type == "quiz");
        if (totalQuizzes == 0)
            return TypedResults.BadRequest("Story has no quizzes");

        // Get all answers for this session - we only need SelectedAnswerId, not IsCorrect
        // CRITICAL: We will recalculate IsCorrect from current story version, not from stored values
        var answers = await ep._context.StoryQuizAnswers
            .AsNoTracking()
            .Where(a => a.UserId == userId.Value
                     && a.StoryId == storyId
                     && a.SessionId == request.SessionId)
            .Select(a => new { a.TileId, a.SelectedAnswerId, a.Id })
            .ToListAsync(ct);

        // Debug: Log all answers to verify what we have
        ep._logger.LogInformation(
            "CompleteEvaluation: Found {Count} answers for session {SessionId}",
            answers.Count, request.SessionId);
        
        foreach (var answer in answers)
        {
            ep._logger.LogInformation(
                "Quiz answer (from DB): TileId={TileId} SelectedAnswerId={SelectedAnswerId}",
                answer.TileId, answer.SelectedAnswerId);
        }

        // Build quiz details breakdown
        var quizDetails = new List<QuizAnswerDetail>();
        var quizTiles = story.Tiles.Where(t => t.Type == "quiz").ToList();

        // CRITICAL FIX: Always recalculate IsCorrect from current story version
        // NEVER trust stored IsCorrect values - they may be stale
        int correctAnswers = 0;
        var answersToUpdate = new List<(Guid Id, bool NewIsCorrect)>();

        foreach (var quizTile in quizTiles)
        {
            var answer = answers.FirstOrDefault(a => a.TileId == quizTile.TileId);
            var selectedAnswer = answer != null
                ? quizTile.Answers.FirstOrDefault(a => a.AnswerId == answer.SelectedAnswerId)
                : null;

            // Debug: Log all answers in quiz tile to verify IsCorrect values
            foreach (var quizAnswer in quizTile.Answers)
            {
                ep._logger.LogInformation(
                    "Quiz tile answer: TileId={TileId} AnswerId={AnswerId} IsCorrect={IsCorrect}",
                    quizTile.TileId, quizAnswer.AnswerId, quizAnswer.IsCorrect);
            }

            // CRITICAL: Always recalculate IsCorrect from current story version
            // Do NOT use stored IsCorrect value - it may be stale
            bool isCorrect = false;
            if (selectedAnswer != null)
            {
                isCorrect = selectedAnswer.IsCorrect;
                
                // Track answers that need updating
                if (answer != null)
                {
                    answersToUpdate.Add((answer.Id, isCorrect));
                }
                
                ep._logger.LogInformation(
                    "Recalculated IsCorrect: TileId={TileId} SelectedAnswerId={SelectedAnswerId} IsCorrect={IsCorrect}",
                    quizTile.TileId, answer?.SelectedAnswerId ?? "none", isCorrect);
            }
            else if (answer != null)
            {
                ep._logger.LogWarning(
                    "Selected answer not found in current story: TileId={TileId} SelectedAnswerId={SelectedAnswerId}",
                    quizTile.TileId, answer.SelectedAnswerId);
            }

            // Find correct answer - must have IsCorrect = true
            var correctAnswer = quizTile.Answers.FirstOrDefault(a => a.IsCorrect);
            
            // Debug: Log if no correct answer found
            if (correctAnswer == null)
            {
                ep._logger.LogWarning(
                    "No correct answer found for quiz tile: TileId={TileId} TotalAnswers={TotalAnswers}",
                    quizTile.TileId, quizTile.Answers.Count);
            }

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
            
            // Debug: Log answer details
            ep._logger.LogInformation(
                "Quiz detail: TileId={TileId} HasAnswer={HasAnswer} IsCorrect={IsCorrect} SelectedText={SelectedText} CorrectText={CorrectText}",
                quizTile.TileId,
                answer != null,
                isCorrect,
                selectedAnswerText ?? "null",
                correctAnswerText ?? "null");

            quizDetails.Add(new QuizAnswerDetail
            {
                TileId = quizTile.TileId,
                Question = questionText,
                SelectedAnswerId = answer?.SelectedAnswerId ?? string.Empty,
                SelectedAnswerText = selectedAnswerText,
                IsCorrect = isCorrect, // Always use recalculated value
                CorrectAnswerId = !isCorrect ? correctAnswer?.AnswerId : null,
                CorrectAnswerText = !isCorrect ? correctAnswerText : null
            });
            
            // Count correct answers as we build details
            if (isCorrect)
            {
                correctAnswers++;
            }
        }
        
        // Update all stored IsCorrect values to match current story version
        if (answersToUpdate.Any())
        {
            foreach (var (id, newIsCorrect) in answersToUpdate)
            {
                var answerToUpdate = await ep._context.StoryQuizAnswers
                    .FirstOrDefaultAsync(a => a.Id == id, ct);
                
                if (answerToUpdate != null)
                {
                    var oldIsCorrect = answerToUpdate.IsCorrect;
                    answerToUpdate.IsCorrect = newIsCorrect;
                    
                    if (oldIsCorrect != newIsCorrect)
                    {
                        ep._logger.LogInformation(
                            "Updated stored IsCorrect: AnswerId={AnswerId} OldIsCorrect={OldIsCorrect} NewIsCorrect={NewIsCorrect}",
                            id, oldIsCorrect, newIsCorrect);
                    }
                }
            }
            
            await ep._context.SaveChangesAsync(ct);
        }
        
        // Debug: Log summary
        ep._logger.LogInformation(
            "Evaluation summary: TotalQuizzes={TotalQuizzes} TotalAnswers={TotalAnswers} CorrectAnswers={CorrectAnswers} ScorePercentage={ScorePercentage}",
            totalQuizzes, answers.Count, correctAnswers, 
            totalQuizzes > 0 ? (int)Math.Round((double)correctAnswers / totalQuizzes * 100) : 0);

        // Calculate score
        var scorePercentage = totalQuizzes > 0
            ? (int)Math.Round((double)correctAnswers / totalQuizzes * 100)
            : 0;

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

