using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
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
    private readonly ILogger<GetMarketplaceStoriesEndpoint>? _logger;
    private readonly TelemetryClient? _telemetryClient;

    public GetMarketplaceStoriesEndpoint(
        IStoriesMarketplaceService marketplaceService, 
        IUserContextService userContext,
        ILogger<GetMarketplaceStoriesEndpoint>? logger = null,
        TelemetryClient? telemetryClient = null)
    {
        _marketplaceService = marketplaceService;
        _userContext = userContext;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market")]
    [AllowAnonymous]
    public static async Task<Ok<GetMarketplaceStoriesResponse>> HandleGetMarketplace(
        [FromRoute] string locale,
        [FromServices] GetMarketplaceStoriesEndpoint ep,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string[]? regions = null,
        [FromQuery] string[]? ageRatings = null,
        [FromQuery] string[]? ageGroupIds = null,
        [FromQuery] string[]? characters = null,
        [FromQuery] string[]? categories = null,
        [FromQuery] string[]? difficulties = null,
        [FromQuery] string[]? topics = null,
        [FromQuery] bool? isEvaluative = null,
        [FromQuery] string completionStatus = "all",
        [FromQuery] string sortBy = "sortOrder",
        [FromQuery] string sortOrder = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] int? skip = null,
        [FromQuery] string searchType = "title")
    {
        var endpointStopwatch = Stopwatch.StartNew();
        
        try
        {
            var userId = await ep._userContext.GetUserIdAsync();

            ep._logger?.LogInformation("Marketplace Request: searchType={SearchType}, searchTerm={SearchTerm}", searchType, searchTerm);

            var request = new SearchStoriesRequest
            {
                SearchTerm = searchTerm,
                SearchType = searchType,
                Regions = regions?.ToList() ?? new List<string>(),
                AgeRatings = ageRatings?.ToList() ?? new List<string>(),
                AgeGroupIds = ageGroupIds?.ToList() ?? new List<string>(),
                Characters = characters?.ToList() ?? new List<string>(),
                Categories = categories?.ToList() ?? new List<string>(),
                Difficulties = difficulties?.ToList() ?? new List<string>(),
                Topics = topics?.ToList() ?? new List<string>(),
                IsEvaluative = isEvaluative,
                CompletionStatus = completionStatus,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize,
                Skip = skip
            };

            // For anonymous users, pass an empty userId to get a generic marketplace view
            var effectiveUserId = userId ?? Guid.Empty;

            var result = await ep._marketplaceService.GetMarketplaceStoriesAsync(effectiveUserId, locale, request);
            
            return TypedResults.Ok(result);
        }
        finally
        {
            endpointStopwatch.Stop();
            var endpointDuration = endpointStopwatch.ElapsedMilliseconds;
            
            // Log endpoint duration
            ep._logger?.LogInformation(
                "GetMarketplaceStoriesEndpoint completed | Duration={Duration}ms | Locale={Locale} | Page={Page} | PageSize={PageSize}",
                endpointDuration, locale, page, pageSize);
            
            // Track in Application Insights
            if (ep._telemetryClient != null)
            {
                ep._telemetryClient.TrackMetric("GetMarketplaceStoriesEndpoint_Duration", endpointDuration, new Dictionary<string, string>
                {
                    ["Endpoint"] = "GetMarketplaceStories",
                    ["Locale"] = locale,
                    ["Page"] = page.ToString(),
                    ["PageSize"] = pageSize.ToString(),
                    ["SortBy"] = sortBy,
                    ["SortOrder"] = sortOrder
                });
            }
        }
    }
}


