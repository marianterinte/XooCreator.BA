using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Endpoints;

[Endpoint]
public class ListAnimalDefinitionsEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ListAnimalDefinitionsEndpoint> _logger;

    public ListAnimalDefinitionsEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        ILogger<ListAnimalDefinitionsEndpoint> logger)
    {
        _db = db;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/alchimalia-universe/animal-definitions")]
    [Route("/api/{locale}/alchimalia-universe/animal-definitions")]
    [Authorize]
    public static async Task<Results<Ok<ListAnimalCraftsResponse>, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        string? locale,
        [FromQuery] Guid? regionId,
        [FromQuery] bool? isHybrid,
        [FromQuery] string? search,
        [FromQuery] string? language,
        [FromServices] ListAnimalDefinitionsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("ListAnimalDefinitions forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        var lang = (language ?? locale ?? "ro-ro").ToLowerInvariant();

        var query = ep._db.AnimalDefinitions
            .AsNoTracking()
            .Include(x => x.Translations)
            .Include(x => x.Region)
            .Where(x => x.Status == AlchimaliaUniverseStatus.Published.ToDb())
            .AsQueryable();

        if (regionId.HasValue)
        {
            query = query.Where(x => x.RegionId == regionId.Value);
        }

        if (isHybrid.HasValue)
        {
            query = query.Where(x => x.IsHybrid == isHybrid.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x =>
                x.Label.Contains(s) ||
                x.Translations.Any(t => t.Label.Contains(s)));
        }

        var definitions = await query
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

        var ownerIds = definitions
            .Where(d => d.PublishedByUserId.HasValue)
            .Select(d => d.PublishedByUserId!.Value)
            .Distinct()
            .ToList();

        var ownerEmailMap = await ep._db.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => ownerIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? string.Empty, ct);

        var items = definitions.Select(d =>
        {
            // pick translation by requested language, fallback first
            var t = d.Translations.FirstOrDefault(x => (x.LanguageCode ?? string.Empty).ToLowerInvariant() == lang)
                    ?? d.Translations.FirstOrDefault();

            var availableLanguages = d.Translations
                .Select(x => (x.LanguageCode ?? string.Empty).ToLowerInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var ownerEmail = d.PublishedByUserId.HasValue && ownerEmailMap.TryGetValue(d.PublishedByUserId.Value, out var email)
                ? email
                : string.Empty;

            var isOwnedByCurrentUser = d.PublishedByUserId.HasValue && d.PublishedByUserId.Value == user.Id;

            return new AnimalCraftListItemDto
            {
                Id = d.Id,
                PublishedDefinitionId = null,
                Label = t?.Label ?? d.Label,
                Src = d.Src,
                IsHybrid = d.IsHybrid,
                RegionId = d.RegionId,
                RegionName = d.Region?.Name,
                Status = d.Status,
                UpdatedAt = d.UpdatedAt,
                CreatedByUserId = d.PublishedByUserId,
                AvailableLanguages = availableLanguages,
                AssignedReviewerUserId = null,
                IsAssignedToCurrentUser = false,
                IsOwnedByCurrentUser = isOwnedByCurrentUser,
                OwnerEmail = ownerEmail
            };
        }).ToList();

        return TypedResults.Ok(new ListAnimalCraftsResponse
        {
            Animals = items,
            TotalCount = items.Count
        });
    }
}

