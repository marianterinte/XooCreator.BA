using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class DeleteStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryEditorService _editorService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<DeleteStoryEndpoint> _logger;

    public DeleteStoryEndpoint(
        IStoryCraftsRepository crafts,
        IStoryEditorService editorService,
        IAuth0UserService auth0,
        ILogger<DeleteStoryEndpoint> logger)
    {
        _crafts = crafts;
        _editorService = editorService;
        _auth0 = auth0;
        _logger = logger;
    }

    public record DeleteResponse
    {
        public bool Ok { get; init; } = true;
        public string Message { get; init; } = "Story deleted successfully";
    }

    [Route("/api/{locale}/stories/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<DeleteResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandleDelete(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] DeleteStoryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        if (craft.OwnerUserId != user.Id)
        {
            return TypedResults.Forbid();
        }

        var currentStatus = StoryStatusExtensions.FromDb(craft.Status);
        if (currentStatus != StoryStatus.Draft)
        {
            return TypedResults.Conflict($"Cannot delete story. Only stories in Draft status can be deleted. Current status: {currentStatus.GetDescription()}");
        }

        await ep._editorService.DeleteDraftAsync(user.Id, storyId, ct);
        ep._logger.LogInformation("Delete story: userId={UserId} storyId={StoryId}", user.Id, storyId);
        
        return TypedResults.Ok(new DeleteResponse());
    }
}

