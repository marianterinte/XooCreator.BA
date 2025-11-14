using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public partial class PublishStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<PublishStoryEndpoint> _logger;
    private readonly XooCreator.BA.Features.StoryEditor.Services.IStoryPublishingService _publisher;
    private readonly XooCreator.BA.Features.StoryEditor.Services.IStoryPublishAssetService _assetService;

    public PublishStoryEndpoint(
        IStoryCraftsRepository crafts, 
        IUserContextService userContext, 
        IAuth0UserService auth0, 
        ILogger<PublishStoryEndpoint> logger, 
        XooCreator.BA.Features.StoryEditor.Services.IStoryPublishingService publisher,
        XooCreator.BA.Features.StoryEditor.Services.IStoryPublishAssetService assetService)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _logger = logger;
        _publisher = publisher;
        _assetService = assetService;
    }

    [Route("/api/{locale}/stories/{storyId}/publish")]
    [Authorize]
    public static async Task<Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] PublishStoryEndpoint ep,
        CancellationToken ct)
    {
        // Authorization check
        var authResult = await ep.ValidateAuthorizationAsync(ct);
        if (authResult.Result != null) return authResult.Result;
        var user = authResult.User!;

        var langTag = ep._userContext.GetRequestLocaleOrDefault("ro-ro");

        // Load craft
        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        // Validate permissions and status
        var permissionResult = ep.ValidatePublishPermissions(user, craft);
        if (permissionResult != null) return permissionResult;

        // Collect assets for all available languages
        // Images are common (extract once), Audio/Video are language-specific (extract per language)
        var allAssets = ep._assetService.CollectAllAssets(craft);
        var copyResult = await ep._assetService.CopyAssetsToPublishedAsync(allAssets, user.Email, storyId, ct);
        if (copyResult.HasError) return copyResult.ErrorResult;

        // Finalize publishing
        await ep.FinalizePublishingAsync(craft, user.Email, langTag, allAssets.Count, storyId, ct);

        return TypedResults.Ok(new PublishResponse());
    }

    private async Task<(AlchimaliaUser? User, Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>? Result)> ValidateAuthorizationAsync(CancellationToken ct)
    {
        var user = await _auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return (null, TypedResults.Unauthorized());
        }

        if (!_auth0.HasRole(user, Data.Enums.UserRole.Creator))
        {
            return (null, TypedResults.Forbid());
        }

        return (user, null);
    }

    private Results<Ok<PublishResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>? ValidatePublishPermissions(AlchimaliaUser user, StoryCraft craft)
    {
        var isAdmin = _auth0.HasRole(user, Data.Enums.UserRole.Admin);

        if (!isAdmin && craft.OwnerUserId != user.Id)
        {
            return TypedResults.BadRequest("Only the owner can publish this story.");
        }

        var current = StoryStatusExtensions.FromDb(craft.Status);
        if (!isAdmin && current != StoryStatus.Approved)
        {
            _logger.LogWarning("Publish invalid state: storyId={StoryId} state={State}", craft.StoryId, current);
            return TypedResults.Conflict("Invalid state transition. Expected Approved.");
        }

        return null;
    }

    private async Task FinalizePublishingAsync(
        StoryCraft craft,
        string userEmail,
        string langTag,
        int assetsCount,
        string storyId,
        CancellationToken ct)
    {
        var newVersion = await _publisher.UpsertFromCraftAsync(craft, userEmail, langTag, ct);
        craft.Status = StoryStatus.Published.ToDb();
        craft.BaseVersion = newVersion;
        await _crafts.SaveAsync(craft, ct);
        _logger.LogInformation("Published story: storyId={StoryId} assets={Count}", storyId, assetsCount);
    }
}

