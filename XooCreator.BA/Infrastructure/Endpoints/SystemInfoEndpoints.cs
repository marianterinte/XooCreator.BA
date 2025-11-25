using Microsoft.AspNetCore.Mvc;

namespace XooCreator.BA.Infrastructure.Endpoints;

[Endpoint]
public class SystemInfoEndpoints
{
    [Route("/api/system/version")]
    [HttpGet]
    public static IResult HandleGet([FromServices] IConfiguration configuration)
    {
        var version = configuration["Application:Version"] ?? "unknown";
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        return TypedResults.Ok(new
        {
            version,
            environment,
            timestamp = DateTimeOffset.UtcNow
        });
    }
}

