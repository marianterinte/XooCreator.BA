using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class CreateStoryReviewEndpoint
{
    private readonly IStoryReviewsService _reviewsService;
    private readonly IUserContextService _userContext;

    public CreateStoryReviewEndpoint(IStoryReviewsService reviewsService, IUserContextService userContext)
    {
        _reviewsService = reviewsService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/reviews")]
    [Authorize]
    public static async Task<Results<Ok<CreateStoryReviewResponse>, BadRequest<CreateStoryReviewResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] CreateStoryReviewRequest request,
        [FromServices] CreateStoryReviewEndpoint ep)
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
public class UpdateStoryReviewEndpoint
{
    private readonly IStoryReviewsService _reviewsService;
    private readonly IUserContextService _userContext;

    public UpdateStoryReviewEndpoint(IStoryReviewsService reviewsService, IUserContextService userContext)
    {
        _reviewsService = reviewsService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/reviews/{reviewId}")]
    [Authorize]
    public static async Task<Results<Ok<UpdateStoryReviewResponse>, BadRequest<UpdateStoryReviewResponse>, UnauthorizedHttpResult>> HandlePut(
        [FromRoute] string locale,
        [FromRoute] Guid reviewId,
        [FromBody] UpdateStoryReviewRequest request,
        [FromServices] UpdateStoryReviewEndpoint ep)
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
public class DeleteStoryReviewEndpoint
{
    private readonly IStoryReviewsService _reviewsService;
    private readonly IUserContextService _userContext;

    public DeleteStoryReviewEndpoint(IStoryReviewsService reviewsService, IUserContextService userContext)
    {
        _reviewsService = reviewsService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/reviews/{reviewId}")]
    [Authorize]
    public static async Task<Results<Ok<DeleteStoryReviewResponse>, BadRequest<DeleteStoryReviewResponse>, UnauthorizedHttpResult>> HandleDelete(
        [FromRoute] string locale,
        [FromRoute] Guid reviewId,
        [FromServices] DeleteStoryReviewEndpoint ep)
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
public class GetStoryReviewsEndpoint
{
    private readonly IStoryReviewsService _reviewsService;
    private readonly IUserContextService _userContext;

    public GetStoryReviewsEndpoint(IStoryReviewsService reviewsService, IUserContextService userContext)
    {
        _reviewsService = reviewsService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/reviews/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<GetStoryReviewsResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] GetStoryReviewsEndpoint ep,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc")
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var request = new GetStoryReviewsRequest
        {
            StoryId = storyId,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        var response = await ep._reviewsService.GetStoryReviewsAsync(storyId, userId, request);
        return TypedResults.Ok(response);
    }
}

