using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.ParentDashboard.DTOs;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.ParentDashboard.Endpoints;

[Endpoint]
public class GetChildAgePreferencesEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContext;

    public GetChildAgePreferencesEndpoint(
        XooDbContext context,
        IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    [Route("/api/{locale}/parent-dashboard/child-age-preferences")]
    [Authorize]
    public static async Task<Results<Ok<GetChildAgePreferencesResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetChildAgePreferencesEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var user = await ep._context.AlchimaliaUsers
            .Where(u => u.Id == userId.Value)
            .Select(u => new { u.SelectedAgeGroupIds, u.AutoFilterStoriesByAge })
            .FirstOrDefaultAsync(ct);

        if (user == null)
            return TypedResults.Unauthorized();

        return TypedResults.Ok(new GetChildAgePreferencesResponse
        {
            SelectedAgeGroupIds = user.SelectedAgeGroupIds,
            AutoFilterStoriesByAge = user.AutoFilterStoriesByAge
        });
    }
}

