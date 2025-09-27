using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.TreeOfLight.Endpoints;

[Endpoint]
public class GetTreeStateEndpoint
{
    [Route("/api/{locale}/tree-of-light/state")] // GET
    public static async Task<IResult> HandleGet(
        [FromServices] ITreeModelService treeModelService,
        [FromServices] IUserContextService userContext,
        CancellationToken ct)
    {
        try
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null)
            {
                return Results.Unauthorized();
            }
            
            var treeState = await treeModelService.GetTreeStateAsync(userId.Value);
            return Results.Ok(treeState);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            return Results.Problem($"Failed to get tree state: {ex.Message}");
        }
    }
}
