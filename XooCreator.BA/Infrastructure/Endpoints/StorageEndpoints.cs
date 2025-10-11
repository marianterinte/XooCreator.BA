using Microsoft.AspNetCore.Mvc;

namespace XooCreator.BA.Infrastructure.Endpoints;

[Endpoint]
public class HealthEndpoints
{
    [Route("/api/{locale?}/health")]
    [HttpGet]
    public static IResult HandleGet()
    {
        return TypedResults.Ok(new { status = "healthy", timestamp = DateTimeOffset.UtcNow });
    }
}


