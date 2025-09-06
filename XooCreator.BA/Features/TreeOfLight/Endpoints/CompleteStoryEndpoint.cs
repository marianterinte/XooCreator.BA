using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class CompleteStoryEndpoint
{
    private readonly ITreeOfLightService _service;
    private readonly IUserContextService _userContext;
    public CompleteStoryEndpoint(ITreeOfLightService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/tree-of-light/complete-story")] // POST
    public static async Task<Results<Ok<CompleteStoryResponse>, BadRequest<CompleteStoryResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] CompleteStoryEndpoint ep,
        [FromBody] CompleteStoryRequest request)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        var result = await ep._service.CompleteStoryAsync(userId.Value, request);
        return result.Success ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}
