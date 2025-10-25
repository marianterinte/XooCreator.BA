using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.Stories.Endpoints;

[Endpoint]
public class StoriesMarketplaceEndpoint
{
    private readonly IStoriesMarketplaceService _marketplaceService;
    private readonly IUserContextService _userContext;

    public StoriesMarketplaceEndpoint(IStoriesMarketplaceService marketplaceService, IUserContextService userContext)
    {
        _marketplaceService = marketplaceService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/stories/marketplace")]
    [Authorize]
    public static async Task<Ok<GetMarketplaceStoriesResponse>> HandleGetMarketplace(
        [FromRoute] string locale,
        [FromServices] StoriesMarketplaceEndpoint ep,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string[]? regions = null,
        [FromQuery] string[]? ageRatings = null,
        [FromQuery] string[]? characters = null,
        [FromQuery] string[]? categories = null,
        [FromQuery] string[]? difficulties = null,
        [FromQuery] string completionStatus = "all",
        [FromQuery] string sortBy = "sortOrder",
        [FromQuery] string sortOrder = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) throw new UnauthorizedAccessException("User not found");

        var request = new SearchStoriesRequest
        {
            SearchTerm = searchTerm,
            Regions = regions?.ToList() ?? new List<string>(),
            AgeRatings = ageRatings?.ToList() ?? new List<string>(),
            Characters = characters?.ToList() ?? new List<string>(),
            Categories = categories?.ToList() ?? new List<string>(),
            Difficulties = difficulties?.ToList() ?? new List<string>(),
            CompletionStatus = completionStatus,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Page = page,
            PageSize = pageSize
        };

        var result = await ep._marketplaceService.GetMarketplaceStoriesAsync(userId.Value, locale, request);
        return TypedResults.Ok(result);
    }

    [Route("/api/{locale}/stories/marketplace/purchase")]
    [Authorize]
    public static async Task<Results<Ok<PurchaseStoryResponse>, BadRequest<PurchaseStoryResponse>, UnauthorizedHttpResult>> HandlePurchaseStory(
        [FromRoute] string locale,
        [FromBody] PurchaseStoryRequest request,
        [FromServices] StoriesMarketplaceEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var result = await ep._marketplaceService.PurchaseStoryAsync(userId.Value, request);
        
        if (result.Success)
        {
            return TypedResults.Ok(result);
        }
        else
        {
            return TypedResults.BadRequest(result);
        }
    }

    [Route("/api/{locale}/stories/marketplace/purchased")]
    [Authorize]
    public static async Task<Ok<GetUserPurchasedStoriesResponse>> HandleGetPurchasedStories(
        [FromRoute] string locale,
        [FromServices] StoriesMarketplaceEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) throw new UnauthorizedAccessException("User not found");

        var result = await ep._marketplaceService.GetUserPurchasedStoriesAsync(userId.Value, locale);
        return TypedResults.Ok(result);
    }

    [Route("/api/{locale}/stories/marketplace/initialize")]
    [Authorize]
    public static async Task<Results<Ok, UnauthorizedHttpResult>> HandleInitializeMarketplace(
        [FromRoute] string locale,
        [FromServices] StoriesMarketplaceEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        await ep._marketplaceService.InitializeMarketplaceAsync();
        return TypedResults.Ok();
    }
}
