using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class AddEpicFavoriteEndpoint
{
    private readonly IEpicFavoritesService _favoritesService;
    private readonly IUserContextService _userContext;

    public AddEpicFavoriteEndpoint(IEpicFavoritesService favoritesService, IUserContextService userContext)
    {
        _favoritesService = favoritesService;
        _userContext = userContext;
    }

    public record AddEpicFavoriteRequest(string EpicId);
    public record AddEpicFavoriteResponse(bool Success, string? ErrorMessage);

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/favorites")]
    [Authorize]
    public static async Task<Results<Ok<AddEpicFavoriteResponse>, BadRequest<AddEpicFavoriteResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] AddEpicFavoriteRequest request,
        [FromServices] AddEpicFavoriteEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(request.EpicId))
            return TypedResults.BadRequest(new AddEpicFavoriteResponse(false, "EpicId is required"));

        var success = await ep._favoritesService.AddFavoriteAsync(userId.Value, request.EpicId);
        
        if (!success)
            return TypedResults.BadRequest(new AddEpicFavoriteResponse(false, "Failed to add favorite"));

        return TypedResults.Ok(new AddEpicFavoriteResponse(true, null));
    }
}

[Endpoint]
public class RemoveEpicFavoriteEndpoint
{
    private readonly IEpicFavoritesService _favoritesService;
    private readonly IUserContextService _userContext;

    public RemoveEpicFavoriteEndpoint(IEpicFavoritesService favoritesService, IUserContextService userContext)
    {
        _favoritesService = favoritesService;
        _userContext = userContext;
    }

    public record RemoveEpicFavoriteResponse(bool Success, string? ErrorMessage);

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/favorites/{epicId}")]
    [Authorize]
    public static async Task<Results<Ok<RemoveEpicFavoriteResponse>, BadRequest<RemoveEpicFavoriteResponse>, UnauthorizedHttpResult>> HandleDelete(
        [FromRoute] string locale,
        [FromRoute] string epicId,
        [FromServices] RemoveEpicFavoriteEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(epicId))
            return TypedResults.BadRequest(new RemoveEpicFavoriteResponse(false, "EpicId is required"));

        var success = await ep._favoritesService.RemoveFavoriteAsync(userId.Value, epicId);
        
        if (!success)
            return TypedResults.BadRequest(new RemoveEpicFavoriteResponse(false, "Failed to remove favorite"));

        return TypedResults.Ok(new RemoveEpicFavoriteResponse(true, null));
    }
}

[Endpoint]
public class CheckEpicFavoriteEndpoint
{
    private readonly IEpicFavoritesService _favoritesService;
    private readonly IUserContextService _userContext;

    public CheckEpicFavoriteEndpoint(IEpicFavoritesService favoritesService, IUserContextService userContext)
    {
        _favoritesService = favoritesService;
        _userContext = userContext;
    }

    public record CheckEpicFavoriteResponse(bool IsFavorite);

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/favorites/{epicId}")]
    [Authorize]
    public static async Task<Results<Ok<CheckEpicFavoriteResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string epicId,
        [FromServices] CheckEpicFavoriteEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var isFavorite = await ep._favoritesService.IsFavoriteAsync(userId.Value, epicId);
        return TypedResults.Ok(new CheckEpicFavoriteResponse(isFavorite));
    }
}

[Endpoint]
public class GetEpicFavoritesEndpoint
{
    private readonly IEpicFavoritesService _favoritesService;
    private readonly IUserContextService _userContext;

    public GetEpicFavoritesEndpoint(IEpicFavoritesService favoritesService, IUserContextService userContext)
    {
        _favoritesService = favoritesService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/favorites")]
    [Authorize]
    public static async Task<Results<Ok<GetMarketplaceEpicsResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetEpicFavoritesEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var response = await ep._favoritesService.GetFavoriteEpicsAsync(userId.Value, locale);
        return TypedResults.Ok(response);
    }
}

