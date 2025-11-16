using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class AddFavoriteEndpoint
{
    private readonly IFavoritesService _favoritesService;
    private readonly IUserContextService _userContext;

    public AddFavoriteEndpoint(IFavoritesService favoritesService, IUserContextService userContext)
    {
        _favoritesService = favoritesService;
        _userContext = userContext;
    }

    public record AddFavoriteRequest(string StoryId);
    public record AddFavoriteResponse(bool Success, string? ErrorMessage);

    [Route("/api/{locale}/tales-of-alchimalia/market/favorites")]
    [Authorize]
    public static async Task<Results<Ok<AddFavoriteResponse>, BadRequest<AddFavoriteResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromBody] AddFavoriteRequest request,
        [FromServices] AddFavoriteEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(request.StoryId))
            return TypedResults.BadRequest(new AddFavoriteResponse(false, "StoryId is required"));

        var success = await ep._favoritesService.AddFavoriteAsync(userId.Value, request.StoryId);
        
        if (!success)
            return TypedResults.BadRequest(new AddFavoriteResponse(false, "Failed to add favorite"));

        return TypedResults.Ok(new AddFavoriteResponse(true, null));
    }
}

[Endpoint]
public class RemoveFavoriteEndpoint
{
    private readonly IFavoritesService _favoritesService;
    private readonly IUserContextService _userContext;

    public RemoveFavoriteEndpoint(IFavoritesService favoritesService, IUserContextService userContext)
    {
        _favoritesService = favoritesService;
        _userContext = userContext;
    }

    public record RemoveFavoriteResponse(bool Success, string? ErrorMessage);

    [Route("/api/{locale}/tales-of-alchimalia/market/favorites/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<RemoveFavoriteResponse>, BadRequest<RemoveFavoriteResponse>, UnauthorizedHttpResult>> HandleDelete(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] RemoveFavoriteEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(storyId))
            return TypedResults.BadRequest(new RemoveFavoriteResponse(false, "StoryId is required"));

        var success = await ep._favoritesService.RemoveFavoriteAsync(userId.Value, storyId);
        
        if (!success)
            return TypedResults.BadRequest(new RemoveFavoriteResponse(false, "Failed to remove favorite"));

        return TypedResults.Ok(new RemoveFavoriteResponse(true, null));
    }
}

[Endpoint]
public class CheckFavoriteEndpoint
{
    private readonly IFavoritesService _favoritesService;
    private readonly IUserContextService _userContext;

    public CheckFavoriteEndpoint(IFavoritesService favoritesService, IUserContextService userContext)
    {
        _favoritesService = favoritesService;
        _userContext = userContext;
    }

    public record CheckFavoriteResponse(bool IsFavorite);

    [Route("/api/{locale}/tales-of-alchimalia/market/favorites/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<CheckFavoriteResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] CheckFavoriteEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var isFavorite = await ep._favoritesService.IsFavoriteAsync(userId.Value, storyId);
        return TypedResults.Ok(new CheckFavoriteResponse(isFavorite));
    }
}

[Endpoint]
public class GetFavoritesEndpoint
{
    private readonly IFavoritesService _favoritesService;
    private readonly IUserContextService _userContext;

    public GetFavoritesEndpoint(IFavoritesService favoritesService, IUserContextService userContext)
    {
        _favoritesService = favoritesService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tales-of-alchimalia/market/favorites")]
    [Authorize]
    public static async Task<Results<Ok<GetMarketplaceStoriesResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetFavoritesEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var response = await ep._favoritesService.GetFavoriteStoriesAsync(userId.Value, locale);
        return TypedResults.Ok(response);
    }
}

