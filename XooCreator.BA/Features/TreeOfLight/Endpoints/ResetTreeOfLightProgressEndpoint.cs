using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfLight.DTOs;
using XooCreator.BA.Features.TreeOfLight.Services;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class ResetTreeOfLightProgressEndpoint
{
    private readonly ITreeOfLightService _service;
    private readonly IUserContextService _userContext;
    public ResetTreeOfLightProgressEndpoint(ITreeOfLightService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-light/reset-progress")] // POST
    [Authorize]
    public static async Task<Results<Ok<ResetProgressResponse>, BadRequest<ResetProgressResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] ResetTreeOfLightProgressEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var result = await ep._service.ResetUserProgressAsync(userId.Value);
        return result.Success ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}
