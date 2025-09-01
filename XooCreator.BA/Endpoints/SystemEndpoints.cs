using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Services;

namespace XooCreator.BA.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("System");

        // DB health
        group.MapGet("/db/health", async (IDbHealthService health, CancellationToken ct) =>
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
        })
        .WithName("DbHealth")
        .Produces<object>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // Root
        app.MapGet("/", () => "XooCreator.BA API is running! ?? Visit /swagger for API docs.");

        return app;
    }
}
