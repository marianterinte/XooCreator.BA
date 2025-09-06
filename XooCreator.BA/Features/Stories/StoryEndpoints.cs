using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.Stories;

public static class StoryEndpoints
{
    public static IEndpointRouteBuilder MapStoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stories").WithTags("Stories");

        group.MapGet("/", async ([FromServices] IStoriesService service) =>
        {
            var result = await service.GetAllStoriesAsync();
            return Results.Ok(result);
        })
        .WithName("GetStories")
        .Produces<GetStoriesResponse>(StatusCodes.Status200OK);

        group.MapGet("/{storyId}", async (
            string storyId,
            [FromServices] IStoriesService service,
            [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();

            var result = await service.GetStoryByIdAsync(userId.Value, storyId);
            if (result.Story == null) return Results.NotFound();
            return Results.Ok(result);
        })
        .WithName("GetStoryById")
        .Produces<GetStoryByIdResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/mark-tile-read", async (
            [FromBody] MarkTileAsReadRequest request,
            [FromServices] IStoriesService service,
            [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();

            var result = await service.MarkTileAsReadAsync(userId.Value, request);
            if (!result.Success) return Results.BadRequest(result);
            return Results.Ok(result);
        })
        .WithName("MarkTileAsRead")
        .Produces<MarkTileAsReadResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
