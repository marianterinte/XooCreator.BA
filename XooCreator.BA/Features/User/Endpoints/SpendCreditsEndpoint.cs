using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

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

    [Route("/api/user/spend-credits")] // POST (legacy / generative)
    public static async Task<Results<Ok<SpendCreditsResponse>, UnauthorizedHttpResult, BadRequest<SpendCreditsResponse>>> HandlePost(
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

    [Route("/api/user/spend-discovery-credits")] // POST
    public static async Task<Results<Ok<SpendCreditsResponse>, UnauthorizedHttpResult, BadRequest<SpendCreditsResponse>>> HandlePost(
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
