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

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class GetMarketplaceEpicsEndpoint
{
    private readonly IEpicsMarketplaceService _epicsMarketplaceService;
    private readonly IUserContextService _userContext;
    private readonly ILogger<GetMarketplaceEpicsEndpoint>? _logger;
    private readonly TelemetryClient? _telemetryClient;

    public GetMarketplaceEpicsEndpoint(
        IEpicsMarketplaceService epicsMarketplaceService,
        IUserContextService userContext,
        ILogger<GetMarketplaceEpicsEndpoint>? logger = null,
        TelemetryClient? telemetryClient = null)
    {
        _epicsMarketplaceService = epicsMarketplaceService;
        _userContext = userContext;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/epics")]
    [Authorize]
    public static async Task<Ok<GetMarketplaceEpicsResponse>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetMarketplaceEpicsEndpoint ep,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string sortBy = "publishedAt",
        [FromQuery] string sortOrder = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var endpointStopwatch = Stopwatch.StartNew();
        
        try
        {
            var userId = await ep._userContext.GetUserIdAsync();
            if (userId == null) throw new UnauthorizedAccessException("User not found");

            var request = new SearchEpicsRequest
            {
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize
            };

            var result = await ep._epicsMarketplaceService.GetMarketplaceEpicsAsync(userId.Value, locale, request);
            
            return TypedResults.Ok(result);
        }
        finally
        {
            endpointStopwatch.Stop();
            var endpointDuration = endpointStopwatch.ElapsedMilliseconds;
            
            ep._logger?.LogInformation(
                "GetMarketplaceEpicsEndpoint completed | Duration={Duration}ms | Locale={Locale} | Page={Page} | PageSize={PageSize}",
                endpointDuration, locale, page, pageSize);
            
            if (ep._telemetryClient != null)
            {
                ep._telemetryClient.TrackMetric("GetMarketplaceEpicsEndpoint_Duration", endpointDuration, new Dictionary<string, string>
                {
                    ["Endpoint"] = "GetMarketplaceEpics",
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

