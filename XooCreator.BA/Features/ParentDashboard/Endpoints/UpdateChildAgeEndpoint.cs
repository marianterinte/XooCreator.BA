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
public class UpdateChildAgePreferencesEndpoint
{
    private readonly XooDbContext _context;
    private readonly IUserContextService _userContext;

    public UpdateChildAgePreferencesEndpoint(
        XooDbContext context,
        IUserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    [Route("/api/{locale}/parent-dashboard/child-age-preferences")]
    [Authorize]
    public static async Task<Results<Ok<UpdateChildAgePreferencesResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePut(
        [FromRoute] string locale,
        [FromBody] UpdateChildAgePreferencesRequest request,
        [FromServices] UpdateChildAgePreferencesEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var user = await ep._context.AlchimaliaUsers
            .FirstOrDefaultAsync(u => u.Id == userId.Value, ct);

        if (user == null)
            return TypedResults.Unauthorized();

        // Update selected age group IDs if provided
        if (request.SelectedAgeGroupIds != null)
        {
            user.SelectedAgeGroupIds = request.SelectedAgeGroupIds.Count > 0 
                ? request.SelectedAgeGroupIds 
                : null;
        }

        // Update auto-filter flag if provided
        if (request.AutoFilterStoriesByAge.HasValue)
        {
            user.AutoFilterStoriesByAge = request.AutoFilterStoriesByAge.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;

        try
        {
            await ep._context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Failed to update child age preferences: {ex.Message}");
        }

        return TypedResults.Ok(new UpdateChildAgePreferencesResponse
        {
            Success = true,
            SelectedAgeGroupIds = user.SelectedAgeGroupIds,
            AutoFilterStoriesByAge = user.AutoFilterStoriesByAge
        });
    }
}

