using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Features.Stories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class GetMarketplaceStoriesEndpoint
{
    private readonly IStoriesMarketplaceService _marketplaceService;
    private readonly IUserContextService _userContext;

    public GetMarketplaceStoriesEndpoint(IStoriesMarketplaceService marketplaceService, IUserContextService userContext)
    {
        _marketplaceService = marketplaceService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market")]
    [Authorize]
    public static async Task<Ok<GetMarketplaceStoriesResponse>> HandleGetMarketplace(
        [FromRoute] string locale,
        [FromServices] GetMarketplaceStoriesEndpoint ep,
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
        [FromQuery] int pageSize = 5)
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
}


