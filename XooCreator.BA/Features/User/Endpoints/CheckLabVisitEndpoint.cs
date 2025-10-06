
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.User.Endpoints;

// Define a DTO for the response
public class FirstVisitDto
{
    public bool IsFirstVisit { get; set; }
}

[Endpoint]
public class CheckLabVisitEndpoint
{
    [Route("/api/{locale}/user/check-lab-visit")] // GET
    [Authorize]
    public static async Task<IResult> HandleGet(
        [FromRoute] string locale,
        [FromServices] IUserContextService userContext,
        [FromServices] XooDbContext dbContext,
        CancellationToken ct)
    {
        var userId = await userContext.GetUserIdAsync();
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        var user = await dbContext.AlchimaliaUsers.FindAsync(userId.Value);
        if (user == null)
        {
            return Results.NotFound("User not found.");
        }

        return Results.Ok(new FirstVisitDto { IsFirstVisit = !user.HasVisitedImaginationLaboratory });
    }
}
