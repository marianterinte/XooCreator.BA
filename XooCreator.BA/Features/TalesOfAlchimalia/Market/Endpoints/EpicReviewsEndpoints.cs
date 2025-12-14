using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class CreateEpicReviewEndpoint
{
    private readonly IEpicReviewsService _reviewsService;
    private readonly IUserContextService _userContext;

    public CreateEpicReviewEndpoint(IEpicReviewsService reviewsService, IUserContextService userContext)
    {
        _reviewsService = reviewsService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/reviews")]
    [Authorize]
    public static async Task<Results<Ok<CreateEpicReviewResponse>, BadRequest<CreateEpicReviewResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] CreateEpicReviewRequest request,
        [FromServices] CreateEpicReviewEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var response = await ep._reviewsService.CreateReviewAsync(userId.Value, request);
        
        if (!response.Success)
            return TypedResults.BadRequest(response);

        return TypedResults.Ok(response);
    }
}

[Endpoint]
public class UpdateEpicReviewEndpoint
{
    private readonly IEpicReviewsService _reviewsService;
    private readonly IUserContextService _userContext;

    public UpdateEpicReviewEndpoint(IEpicReviewsService reviewsService, IUserContextService userContext)
    {
        _reviewsService = reviewsService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/reviews/{reviewId}")]
    [Authorize]
    public static async Task<Results<Ok<UpdateEpicReviewResponse>, BadRequest<UpdateEpicReviewResponse>, UnauthorizedHttpResult>> HandlePut(
        [FromRoute] string locale,
        [FromRoute] Guid reviewId,
        [FromBody] UpdateEpicReviewRequest request,
        [FromServices] UpdateEpicReviewEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        // Ensure reviewId in route matches request
        var updateRequest = request with { ReviewId = reviewId };
        var response = await ep._reviewsService.UpdateReviewAsync(userId.Value, updateRequest);
        
        if (!response.Success)
            return TypedResults.BadRequest(response);

        return TypedResults.Ok(response);
    }
}

[Endpoint]
public class DeleteEpicReviewEndpoint
{
    private readonly IEpicReviewsService _reviewsService;
    private readonly IUserContextService _userContext;

    public DeleteEpicReviewEndpoint(IEpicReviewsService reviewsService, IUserContextService userContext)
    {
        _reviewsService = reviewsService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/reviews/{reviewId}")]
    [Authorize]
    public static async Task<Results<Ok<DeleteEpicReviewResponse>, BadRequest<DeleteEpicReviewResponse>, UnauthorizedHttpResult>> HandleDelete(
        [FromRoute] string locale,
        [FromRoute] Guid reviewId,
        [FromServices] DeleteEpicReviewEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var response = await ep._reviewsService.DeleteReviewAsync(userId.Value, reviewId);
        
        if (!response.Success)
            return TypedResults.BadRequest(response);

        return TypedResults.Ok(response);
    }
}

[Endpoint]
public class GetEpicReviewsEndpoint
{
    private readonly IEpicReviewsService _reviewsService;
    private readonly IUserContextService _userContext;

    public GetEpicReviewsEndpoint(IEpicReviewsService reviewsService, IUserContextService userContext)
    {
        _reviewsService = reviewsService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/reviews/{epicId}")]
    [Authorize]
    public static async Task<Results<Ok<GetEpicReviewsResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string epicId,
        [FromServices] GetEpicReviewsEndpoint ep,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc")
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var request = new GetEpicReviewsRequest
        {
            EpicId = epicId,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        var response = await ep._reviewsService.GetEpicReviewsAsync(epicId, userId, request);
        return TypedResults.Ok(response);
    }
}

[Endpoint]
public class GetEpicReviewStatisticsEndpoint
{
    private readonly IEpicReviewsRepository _reviewsRepository;
    private readonly IUserContextService _userContext;

    public GetEpicReviewStatisticsEndpoint(IEpicReviewsRepository reviewsRepository, IUserContextService userContext)
    {
        _reviewsRepository = reviewsRepository;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/reviews/{epicId}/statistics")]
    [Authorize]
    public static async Task<Results<Ok<GetEpicReviewStatisticsResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string epicId,
        [FromServices] GetEpicReviewStatisticsEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var stats = await ep._reviewsRepository.GetReviewStatisticsAsync(epicId);
        
        var response = new GetEpicReviewStatisticsResponse
        {
            Success = true,
            AverageRating = stats.AverageRating,
            TotalCount = stats.TotalCount,
            RatingDistribution = stats.RatingDistribution
        };

        return TypedResults.Ok(response);
    }
}

public record GetEpicReviewStatisticsResponse
{
    public bool Success { get; init; }
    public double AverageRating { get; init; }
    public int TotalCount { get; init; }
    public Dictionary<int, int> RatingDistribution { get; init; } = new();
}

