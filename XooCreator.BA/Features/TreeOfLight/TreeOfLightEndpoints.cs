using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight;

public static class TreeOfLightEndpoints
{
    public static IEndpointRouteBuilder MapTreeOfLightEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tree-of-light").WithTags("TreeOfLight");

        group.MapGet("/progress", async ([FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var progress = await service.GetTreeProgressAsync(userId.Value);
            return Results.Ok(progress);
        })
        .WithName("GetTreeProgress")
        .Produces<List<TreeProgressDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/stories", async ([FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            // Deprecated endpoint kept for backward compatibility
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var stories = await service.GetStoryProgressAsync(userId.Value);
            return Results.Ok(stories);
        })
        .WithName("GetStoryProgressLegacy")
        .Produces<List<StoryProgressDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/user-progress", async ([FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var stories = await service.GetStoryProgressAsync(userId.Value);
            return Results.Ok(stories);
        })
        .WithName("GetUserStoryProgress")
        .Produces<List<StoryProgressDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/tokens", async ([FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var tokens = await service.GetUserTokensAsync(userId.Value);
            return Results.Ok(tokens);
        })
        .WithName("GetUserTokens")
        .Produces<UserTokensDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/heroes", async ([FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var heroes = await service.GetHeroProgressAsync(userId.Value);
            return Results.Ok(heroes);
        })
        .WithName("GetHeroProgress")
        .Produces<List<HeroDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/hero-tree", async ([FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var heroTree = await service.GetHeroTreeProgressAsync(userId.Value);
            return Results.Ok(heroTree);
        })
        .WithName("GetHeroTreeProgress")
        .Produces<List<HeroTreeNodeDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/complete-story", async ([FromBody] CompleteStoryRequest request, [FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var result = await service.CompleteStoryAsync(userId.Value, request);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("CompleteStory")
        .Produces<CompleteStoryResponse>(StatusCodes.Status200OK)
        .Produces<CompleteStoryResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/unlock-hero-tree-node", async ([FromBody] UnlockHeroTreeNodeRequest request, [FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var result = await service.UnlockHeroTreeNodeAsync(userId.Value, request);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("UnlockHeroTreeNode")
        .Produces<UnlockHeroTreeNodeResponse>(StatusCodes.Status200OK)
        .Produces<UnlockHeroTreeNodeResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/transform-hero", async ([FromBody] TransformToHeroRequest request, [FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var result = await service.TransformToHeroAsync(userId.Value, request);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("TransformToHero")
        .Produces<TransformToHeroResponse>(StatusCodes.Status200OK)
        .Produces<TransformToHeroResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/reset-progress", async ([FromServices] ITreeOfLightService service, [FromServices] IUserContextService userContext) =>
        {
            var userId = await userContext.GetUserIdAsync();
            if (userId == null) return Results.Unauthorized();
            var result = await service.ResetUserProgressAsync(userId.Value);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("ResetTreeOfLightProgress")
        .Produces<ResetProgressResponse>(StatusCodes.Status200OK)
        .Produces<ResetProgressResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
