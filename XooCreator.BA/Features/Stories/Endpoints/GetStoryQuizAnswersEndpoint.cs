using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.Stories.Endpoints;

public record QuizAnswerDto
{
    public required string TileId { get; init; }
    public required string SelectedAnswerId { get; init; }
    public bool IsCorrect { get; init; }
}

public record StoryQuizAnswersResponse
{
    public List<QuizAnswerDto> Answers { get; init; } = new();
}

[Endpoint]
public class GetStoryQuizAnswersEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContext;

    public GetStoryQuizAnswersEndpoint(
        XooDbContext context,
        IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    [Route("/api/{locale}/stories/{storyId}/quiz-answers")]
    [Authorize]
    public static async Task<Results<Ok<StoryQuizAnswersResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] GetStoryQuizAnswersEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        // Get all quiz answers for this user and story
        // We get the latest answers (most recent session) for each tile
        var allAnswers = await ep._context.StoryQuizAnswers
            .Where(a => a.UserId == userId.Value)
            .ToListAsync(ct);

        // Filter in memory for case-insensitive StoryId match
        var storyAnswers = allAnswers
            .Where(a => string.Equals(a.StoryId, storyId, StringComparison.OrdinalIgnoreCase))
            .GroupBy(a => a.TileId)
            .Select(g => g.OrderByDescending(a => a.AnsweredAt).First()) // Get most recent answer for each tile
            .ToList();

        var response = new StoryQuizAnswersResponse
        {
            Answers = storyAnswers.Select(a => new QuizAnswerDto
            {
                TileId = a.TileId,
                SelectedAnswerId = a.SelectedAnswerId,
                IsCorrect = a.IsCorrect
            }).ToList()
        };

        return TypedResults.Ok(response);
    }
}

