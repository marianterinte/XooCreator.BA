using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly IStoryDraftAssetCleanupService _cleanupService;
    private readonly XooDbContext _db;
    private readonly ILogger<DeleteStoryEndpoint> _logger;

    public DeleteStoryEndpoint(
        IStoryCraftsRepository crafts,
        IStoryEditorService editorService,
        IAuth0UserService auth0,
        IStoryDraftAssetCleanupService cleanupService,
        XooDbContext db,
        ILogger<DeleteStoryEndpoint> logger)
    {
        _crafts = crafts;
        _editorService = editorService;
        _auth0 = auth0;
        _cleanupService = cleanupService;
        _db = db;
        _logger = logger;
    }

    public record DeleteResponse
    {
        public bool Ok { get; init; } = true;
        public string Message { get; init; } = "Story deleted successfully";
    }

    [Route("/api/stories/{storyId}")]
    [Authorize]
    public static async Task<Results<Ok<DeleteResponse>, NotFound, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandleDelete(
        [FromRoute] string storyId,
        [FromServices] DeleteStoryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isCreator = ep._auth0.HasRole(user, Data.Enums.UserRole.Creator);
        var isAdmin = ep._auth0.HasRole(user, Data.Enums.UserRole.Admin);
        if (!isCreator && !isAdmin)
        {
            return TypedResults.Forbid();
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        if (craft.OwnerUserId != user.Id && !isAdmin)
        {
            return TypedResults.Forbid();
        }

        var currentStatus = StoryStatusExtensions.FromDb(craft.Status);
        if (currentStatus != StoryStatus.Draft)
        {
            return TypedResults.Conflict($"Cannot delete story. Only stories in Draft status can be deleted. Current status: {currentStatus.GetDescription()}");
        }

        // For asset cleanup use owner email (when admin deleting another user's draft we need the draft owner's email)
        var ownerEmail = user.Email ?? "";
        if (isAdmin && craft.OwnerUserId != user.Id)
        {
            var owner = await ep._db.AlchimaliaUsers
                .AsNoTracking()
                .Where(u => u.Id == craft.OwnerUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(ct);
            ownerEmail = owner ?? ownerEmail;
        }

        // Delete draft assets from Azure Storage before deleting from database
        await ep._cleanupService.DeleteDraftAssetsAsync(ownerEmail, storyId, ct);
        
        // Delete draft from database
        await ep._editorService.DeleteDraftAsync(user.Id, storyId, allowAdminOverride: isAdmin, ct);
        
        ep._logger.LogInformation("Delete story: userId={UserId} storyId={StoryId} assetsDeleted=true", user.Id, storyId);
        
        return TypedResults.Ok(new DeleteResponse());
    }
}

