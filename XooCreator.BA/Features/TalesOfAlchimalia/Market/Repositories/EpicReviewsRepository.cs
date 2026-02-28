using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public interface IEpicReviewsRepository
{
    Task<EpicReviewDto?> CreateReviewAsync(Guid userId, string epicId, int rating, string? comment);
    Task<EpicReviewDto?> UpdateReviewAsync(Guid userId, Guid reviewId, int rating, string? comment);
    Task<bool> DeleteReviewAsync(Guid userId, Guid reviewId);
    Task<EpicReviewDto?> GetUserReviewAsync(Guid userId, string epicId);
    Task<GetEpicReviewsResponse> GetEpicReviewsAsync(string epicId, Guid? currentUserId, GetEpicReviewsRequest request);
    Task<(double AverageRating, int TotalCount, Dictionary<int, int> RatingDistribution)> GetReviewStatisticsAsync(string epicId);
}

public class EpicReviewsRepository : IEpicReviewsRepository
{
    private readonly XooDbContext _context;

    public EpicReviewsRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<EpicReviewDto?> CreateReviewAsync(Guid userId, string epicId, int rating, string? comment)
    {
        // Validate rating
        if (rating < 1 || rating > 5)
            return null;

        // Check if user already has a review for this epic
        var existingReview = await _context.EpicReviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EpicId == epicId && r.IsActive);

        if (existingReview != null)
            return null; // User already has a review

        // Verify epic exists and is published (use StoryEpicDefinitions)
        var epicExists = await _context.StoryEpicDefinitions
            .AnyAsync(e => e.Id == epicId && e.Status == "published");
        
        if (!epicExists)
            return null;

        var review = new EpicReview
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EpicId = epicId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.EpicReviews.Add(review);
        await _context.SaveChangesAsync();

        return await MapToDtoAsync(review, userId);
    }

    public async Task<EpicReviewDto?> UpdateReviewAsync(Guid userId, Guid reviewId, int rating, string? comment)
    {
        // Validate rating
        if (rating < 1 || rating > 5)
            return null;

        var review = await _context.EpicReviews
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
        var review = await _context.EpicReviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId && r.IsActive);

        if (review == null)
            return false;

        // Soft delete
        review.IsActive = false;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<EpicReviewDto?> GetUserReviewAsync(Guid userId, string epicId)
    {
        var review = await _context.EpicReviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EpicId == epicId && r.IsActive);

        if (review == null)
            return null;

        return await MapToDtoAsync(review, userId);
    }

    public async Task<GetEpicReviewsResponse> GetEpicReviewsAsync(string epicId, Guid? currentUserId, GetEpicReviewsRequest request)
    {
        var query = _context.EpicReviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.EpicId == epicId && r.IsActive);

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
        var stats = await GetReviewStatisticsAsync(epicId);

        var reviewDtos = new List<EpicReviewDto>();
        foreach (var review in reviews)
        {
            var dto = await MapToDtoAsync(review, currentUserId);
            if (dto != null)
                reviewDtos.Add(dto);
        }

        return new GetEpicReviewsResponse
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

    public async Task<(double AverageRating, int TotalCount, Dictionary<int, int> RatingDistribution)> GetReviewStatisticsAsync(string epicId)
    {
        var stats = await _context.EpicReviews
            .AsNoTracking()
            .Where(r => r.EpicId == epicId && r.IsActive)
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

    private async Task<EpicReviewDto?> MapToDtoAsync(EpicReview review, Guid? currentUserId)
    {
        if (review.User == null)
        {
            // Load user if not included
            review.User = await _context.AlchimaliaUsers
                .FirstOrDefaultAsync(u => u.Id == review.UserId);
        }

        if (review.User == null)
            return null;

        return new EpicReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            UserName = review.User.Name,
            UserPicture = review.User.Picture,
            EpicId = review.EpicId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            IsOwnReview = currentUserId.HasValue && review.UserId == currentUserId.Value
        };
    }
}

