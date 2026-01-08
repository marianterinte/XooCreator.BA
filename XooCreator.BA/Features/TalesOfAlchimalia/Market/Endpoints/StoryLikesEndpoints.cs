using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Endpoints;

[Endpoint]
public class ToggleStoryLikeEndpoint
{
    private readonly IStoryLikesService _likesService;
    private readonly IUserContextService _userContext;

    public ToggleStoryLikeEndpoint(IStoryLikesService likesService, IUserContextService userContext)
    {
        _likesService = likesService;
        _userContext = userContext;
    }

    public record ToggleStoryLikeResponse(bool IsLiked, int LikesCount, bool Success, string? ErrorMessage);

    [Route("/api/{locale}/tales-of-alchimalia/market/stories/{storyId}/like/toggle")]
    [Authorize]
    public static async Task<Results<Ok<ToggleStoryLikeResponse>, BadRequest<ToggleStoryLikeResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ToggleStoryLikeEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(storyId))
            return TypedResults.BadRequest(new ToggleStoryLikeResponse(false, 0, false, "StoryId is required"));

        var response = await ep._likesService.ToggleLikeAsync(userId.Value, storyId);
        
        return TypedResults.Ok(new ToggleStoryLikeResponse(
            response.IsLiked,
            response.LikesCount,
            response.Success,
            response.ErrorMessage));
    }
}

[Endpoint]
public class GetStoryLikeStatusEndpoint
{
    private readonly IStoryLikesService _likesService;
    private readonly IUserContextService _userContext;

    public GetStoryLikeStatusEndpoint(IStoryLikesService likesService, IUserContextService userContext)
    {
        _likesService = likesService;
        _userContext = userContext;
    }

    public record GetStoryLikeStatusResponse(bool IsLiked, int LikesCount);

    [Route("/api/{locale}/tales-of-alchimalia/market/stories/{storyId}/like/status")]
    [Authorize]
    public static async Task<Results<Ok<GetStoryLikeStatusResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] GetStoryLikeStatusEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var response = await ep._likesService.GetLikeStatusAsync(userId.Value, storyId);
        return TypedResults.Ok(new GetStoryLikeStatusResponse(response.IsLiked, response.LikesCount));
    }
}

