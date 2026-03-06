using Microsoft.AspNetCore.Http;

namespace XooCreator.BA.Features.System.Services;

/// <summary>
/// Standard 503 response when Story Creator is in maintenance and job submission is disabled.
/// </summary>
public static class StoryCreatorMaintenanceResult
{
    public const string Code = "StoryCreatorMaintenance";
    public const string Message = "Story Creator is currently in maintenance. Job submission is disabled.";

    public static IResult Unavailable()
    {
        return Results.Json(new { code = Code, message = Message }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
}
