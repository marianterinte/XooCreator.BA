using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public interface IStoryReviewsRepository
{
    Task<StoryReviewDto?> CreateReviewAsync(Guid userId, string storyId, int rating, string? comment, CancellationToken ct = default);
    Task<StoryReviewDto?> UpdateReviewAsync(Guid userId, Guid reviewId, int rating, string? comment, CancellationToken ct = default);
    Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId, CancellationToken ct = default);
    Task<StoryReviewDto?> GetUserReviewAsync(Guid userId, string storyId);
    Task<GetStoryReviewsResponse> GetStoryReviewsAsync(string storyId, Guid? currentUserId, GetStoryReviewsRequest request);
    Task<(double AverageRating, int TotalCount, Dictionary<int, int> RatingDistribution)> GetReviewStatisticsAsync(string storyId);
    Task<(double AverageRating, int TotalCount, Dictionary<int, int> RatingDistribution)> GetGlobalReviewStatisticsAsync();
}

public class StoryReviewsRepository : IStoryReviewsRepository
{
    private readonly XooDbContext _context;

    public StoryReviewsRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<StoryReviewDto?> CreateReviewAsync(Guid userId, string storyId, int rating, string? comment, CancellationToken ct = default)
    {
        // Validate rating
        if (rating < 1 || rating > 5)
            return null;

        // Check if user already has a review for this story
        var existingReview = await _context.StoryReviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.StoryId == storyId && r.IsActive);

        if (existingReview != null)
            return null; // User already has a review

        // Verify story exists
        var storyExists = await _context.StoryDefinitions
            .AnyAsync(s => s.StoryId == storyId && s.IsActive);
        
        if (!storyExists)
            return null;

        var review = new StoryReview
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StoryId = storyId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.StoryReviews.Add(review);
        await _context.SaveChangesAsync(ct);

        return await MapToDtoAsync(review, userId);
    }

    public async Task<StoryReviewDto?> UpdateReviewAsync(Guid userId, Guid reviewId, int rating, string? comment, CancellationToken ct = default)
    {
        // Validate rating
        if (rating < 1 || rating > 5)
            return null;

        var review = await _context.StoryReviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId && r.IsActive);

        if (review == null)
            return null;

        review.Rating = rating;
        review.Comment = comment;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await MapToDtoAsync(review, userId);
    }

    public async Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId, CancellationToken ct = default)
    {
        var review = await _context.StoryReviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId && r.IsActive);

        if (review == null)
            return false;

        // Soft delete
        review.IsActive = false;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<StoryReviewDto?> GetUserReviewAsync(Guid userId, string storyId)
    {
        var review = await _context.StoryReviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.StoryId == storyId && r.IsActive);

        if (review == null)
            return null;

        return await MapToDtoAsync(review, userId);
    }

    public async Task<GetStoryReviewsResponse> GetStoryReviewsAsync(string storyId, Guid? currentUserId, GetStoryReviewsRequest request)
    {
        var query = _context.StoryReviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.StoryId == storyId && r.IsActive);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "rating" => request.SortOrder == "asc" 
                ? query.OrderBy(r => r.Rating).ThenByDescending(r => r.CreatedAt)
                : query.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAt),
            _ => request.SortOrder == "asc"
                ? query.OrderBy(r => r.CreatedAt)
                : query.OrderByDescending(r => r.CreatedAt)
        };

        // Apply pagination
        var reviews = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // Get statistics
        var stats = await GetReviewStatisticsAsync(storyId);

        var reviewDtos = new List<StoryReviewDto>();
        foreach (var review in reviews)
        {
            var dto = await MapToDtoAsync(review, currentUserId);
            if (dto != null)
                reviewDtos.Add(dto);
        }

        return new GetStoryReviewsResponse
        {
            Reviews = reviewDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            HasMore = totalCount > request.Page * request.PageSize,
            AverageRating = stats.AverageRating,
            RatingDistribution = stats.RatingDistribution
        };
    }

    public async Task<(double AverageRating, int TotalCount, Dictionary<int, int> RatingDistribution)> GetReviewStatisticsAsync(string storyId)
    {
        var stats = await _context.StoryReviews
            .AsNoTracking()
            .Where(r => r.StoryId == storyId && r.IsActive)
            .GroupBy(r => r.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalCount = stats.Sum(s => s.Count);
        var averageRating = totalCount > 0
            ? stats.Sum(s => s.Rating * s.Count) / (double)totalCount
            : 0;

        var ratingDistribution = Enumerable.Range(1, 5)
            .ToDictionary(r => r, r => stats.FirstOrDefault(s => s.Rating == r)?.Count ?? 0);

        return (Math.Round(averageRating, 1), totalCount, ratingDistribution);
    }

    public async Task<(double AverageRating, int TotalCount, Dictionary<int, int> RatingDistribution)> GetGlobalReviewStatisticsAsync()
    {
        var stats = await _context.StoryReviews
            .AsNoTracking()
            .Where(r => r.IsActive)
            .GroupBy(r => r.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync();

        var totalCount = stats.Sum(s => s.Count);
        var averageRating = totalCount > 0
            ? stats.Sum(s => s.Rating * s.Count) / (double)totalCount
            : 0;

        var ratingDistribution = Enumerable.Range(1, 5)
            .ToDictionary(r => r, r => stats.FirstOrDefault(s => s.Rating == r)?.Count ?? 0);

        return (Math.Round(averageRating, 1), totalCount, ratingDistribution);
    }

    private async Task<StoryReviewDto?> MapToDtoAsync(StoryReview review, Guid? currentUserId)
    {
        if (review.User == null)
        {
            // Load user if not included
            review.User = await _context.AlchimaliaUsers
                .FirstOrDefaultAsync(u => u.Id == review.UserId);
        }

        if (review.User == null)
            return null;

        return new StoryReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            UserName = review.User.Name,
            UserPicture = review.User.Picture,
            StoryId = review.StoryId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            IsOwnReview = currentUserId.HasValue && review.UserId == currentUserId.Value
        };
    }
}

