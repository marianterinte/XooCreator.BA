using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class UnpublishStoryEndpoint
{
    private static readonly TimeSpan MinPublishAge = TimeSpan.FromMinutes(2);

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IStoryPublishedAssetCleanupService _publishedCleanup;
    private readonly ILogger<UnpublishStoryEndpoint> _logger;

    public UnpublishStoryEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IStoryPublishedAssetCleanupService publishedCleanup,
        ILogger<UnpublishStoryEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _publishedCleanup = publishedCleanup;
        _logger = logger;
    }

    [Route("/api/stories/{storyId}/unpublish")]
    [Authorize]
    public static async Task<Results<Ok<UnpublishStoryResponse>, NotFound, BadRequest<string>, Conflict<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string storyId,
        [FromBody] UnpublishStoryRequest request,
        [FromServices] UnpublishStoryEndpoint ep,
        CancellationToken ct)
    {
        if (request == null)
        {
            return TypedResults.BadRequest("Request body is required.");
        }

        var trimmedStoryId = (storyId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmedStoryId))
        {
            return TypedResults.BadRequest("Invalid story id.");
        }

        if (!string.Equals(trimmedStoryId, request.ConfirmStoryId?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("Story id confirmation does not match.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return TypedResults.BadRequest("An unpublish reason is required.");
        }

        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var def = await ep._db.StoryDefinitions
            .FirstOrDefaultAsync(d => d.StoryId == trimmedStoryId, ct);

        if (def == null)
        {
            return TypedResults.NotFound();
        }

        if (!def.IsActive || def.Status != StoryStatus.Published)
        {
            return TypedResults.Conflict("Only published stories can be unpublished.");
        }

        var storyOwner = await ep._db.UserCreatedStories
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.StoryDefinitionId == def.Id, ct);

        var ownerUserId = storyOwner?.UserId ?? def.CreatedBy;
        var ownerEmail = storyOwner?.User?.Email;

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isAdmin)
        {
            if (ownerUserId.HasValue)
            {
                if (ownerUserId.Value != user.Id)
                {
                    return TypedResults.Forbid();
                }
            }
            else if (!string.Equals(ownerEmail, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                return TypedResults.Forbid();
            }
        }

        var lastPublishAudit = await ep._db.StoryPublicationAudits
            .Where(a => a.StoryDefinitionId == def.Id && a.Action == StoryPublicationAction.Publish)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (lastPublishAudit != null && DateTime.UtcNow - lastPublishAudit.CreatedAt < MinPublishAge)
        {
            return TypedResults.Conflict("Story was recently published. Please wait at least 2 minutes before unpublishing.");
        }

        if (string.IsNullOrWhiteSpace(ownerEmail))
        {
            ownerEmail = ep.TryExtractOwnerEmail(def) ?? user.Email;
        }

        using var tx = await ep._db.Database.BeginTransactionAsync(ct);

        def.IsActive = false;
        def.Status = StoryStatus.Archived;
        def.UpdatedAt = DateTime.UtcNow;
        def.UpdatedBy = user.Id;

        if (storyOwner != null)
        {
            storyOwner.IsPublished = false;
            storyOwner.PublishedAt = null;
        }

        ep._db.StoryPublicationAudits.Add(new StoryPublicationAudit
        {
            Id = Guid.NewGuid(),
            StoryDefinitionId = def.Id,
            StoryId = def.StoryId,
            PerformedByUserId = user.Id,
            PerformedByEmail = user.Email,
            Action = StoryPublicationAction.Unpublish,
            Notes = request.Reason.Trim(),
            CreatedAt = DateTime.UtcNow
        });

        await ep._db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        if (!string.IsNullOrWhiteSpace(ownerEmail))
        {
            await ep._publishedCleanup.DeletePublishedAssetsAsync(ownerEmail, def.StoryId, ct);
        }
        else
        {
            ep._logger.LogWarning("Could not determine owner email for published assets cleanup. storyId={StoryId}", def.StoryId);
        }

        return TypedResults.Ok(new UnpublishStoryResponse());
    }

    private string? TryExtractOwnerEmail(StoryDefinition def)
    {
        if (string.IsNullOrWhiteSpace(def.CoverImageUrl))
        {
            return null;
        }

        var parts = def.CoverImageUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var idx = Array.FindIndex(parts, p => string.Equals(p, "stories", StringComparison.OrdinalIgnoreCase));
        if (idx >= 0 && idx + 1 < parts.Length)
        {
            return parts[idx + 1];
        }

        return null;
    }
}

