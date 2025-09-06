using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Services;

namespace XooCreator.BA.Features.System.Endpoints;

[Endpoint]
public class DbHealthEndpoint
{
    [Route("/api/db/health")] // GET
    public static async Task<IResult> HandleGet(
        [FromServices] IDbHealthService health,
        CancellationToken ct)
    {
        try
        {
            var result = await health.GetAsync(ct);
            return Results.Ok(new { canConnect = result.CanConnect, pendingMigrations = result.PendingMigrations });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}
