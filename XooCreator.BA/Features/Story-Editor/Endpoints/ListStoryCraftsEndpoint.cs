using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.DTOs;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ListStoryCraftsEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;

    public ListStoryCraftsEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/story-editor/crafts")]
    [Authorize]
    public static async Task<Results<Ok<ListStoryCraftsResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? scope,
        [FromServices] ListStoryCraftsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isReviewerOrAdmin = ep._auth0.HasRole(user, Data.Enums.UserRole.Reviewer) || ep._auth0.HasRole(user, Data.Enums.UserRole.Admin);
        var wantAll = string.Equals(scope, "all", StringComparison.OrdinalIgnoreCase);

        var list = (isReviewerOrAdmin && wantAll)
            ? await ep._crafts.ListAllAsync(ct)
            : await ep._crafts.ListByOwnerAsync(user.Id, ct);

        var items = new List<StoryCraftListItemDto>(list.Count);
        foreach (var c in list)
        {
            // Extract title/cover from JSON
            string title = c.StoryId;
            string? cover = null;
            try
            {
                using var doc = JsonDocument.Parse(c.Json ?? "{}");
                if (doc.RootElement.TryGetProperty("title", out var tEl) && tEl.ValueKind == JsonValueKind.String)
                    title = tEl.GetString() ?? title;
                if (doc.RootElement.TryGetProperty("coverImageUrl", out var cEl) && cEl.ValueKind == JsonValueKind.String)
                    cover = cEl.GetString();
            }
            catch { /* ignore malformed draft json */ }

            var status = StoryStatusExtensions.FromDb(c.Status);
            items.Add(new StoryCraftListItemDto
            {
                StoryId = c.StoryId,
                Lang = c.Lang.ToTag(),
                Title = title,
                CoverImageUrl = cover,
                Status = MapStatusForFrontend(status),
                UpdatedAt = c.UpdatedAt,
                OwnerEmail = "", // could be enriched later by joining users table
                IsOwnedByCurrentUser = c.OwnerUserId == user.Id
            });
        }

        return TypedResults.Ok(new ListStoryCraftsResponse
        {
            Stories = items,
            TotalCount = items.Count
        });
    }

    private static string MapStatusForFrontend(StoryStatus status)
        => StoriesService.MapStatusForFrontendForExternal(status);
}


