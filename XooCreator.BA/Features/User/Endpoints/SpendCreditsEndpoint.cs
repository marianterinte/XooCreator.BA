using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Features.User.DTOs;

namespace XooCreator.BA.Features.User.Endpoints;

[Endpoint]
public class SpendCreditsEndpoint
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUserContextService _userContext;

    public SpendCreditsEndpoint(IUserProfileService userProfileService, IUserContextService userContext)
    {
        _userProfileService = userProfileService;
        _userContext = userContext;
    }

    [Route("/api/{locale}/user/spend-credits")] // POST (legacy / generative)
    [Authorize]
    public static async Task<Results<Ok<SpendCreditsResponse>, UnauthorizedHttpResult, BadRequest<SpendCreditsResponse>>> HandlePost(
        [FromRoute] string locale,
        [FromServices] SpendCreditsEndpoint ep,
        [FromBody] SpendCreditsRequest request)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) 
            return TypedResults.Unauthorized();

        var result = await ep._userProfileService.SpendCreditsAsync(userId.Value, request);
        
        return result.Success 
            ? TypedResults.Ok(result) 
            : TypedResults.BadRequest(result);
    }

    [Route("/api/{locale}/user/spend-discovery-credits")] // POST
    [Authorize]
    public static async Task<Results<Ok<SpendCreditsResponse>, UnauthorizedHttpResult, BadRequest<SpendCreditsResponse>>> HandlePost(
        [FromRoute] string locale,
        [FromServices] SpendCreditsEndpoint ep,
        [FromBody] SpendCreditsRequest request,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) 
            return TypedResults.Unauthorized();

        var result = await ep._userProfileService.SpendDiscoveryCreditsAsync(userId.Value, request);
        return result.Success 
            ? TypedResults.Ok(result) 
            : TypedResults.BadRequest(result);
    }
}
