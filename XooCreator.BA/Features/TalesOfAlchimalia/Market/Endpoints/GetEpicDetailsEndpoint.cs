using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class GetEpicDetailsEndpoint
{
    private readonly IEpicsMarketplaceService _epicsMarketplaceService;
    private readonly IUserContextService _userContext;
    private readonly ILogger<GetEpicDetailsEndpoint>? _logger;

    public GetEpicDetailsEndpoint(
        IEpicsMarketplaceService epicsMarketplaceService,
        IUserContextService userContext,
        ILogger<GetEpicDetailsEndpoint>? logger = null)
    {
        _epicsMarketplaceService = epicsMarketplaceService;
        _userContext = userContext;
        _logger = logger;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/{epicId}")]
    [AllowAnonymous]
    public static async Task<Results<Ok<EpicDetailsDto>, NotFound, ProblemHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string epicId,
        [FromServices] GetEpicDetailsEndpoint ep,
        CancellationToken ct)
    {
        try
        {
            var userId = await ep._userContext.GetUserIdAsync();
            var effectiveUserId = userId ?? Guid.Empty;
            var epicDetails = await ep._epicsMarketplaceService.GetEpicDetailsAsync(epicId, effectiveUserId, locale);

            if (epicDetails == null)
                return TypedResults.NotFound();

            // Always return 200 with details; HasExclusiveAccess is set in service. Restriction enforced at play (state-with-progress).
            return TypedResults.Ok(epicDetails);
        }
        catch (Exception ex)
        {
            ep._logger?.LogError(ex, "Error getting epic details for epicId: {EpicId}", epicId);
            throw;
        }
    }
}

