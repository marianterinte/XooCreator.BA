using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public interface IStoryReviewsRepository
{
    Task<StoryReviewDto?> CreateReviewAsync(Guid userId, string storyId, int rating, string? comment);
    Task<StoryReviewDto?> UpdateReviewAsync(Guid userId, Guid reviewId, int rating, string? comment);
    Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId);
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

    public async Task<StoryReviewDto?> CreateReviewAsync(Guid userId, string storyId, int rating, string? comment)
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
        await _context.SaveChangesAsync();

        return await MapToDtoAsync(review, userId);
    }

    public async Task<StoryReviewDto?> UpdateReviewAsync(Guid userId, Guid reviewId, int rating, string? comment)
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

        await _context.SaveChangesAsync();

        return await MapToDtoAsync(review, userId);
    }

    public async Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId)
    {
        var review = await _context.StoryReviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId && r.IsActive);

        if (review == null)
            return false;

        // Soft delete
        review.IsActive = false;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
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
        var reviews = await _context.StoryReviews
            .Where(r => r.StoryId == storyId && r.IsActive)
            .ToListAsync();

        var totalCount = reviews.Count;
        
        if (totalCount == 0)
        {
            return (0, 0, new Dictionary<int, int>());
        }

        var averageRating = reviews.Average(r => r.Rating);
        
        var ratingDistribution = reviews
            .GroupBy(r => r.Rating)
            .ToDictionary(g => g.Key, g => g.Count());

        // Ensure all ratings 1-5 are present in distribution
        for (int i = 1; i <= 5; i++)
        {
            if (!ratingDistribution.ContainsKey(i))
                ratingDistribution[i] = 0;
        }

        return (averageRating, totalCount, ratingDistribution);
    }

    public async Task<(double AverageRating, int TotalCount, Dictionary<int, int> RatingDistribution)> GetGlobalReviewStatisticsAsync()
    {
        var reviews = await _context.StoryReviews
            .Where(r => r.IsActive)
            .ToListAsync();

        var totalCount = reviews.Count;

        var ratingDistribution = reviews
            .GroupBy(r => r.Rating)
            .ToDictionary(g => g.Key, g => g.Count());

        for (int i = 1; i <= 5; i++)
        {
            if (!ratingDistribution.ContainsKey(i))
                ratingDistribution[i] = 0;
        }

        var averageRating = totalCount == 0 ? 0 : reviews.Average(r => r.Rating);
        return (averageRating, totalCount, ratingDistribution);
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

