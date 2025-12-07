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
public class GetEvaluationDetailsEndpoint
{
    private readonly XooDbContext _context;
    private readonly IStoriesRepository _storiesRepository;
    private readonly IUserContextService _userContext;

    public GetEvaluationDetailsEndpoint(
        XooDbContext context,
        IStoriesRepository storiesRepository,
        IUserContextService userContext)
    {
        _context = context;
        _storiesRepository = storiesRepository;
        _userContext = userContext;
    }

    [Route("/api/{locale}/parent-dashboard/evaluative-stories/{storyId}/details")]
    [Authorize]
    public static async Task<Results<Ok<EvaluationDetailsResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] GetEvaluationDetailsEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        // Normalize storyId
        var normalizedStoryId = NormalizeStoryId(storyId);

        // Get story definition
        var story = await ep._storiesRepository.GetStoryDefinitionByIdAsync(normalizedStoryId);
        if (story == null || !story.IsActive)
            return TypedResults.BadRequest("Story not found");

        if (!story.IsEvaluative)
            return TypedResults.BadRequest("Story is not evaluative");

        // Get translation for the requested locale
        var translation = story.Translations
            .FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase))
            ?? story.Translations.FirstOrDefault();

        var title = translation?.Title ?? story.Title;

        // Get all evaluation results for this user (load once, filter in memory for case-insensitive match)
        var allUserResults = await ep._context.StoryEvaluationResults
            .Where(r => r.UserId == userId.Value)
            .ToListAsync(ct);

        // Get latest evaluation result for this user and story (filter in memory)
        var latestResult = allUserResults
            .Where(r => string.Equals(r.StoryId, story.StoryId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.CompletedAt)
            .FirstOrDefault();

        if (latestResult == null)
            return TypedResults.NotFound();

        // Get all quiz answers for this user and session (load once, filter in memory)
        var allUserQuizAnswers = await ep._context.StoryQuizAnswers
            .Where(a => a.UserId == userId.Value && a.SessionId == latestResult.SessionId)
            .ToListAsync(ct);

        // Filter in memory for case-insensitive StoryId match
        var quizAnswers = allUserQuizAnswers
            .Where(a => string.Equals(a.StoryId, story.StoryId, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Build quiz details
        var quizDetails = new List<QuizAnswerDetailDto>();
        var quizTiles = story.Tiles?.Where(t => t.Type == "quiz").ToList() ?? new List<XooCreator.BA.Data.StoryTile>();

        foreach (var quizTile in quizTiles)
        {
            var answer = quizAnswers.FirstOrDefault(a => string.Equals(a.TileId, quizTile.TileId, StringComparison.OrdinalIgnoreCase));
            
            // Get tile translation
            var tileTranslation = quizTile.Translations
                .FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase))
                ?? quizTile.Translations.FirstOrDefault();

            var question = tileTranslation?.Question ?? quizTile.Question ?? string.Empty;

            string selectedAnswerText = string.Empty;
            string? correctAnswerText = null;
            bool isCorrect = false;

            if (answer != null)
            {
                // Find selected answer
                var selectedAnswer = quizTile.Answers
                    .FirstOrDefault(a => string.Equals(a.AnswerId, answer.SelectedAnswerId, StringComparison.OrdinalIgnoreCase));

                if (selectedAnswer != null)
                {
                    var selectedAnswerTranslation = selectedAnswer.Translations
                        .FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase))
                        ?? selectedAnswer.Translations.FirstOrDefault();

                    selectedAnswerText = selectedAnswerTranslation?.Text ?? selectedAnswer.Text ?? string.Empty;
                    isCorrect = answer.IsCorrect;
                }

                // If incorrect, find correct answer
                if (!isCorrect)
                {
                    var correctAnswer = quizTile.Answers.FirstOrDefault(a => a.IsCorrect);
                    if (correctAnswer != null)
                    {
                        var correctAnswerTranslation = correctAnswer.Translations
                            .FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase))
                            ?? correctAnswer.Translations.FirstOrDefault();

                        correctAnswerText = correctAnswerTranslation?.Text ?? correctAnswer.Text;
                    }
                }
            }

            quizDetails.Add(new QuizAnswerDetailDto
            {
                TileId = quizTile.TileId,
                Question = question,
                SelectedAnswerText = selectedAnswerText,
                IsCorrect = isCorrect,
                CorrectAnswerText = correctAnswerText
            });
        }

        var resultDto = new EvaluationResultDto
        {
            ScorePercentage = latestResult.ScorePercentage,
            CorrectAnswers = latestResult.CorrectAnswers,
            TotalQuizzes = latestResult.TotalQuizzes,
            CompletedAt = latestResult.CompletedAt
        };

        return TypedResults.Ok(new EvaluationDetailsResponse
        {
            StoryId = story.StoryId, // Use normalized storyId from database
            Title = title,
            LatestResult = resultDto,
            QuizDetails = quizDetails
        });
    }

    private static string NormalizeStoryId(string storyId)
    {
        if (string.Equals(storyId, "intro-puf-puf", StringComparison.OrdinalIgnoreCase))
            return "intro-pufpuf";
        return storyId;
    }
}
