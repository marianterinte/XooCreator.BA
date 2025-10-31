using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfLight.DTOs;
using XooCreator.BA.Features.TreeOfLight.Services;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class GetUserStoryProgressEndpoint
{
    private readonly ITreeOfLightService _service;
    private readonly IUserContextService _userContext;
    public GetUserStoryProgressEndpoint(ITreeOfLightService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/{locale}/tree-of-light/user-progress")] // GET
    [Authorize]
    public static async Task<Results<Ok<List<StoryProgressDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? configId,
        [FromServices] GetUserStoryProgressEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();
        if (string.IsNullOrEmpty(configId))
        {
            var configs = await ep._service.GetAllConfigurationsAsync();
            configId = configs.FirstOrDefault(c => c.IsDefault)?.Id ?? configs.First().Id;
        }
        var stories = await ep._service.GetStoryProgressAsync(userId.Value, configId);
        return TypedResults.Ok(stories);
    }
}
