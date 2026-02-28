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

public record SubmitQuizAnswerRequest
{
    public required string TileId { get; init; }        // e.g., "q1"
    public required string SelectedAnswerId { get; init; } // e.g., "a", "b", "c"
    public Guid? SessionId { get; init; }               // Optional: for grouping answers
}

public record SubmitQuizAnswerResponse
{
    public bool Success { get; init; }
    public bool IsCorrect { get; init; }
    public Guid SessionId { get; init; }  // Returned session ID (new or existing)
    public string? ErrorMessage { get; init; }
}

[Endpoint]
public class SubmitQuizAnswerEndpoint
{
    private readonly XooDbContext _context;
    private readonly IStoriesRepository _repository;
    private readonly IUserContextService _userContext;
    private readonly ILogger<SubmitQuizAnswerEndpoint> _logger;

    public SubmitQuizAnswerEndpoint(
        XooDbContext context,
        IStoriesRepository repository,
        IUserContextService userContext,
        ILogger<SubmitQuizAnswerEndpoint> logger)
    {
        _context = context;
        _repository = repository;
        _userContext = userContext;
        _logger = logger;
    }

    [Route("/api/{locale}/stories/{storyId}/quiz-answer")]
    [Authorize]
    public static async Task<Results<Ok<SubmitQuizAnswerResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] SubmitQuizAnswerRequest request,
        [FromServices] SubmitQuizAnswerEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var sessionId = request.SessionId ?? Guid.NewGuid();

        // Load story - always get fresh version to ensure correct IsCorrect values
        var story = await ep._repository.GetStoryDefinitionByIdAsync(storyId);
        if (story == null)
            return TypedResults.BadRequest("Story not found");
        
        // Debug: Log story load
        ep._logger.LogDebug(
            "SubmitQuizAnswer: Loaded story {StoryId} for tile {TileId}",
            storyId, request.TileId);

        // Find quiz tile
        var quizTile = story.Tiles.FirstOrDefault(t => t.TileId == request.TileId && t.Type == "quiz");
        if (quizTile == null)
            return TypedResults.BadRequest("Quiz tile not found");

        // Find selected answer
        var selectedAnswer = quizTile.Answers.FirstOrDefault(a => a.AnswerId == request.SelectedAnswerId);
        if (selectedAnswer == null)
            return TypedResults.BadRequest("Answer not found");

        // Debug: Log all answers in quiz tile to verify IsCorrect values
        foreach (var answer in quizTile.Answers)
        {
            ep._logger.LogDebug(
                "Quiz tile answer: TileId={TileId} AnswerId={AnswerId} IsCorrect={IsCorrect}",
                request.TileId, answer.AnswerId, answer.IsCorrect);
        }

        // Check if correct (isCorrect flag from entity)
        var isCorrect = selectedAnswer.IsCorrect;
        
        ep._logger.LogDebug(
            "Submit quiz answer: TileId={TileId} SelectedAnswerId={SelectedAnswerId} IsCorrect={IsCorrect}",
            request.TileId, request.SelectedAnswerId, isCorrect);

        // Check if answer already exists for this session (overwrite)
        var existingAnswer = await ep._context.StoryQuizAnswers
            .FirstOrDefaultAsync(a => a.UserId == userId.Value
                                    && a.StoryId == storyId
                                    && a.TileId == request.TileId
                                    && a.SessionId == sessionId, ct);

        if (existingAnswer != null)
        {
            // CRITICAL FIX: Always recalculate IsCorrect from current story version
            // This ensures we use the correct IsCorrect value even if story was republished
            var oldIsCorrect = existingAnswer.IsCorrect;
            existingAnswer.SelectedAnswerId = request.SelectedAnswerId;
            existingAnswer.IsCorrect = isCorrect; // Recalculated from current story version
            existingAnswer.AnsweredAt = DateTime.UtcNow;
            
            ep._logger.LogDebug(
                "Updated existing quiz answer: TileId={TileId} SelectedAnswerId={SelectedAnswerId} OldIsCorrect={OldIsCorrect} NewIsCorrect={NewIsCorrect}",
                request.TileId, request.SelectedAnswerId, oldIsCorrect, isCorrect);
        }
        else
        {
            // Create new answer
            var quizAnswer = new StoryQuizAnswer
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                StoryId = storyId,
                TileId = request.TileId,
                SelectedAnswerId = request.SelectedAnswerId,
                IsCorrect = isCorrect,
                SessionId = sessionId,
                AnsweredAt = DateTime.UtcNow
            };

            ep._context.StoryQuizAnswers.Add(quizAnswer);
            
            ep._logger.LogDebug(
                "Created new quiz answer: TileId={TileId} SelectedAnswerId={SelectedAnswerId} IsCorrect={IsCorrect}",
                request.TileId, request.SelectedAnswerId, isCorrect);
        }

        await ep._context.SaveChangesAsync(ct);

        return TypedResults.Ok(new SubmitQuizAnswerResponse
        {
            Success = true,
            IsCorrect = isCorrect,
            SessionId = sessionId
        });
    }
}

