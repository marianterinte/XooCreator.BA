using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

    [Route("/api/{locale}/tree-of-light/complete-story")] // POST
    [Authorize]
    public static async Task<Results<Ok<CompleteStoryResponse>, BadRequest<CompleteStoryResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromQuery] string? configId,
        [FromServices] CompleteStoryEndpoint ep,
        [FromBody] CompleteStoryRequest request)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        if (string.IsNullOrEmpty(configId))
        {
            var configs = await ep._service.GetAllConfigurationsAsync();
            configId = configs.FirstOrDefault(c => c.IsDefault)?.Id ?? configs.First().Id;
        }
        var result = await ep._service.CompleteStoryAsync(userId.Value, request, configId);
        return result.Success ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}
