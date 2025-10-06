
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.User.Endpoints;

[Endpoint]
public class CompleteLabIntroEndpoint
{
    [Route("/api/{locale}/user/complete-lab-intro")] // POST
    public static async Task<IResult> HandlePost(
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

        if (!user.HasVisitedImaginationLaboratory)
        {
            user.HasVisitedImaginationLaboratory = true;
            await dbContext.SaveChangesAsync(ct);
        }

        return Results.Ok();
    }
}
