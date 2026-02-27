using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class ToggleEpicLikeEndpoint
{
    private readonly IEpicLikesService _likesService;
    private readonly IUserContextService _userContext;

    public ToggleEpicLikeEndpoint(IEpicLikesService likesService, IUserContextService userContext)
    {
        _likesService = likesService;
        _userContext = userContext;
    }

    public record ToggleEpicLikeResponse(bool IsLiked, int LikesCount, bool Success, string? ErrorMessage);

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/{epicId}/like/toggle")]
    [Authorize]
    public static async Task<Results<Ok<ToggleEpicLikeResponse>, BadRequest<ToggleEpicLikeResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string epicId,
        [FromServices] ToggleEpicLikeEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(epicId))
            return TypedResults.BadRequest(new ToggleEpicLikeResponse(false, 0, false, "EpicId is required"));

        var response = await ep._likesService.ToggleLikeAsync(userId.Value, epicId);

        return TypedResults.Ok(new ToggleEpicLikeResponse(
            response.IsLiked,
            response.LikesCount,
            response.Success,
            response.ErrorMessage));
    }
}

[Endpoint]
public class GetEpicLikeStatusEndpoint
{
    private readonly IEpicLikesService _likesService;
    private readonly IUserContextService _userContext;

    public GetEpicLikeStatusEndpoint(IEpicLikesService likesService, IUserContextService userContext)
    {
        _likesService = likesService;
        _userContext = userContext;
    }

    public record GetEpicLikeStatusResponse(bool IsLiked, int LikesCount);

    [Route("/api/{locale}/tales-of-alchimalia/market/epics/{epicId}/like/status")]
    [Authorize]
    public static async Task<Results<Ok<GetEpicLikeStatusResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string epicId,
        [FromServices] GetEpicLikeStatusEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var response = await ep._likesService.GetLikeStatusAsync(userId.Value, epicId);
        return TypedResults.Ok(new GetEpicLikeStatusResponse(response.IsLiked, response.LikesCount));
    }
}
