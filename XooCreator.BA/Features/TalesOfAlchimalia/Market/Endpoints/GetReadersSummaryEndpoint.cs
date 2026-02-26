using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class GetReadersSummaryEndpoint
{
    private readonly XooDbContext _context;
    private readonly IStoriesMarketplaceService _marketplaceService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<GetReadersSummaryEndpoint> _logger;

    public GetReadersSummaryEndpoint(
        XooDbContext context,
        IStoriesMarketplaceService marketplaceService,
        IAuth0UserService auth0,
        ILogger<GetReadersSummaryEndpoint> logger)
    {
        _context = context;
        _marketplaceService = marketplaceService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/{locale}/admin/reports/readers/summary")]
    [Authorize]
    public static async Task<Results<Ok<ReadersSummaryResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetReadersSummaryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        if (!ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        try
        {
            var totalReaders = await ep._marketplaceService.GetTotalReadersAsync();
            var leaderboard = await ep.BuildLeaderboardAsync(ct);
            var trendPoints = await ep._marketplaceService.GetReadersTrendAsync(7);
            var trend = trendPoints
                .Select(tp => new ReadersTrendPointDto(tp.Date.ToString("yyyy-MM-dd"), tp.ReadersCount))
                .ToList();

            var correlationRaw = await ep._marketplaceService.GetReadersVsReviewsAsync(10);
            var correlation = correlationRaw
                .Select(item => new ReadersCorrelationItemDto(
                    item.StoryId,
                    item.Title,
                    item.ReadersCount,
                    item.ReviewsCount,
                    item.AverageRating))
                .ToList();

            var response = new ReadersSummaryResponse
            {
                Success = true,
                ErrorMessage = null,
                TotalReaders = totalReaders,
                TopStories = leaderboard,
                Trend = trend,
                RatingCorrelation = correlation
            };

            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            var errorDetails = $"Exception: {ex.GetType().Name}\n" +
                              $"Message: {ex.Message}\n" +
                              $"StackTrace: {ex.StackTrace}\n" +
                              (ex.InnerException != null ? $"InnerException: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}\n" : "");

            ep._logger.LogError(ex, "GetReadersSummaryEndpoint failed: {ErrorDetails}", errorDetails);

            var errorResponse = new ReadersSummaryResponse
            {
                Success = false,
                ErrorMessage = errorDetails,
                TotalReaders = 0,
                TopStories = new List<ReadersLeaderboardItem>(),
                Trend = new List<ReadersTrendPointDto>(),
                RatingCorrelation = new List<ReadersCorrelationItemDto>()
            };

            return TypedResults.Ok(errorResponse);
        }
    }

    private async Task<List<ReadersLeaderboardItem>> BuildLeaderboardAsync(CancellationToken ct)
    {
        var aggregates = await _marketplaceService.GetTopStoriesByReadersAsync(10);
        if (aggregates.Count == 0)
        {
            return new List<ReadersLeaderboardItem>();
        }

        var storyIds = aggregates.Select(a => a.StoryId).ToList();

        var stories = await _context.StoryDefinitions
            .Where(sd => storyIds.Contains(sd.StoryId))
            .Select(sd => new { sd.StoryId, sd.Title })
            .ToListAsync(ct);

        var ratings = await _context.StoryReviews
            .Where(r => storyIds.Contains(r.StoryId))
            .GroupBy(r => r.StoryId)
            .Select(g => new { StoryId = g.Key, Average = g.Count() > 0 ? (double?)g.Average(r => (double)r.Rating) : null })
            .ToListAsync(ct);

        var storyMap = stories.ToDictionary(s => s.StoryId, s => s.Title ?? s.StoryId);
        var ratingMap = ratings.ToDictionary(r => r.StoryId, r => r.Average.HasValue ? Math.Round(r.Average.Value, 2) : 0.0);

        return aggregates
            .Select(a =>
            {
                storyMap.TryGetValue(a.StoryId, out var title);
                ratingMap.TryGetValue(a.StoryId, out var averageRating);
                return new ReadersLeaderboardItem(
                    a.StoryId,
                    title ?? a.StoryId,
                    a.ReadersCount,
                    averageRating);
            })
            .ToList();
    }
}

