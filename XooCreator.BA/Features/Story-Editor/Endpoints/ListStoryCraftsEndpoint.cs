using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly XooDbContext _db;

    public ListStoryCraftsEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0, XooDbContext db)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
        _db = db;
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
        var wantAssigned = string.Equals(scope, "assigned", StringComparison.OrdinalIgnoreCase);
        var wantClaimable = string.Equals(scope, "claimable", StringComparison.OrdinalIgnoreCase);

        var list = (isReviewerOrAdmin && (wantAll || wantAssigned || wantClaimable))
            ? await ep._crafts.ListAllAsync(ct)
            : await ep._crafts.ListByOwnerAsync(user.Id, ct);

        if (isReviewerOrAdmin && wantAssigned)
        {
            list = list.Where(c => c.AssignedReviewerUserId == user.Id).ToList();
        }
        else if (isReviewerOrAdmin && wantClaimable)
        {
            list = list.Where(c => c.AssignedReviewerUserId == null && StoryStatusExtensions.FromDb(c.Status) == StoryStatus.SentForApproval).ToList();
        }

        // Enrich OwnerEmail by querying Users table
        var uniqueOwnerIds = list.Select(c => c.OwnerUserId).Distinct().ToList();
        var ownerEmailMap = await ep._db.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => uniqueOwnerIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? "", ct);

        var items = new List<StoryCraftListItemDto>(list.Count);
        var requestedLocale = locale.ToLowerInvariant();
        foreach (var c in list)
        {
            // Try to get translation for requested locale, fallback to first available
            var translation = c.Translations
                .FirstOrDefault(t => t.LanguageCode == requestedLocale && !string.IsNullOrWhiteSpace(t.Title))
                ?? c.Translations.FirstOrDefault(t => t.LanguageCode == requestedLocale)
                ?? c.Translations.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Title))
                ?? c.Translations.FirstOrDefault();
            string title = string.IsNullOrWhiteSpace(translation?.Title) ? c.StoryId : translation!.Title!;
            string? cover = c.CoverImageUrl;
            
            // Get available languages
            var availableLangs = c.Translations.Select(t => t.LanguageCode).ToList();
            var primaryLang = availableLangs.FirstOrDefault() ?? "ro-ro";

            var status = StoryStatusExtensions.FromDb(c.Status);
            var ownerEmail = ownerEmailMap.TryGetValue(c.OwnerUserId, out var email) ? email : "";
            
            items.Add(new StoryCraftListItemDto
            {
                StoryId = c.StoryId,
                Lang = primaryLang,
                Title = title,
                CoverImageUrl = cover,
                Status = MapStatusForFrontend(status),
                UpdatedAt = c.UpdatedAt,
                OwnerEmail = ownerEmail,
                IsOwnedByCurrentUser = c.OwnerUserId == user.Id,
                AssignedReviewerUserId = c.AssignedReviewerUserId,
                IsAssignedToCurrentUser = c.AssignedReviewerUserId == user.Id,
                AvailableLanguages = availableLangs,
                AudioLanguages = c.AudioLanguages ?? new List<string>()
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


