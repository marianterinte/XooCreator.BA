using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

public interface IEpicReviewsService
{
    Task<CreateEpicReviewResponse> CreateReviewAsync(Guid userId, CreateEpicReviewRequest request);
    Task<UpdateEpicReviewResponse> UpdateReviewAsync(Guid userId, UpdateEpicReviewRequest request);
    Task<DeleteEpicReviewResponse> DeleteReviewAsync(Guid userId, Guid reviewId);
    Task<GetEpicReviewsResponse> GetEpicReviewsAsync(string epicId, Guid? currentUserId, GetEpicReviewsRequest request);
}

public class EpicReviewsService : IEpicReviewsService
{
    private readonly IEpicReviewsRepository _repository;

    public EpicReviewsService(IEpicReviewsRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateEpicReviewResponse> CreateReviewAsync(Guid userId, CreateEpicReviewRequest request)
    {
        try
        {
            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                return new CreateEpicReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Rating must be between 1 and 5"
                };
            }

            // Validate comment length if provided
            if (!string.IsNullOrEmpty(request.Comment) && request.Comment.Length > 2000)
            {
                return new CreateEpicReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Comment cannot exceed 2000 characters"
                };
            }

            var review = await _repository.CreateReviewAsync(userId, request.EpicId, request.Rating, request.Comment);

            if (review == null)
            {
                return new CreateEpicReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to create review. You may already have a review for this epic."
                };
            }

            return new CreateEpicReviewResponse
            {
                Success = true,
                Review = review
            };
        }
        catch (Exception ex)
        {
            return new CreateEpicReviewResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while creating the review"
            };
        }
    }

    public async Task<UpdateEpicReviewResponse> UpdateReviewAsync(Guid userId, UpdateEpicReviewRequest request)
    {
        try
        {
            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                return new UpdateEpicReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Rating must be between 1 and 5"
                };
            }

            // Validate comment length if provided
            if (!string.IsNullOrEmpty(request.Comment) && request.Comment.Length > 2000)
            {
                return new UpdateEpicReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Comment cannot exceed 2000 characters"
                };
            }

            var review = await _repository.UpdateReviewAsync(userId, request.ReviewId, request.Rating, request.Comment);

            if (review == null)
            {
                return new UpdateEpicReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Review not found or you don't have permission to update it"
                };
            }

            return new UpdateEpicReviewResponse
            {
                Success = true,
                Review = review
            };
        }
        catch (Exception ex)
        {
            return new UpdateEpicReviewResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while updating the review"
            };
        }
    }

    public async Task<DeleteEpicReviewResponse> DeleteReviewAsync(Guid userId, Guid reviewId)
    {
        try
        {
            var success = await _repository.DeleteReviewAsync(userId, reviewId);

            if (!success)
            {
                return new DeleteEpicReviewResponse
                {
                    Success = false,
                    ErrorMessage = "Review not found or you don't have permission to delete it"
                };
            }

            return new DeleteEpicReviewResponse
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new DeleteEpicReviewResponse
            {
                Success = false,
                ErrorMessage = "An error occurred while deleting the review"
            };
        }
    }

    public async Task<GetEpicReviewsResponse> GetEpicReviewsAsync(string epicId, Guid? currentUserId, GetEpicReviewsRequest request)
    {
        try
        {
            return await _repository.GetEpicReviewsAsync(epicId, currentUserId, request);
        }
        catch (Exception ex)
        {
            return new GetEpicReviewsResponse
            {
                Reviews = new List<EpicReviewDto>(),
                TotalCount = 0,
                Page = request.Page,
                PageSize = request.PageSize,
                HasMore = false,
                AverageRating = 0,
                RatingDistribution = new Dictionary<int, int>()
            };
        }
    }
}

