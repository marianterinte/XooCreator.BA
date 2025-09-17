using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Services;

namespace XooCreator.BA.Features.CreatureBuilder.Endpoints;

[Endpoint]
public class GetUserAwareCreatureBuilderDataEndpoint
{
    private readonly ICreatureBuilderService _service;
    private readonly IUserContextService _userContext;

    public GetUserAwareCreatureBuilderDataEndpoint(ICreatureBuilderService service, IUserContextService userContext)
    {
        _service = service;
        _userContext = userContext;
    }

    [Route("/api/creature-builder/user-data")] // GET /api/creature-builder/user-data
    public static async Task<Results<Ok<UserAwareCreatureBuilderDataDto>, UnauthorizedHttpResult>> HandleGet(
        [FromServices] GetUserAwareCreatureBuilderDataEndpoint ep, 
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) 
            return TypedResults.Unauthorized();

        var data = await ep._service.GetUserAwareDataAsync(userId.Value, ct);
        return TypedResults.Ok(data);
    }

    [Route("/api/creature-builder/discover")] // POST
    public static async Task<Results<Ok<object>, UnauthorizedHttpResult, BadRequest<object>>> HandleDiscover(
        [FromServices] GetUserAwareCreatureBuilderDataEndpoint ep,
        [FromBody] object request,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        // Placeholder: return 200 with success = true until service implemented
        var response = new { success = true };
        return TypedResults.Ok(response);
    }
}
