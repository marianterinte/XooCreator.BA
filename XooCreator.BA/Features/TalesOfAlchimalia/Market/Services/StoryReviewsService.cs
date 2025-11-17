using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

public interface IStoryReviewsService
{
    Task<CreateStoryReviewResponse> CreateReviewAsync(Guid userId, CreateStoryReviewRequest request);
    Task<UpdateStoryReviewResponse> UpdateReviewAsync(Guid userId, UpdateStoryReviewRequest request);
    Task<DeleteStoryReviewResponse> DeleteReviewAsync(Guid userId, Guid reviewId);
    Task<GetStoryReviewsResponse> GetStoryReviewsAsync(string storyId, Guid? currentUserId, GetStoryReviewsRequest request);
    Task<GlobalReviewStatisticsResponse> GetGlobalReviewStatisticsAsync();
}

public class StoryReviewsService : IStoryReviewsService
{
    private readonly IStoryReviewsRepository _repository;

    public StoryReviewsService(IStoryReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateStoryReviewResponse> CreateReviewAsync(Guid userId, CreateStoryReviewRequest request)
    {
        try
        {
            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                return new CreateStoryReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Rating must be between 1 and 5"
                };
            }

            // Validate comment length if provided
            if (!string.IsNullOrEmpty(request.Comment) && request.Comment.Length > 2000)
            {
                return new CreateStoryReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Comment cannot exceed 2000 characters"
                };
            }

            var review = await _repository.CreateReviewAsync(userId, request.StoryId, request.Rating, request.Comment);

            if (review == null)
            {
                return new CreateStoryReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to create review. You may already have a review for this story."
                };
            }

            return new CreateStoryReviewResponse
            {
                Success = true,
                Review = review
            };
        }
        catch (Exception ex)
        {
            return new CreateStoryReviewResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while creating the review"
            };
        }
    }

    public async Task<UpdateStoryReviewResponse> UpdateReviewAsync(Guid userId, UpdateStoryReviewRequest request)
    {
        try
        {
            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                return new UpdateStoryReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Rating must be between 1 and 5"
                };
            }

            // Validate comment length if provided
            if (!string.IsNullOrEmpty(request.Comment) && request.Comment.Length > 2000)
            {
                return new UpdateStoryReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Comment cannot exceed 2000 characters"
                };
            }

            var review = await _repository.UpdateReviewAsync(userId, request.ReviewId, request.Rating, request.Comment);

            if (review == null)
            {
                return new UpdateStoryReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Review not found or you don't have permission to update it"
                };
            }

            return new UpdateStoryReviewResponse
            {
                Success = true,
                Review = review
            };
        }
        catch (Exception ex)
        {
            return new UpdateStoryReviewResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while updating the review"
            };
        }
    }

    public async Task<DeleteStoryReviewResponse> DeleteReviewAsync(Guid userId, Guid reviewId)
    {
        try
        {
            var success = await _repository.DeleteReviewAsync(userId, reviewId);

            if (!success)
            {
                return new DeleteStoryReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Review not found or you don't have permission to delete it"
                };
            }

            return new DeleteStoryReviewResponse
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new DeleteStoryReviewResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while deleting the review"
            };
        }
    }

    public async Task<GetStoryReviewsResponse> GetStoryReviewsAsync(string storyId, Guid? currentUserId, GetStoryReviewsRequest request)
    {
        try
        {
            return await _repository.GetStoryReviewsAsync(storyId, currentUserId, request);
        }
        catch (Exception ex)
        {
            return new GetStoryReviewsResponse
            {
                Reviews = new List<StoryReviewDto>(),
                TotalCount = 0,
                Page = request.Page,
                PageSize = request.PageSize,
                HasMore = false,
                AverageRating = 0,
                RatingDistribution = new Dictionary<int, int>()
            };
        }
    }

    public async Task<GlobalReviewStatisticsResponse> GetGlobalReviewStatisticsAsync()
    {
        try
        {
            var stats = await _repository.GetGlobalReviewStatisticsAsync();
            return new GlobalReviewStatisticsResponse
            {
                Success = true,
                TotalReviews = stats.TotalCount,
                AverageRating = stats.AverageRating,
                RatingDistribution = stats.RatingDistribution
            };
        }
        catch (Exception ex)
        {
            return new GlobalReviewStatisticsResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while loading review statistics",
                RatingDistribution = new Dictionary<int, int>()
            };
        }
    }
}

