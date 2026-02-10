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
    public static async Task<Results<Ok<EpicDetailsDto>, NotFound>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string epicId,
        [FromServices] GetEpicDetailsEndpoint ep)
    {
        try
        {
            var userId = await ep._userContext.GetUserIdAsync();

            // For anonymous users, pass an empty userId to get a generic epic view
            var effectiveUserId = userId ?? Guid.Empty;

            var epicDetails = await ep._epicsMarketplaceService.GetEpicDetailsAsync(epicId, effectiveUserId, locale);
            
            if (epicDetails == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(epicDetails);
        }
        catch (Exception ex)
        {
            ep._logger?.LogError(ex, "Error getting epic details for epicId: {EpicId}", epicId);
            throw;
        }
    }
}

