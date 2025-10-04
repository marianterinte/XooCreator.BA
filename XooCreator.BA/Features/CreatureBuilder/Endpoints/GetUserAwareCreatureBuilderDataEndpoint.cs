using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Services;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.CreatureBuilder.Endpoints;

[Endpoint]
public class GetUserAwareCreatureBuilderDataEndpoint
{
    private readonly ICreatureBuilderService _service;
    private readonly IUserContextService _userContext;
    private readonly XooDbContext _db;

    public GetUserAwareCreatureBuilderDataEndpoint(ICreatureBuilderService service, IUserContextService userContext, XooDbContext db)
    {
        _service = service;
        _userContext = userContext;
        _db = db;
    }

    [Route("/api/{locale}/creature-builder/user-data")] // GET /api/{locale}/creature-builder/user-data
    public static async Task<Results<Ok<UserAwareCreatureBuilderDataDto>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetUserAwareCreatureBuilderDataEndpoint ep, 
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) 
            return TypedResults.Unauthorized();

        var data = await ep._service.GetUserAwareDataAsync(userId.Value, ct);
        return TypedResults.Ok(data);
    }

}
