using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.Bestiary.Endpoints;

[Endpoint]
public sealed class GetUserBestiaryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IUserContextService _userContext;
    public GetUserBestiaryEndpoint(XooDbContext db, IUserContextService userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    [Route("/api/{locale}/bestiary")] // GET
    [Authorize]
    public static async Task<Results<Ok<BestiaryResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? bestiaryType,
        [FromServices] GetUserBestiaryEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var query = ep._db.UserBestiary
            .Where(ub => ub.UserId == userId.Value);

        if (!string.IsNullOrEmpty(bestiaryType))
        {
            query = query.Where(ub => ub.BestiaryType == bestiaryType);
        }

        var rawItems = await query
            .Join(ep._db.BestiaryItems, ub => ub.BestiaryItemId, bi => bi.Id, (ub, bi) => new { ub, bi })
            .OrderByDescending(x => x.ub.DiscoveredAt)
            .Select(x => new { 
                x.bi.Id,
                x.bi.Name,
                x.bi.ArmsKey,
                x.bi.BodyKey,
                x.bi.HeadKey,
                x.bi.Story,
                x.ub.DiscoveredAt,
                x.ub.BestiaryType
            })
            .ToListAsync(ct);

        var heroIds = rawItems
            .Where(i => i.BestiaryType == "treeofheroes")
            .Select(i => i.ArmsKey)
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .Distinct()
            .ToList();

        var storyHeroIds = rawItems
            .Where(i => i.BestiaryType == "storyhero")
            .Select(i => i.ArmsKey)
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .Distinct()
            .ToList();

        // Primary source for story heroes: published epic hero definitions (Story Creator)
        var epicHeroDefinitions = await ep._db.EpicHeroDefinitions
            .Where(d => storyHeroIds.Contains(d.Id))
            .ToListAsync(ct);

        var epicHeroTranslations = await ep._db.EpicHeroDefinitionTranslations
            .Where(t => epicHeroDefinitions.Select(d => d.Id).Contains(t.EpicHeroDefinitionId))
            .ToListAsync(ct);

        // For legacy ArmsKeys that didn't match any EpicHeroDefinition by ID,
        // try to find them by matching EpicHeroDefinitionTranslation.Name (case-insensitive).
        // This bridges legacy heroIds (e.g. "linkaro") to EpicHeroDefinitions with different IDs.
        var legacyHeroIdMapping = new Dictionary<string, string>();
        var unmatchedStoryHeroIds = storyHeroIds
            .Where(id => !epicHeroDefinitions.Any(d => d.Id == id))
            .ToList();
        if (unmatchedStoryHeroIds.Count > 0)
        {
            var lowerUnmatched = unmatchedStoryHeroIds.Select(id => id.ToLowerInvariant()).ToList();
            var matchingTranslations = await ep._db.EpicHeroDefinitionTranslations
                .Where(t => lowerUnmatched.Contains(t.Name.ToLower()))
                .ToListAsync(ct);

            foreach (var mt in matchingTranslations)
            {
                var matchingArmsKey = unmatchedStoryHeroIds.FirstOrDefault(id =>
                    string.Equals(id, mt.Name, StringComparison.OrdinalIgnoreCase));
                if (matchingArmsKey != null && !legacyHeroIdMapping.ContainsKey(matchingArmsKey))
                {
                    legacyHeroIdMapping[matchingArmsKey] = mt.EpicHeroDefinitionId;
                }
            }

            var additionalDefIds = legacyHeroIdMapping.Values.Distinct().ToList();
            if (additionalDefIds.Count > 0)
            {
                var additionalDefs = await ep._db.EpicHeroDefinitions
                    .Where(d => additionalDefIds.Contains(d.Id))
                    .ToListAsync(ct);
                var additionalTrans = await ep._db.EpicHeroDefinitionTranslations
                    .Where(t => additionalDefIds.Contains(t.EpicHeroDefinitionId))
                    .ToListAsync(ct);

                epicHeroDefinitions.AddRange(additionalDefs);
                epicHeroTranslations.AddRange(additionalTrans);
            }
        }

        var heroTranslations = await ep._db.HeroDefinitionDefinitionTranslations
            .Where(t => heroIds.Contains(t.HeroDefinitionDefinitionId))
            .ToListAsync(ct);

        var heroDefinitions = await ep._db.HeroDefinitionDefinitions
            .Where(d => heroIds.Contains(d.Id))
            .ToListAsync(ct);

        var storyHeroes = await ep._db.StoryHeroes
            .Where(sh => storyHeroIds.Contains(sh.HeroId))
            .ToListAsync(ct);

        var storyHeroTranslations = await ep._db.StoryHeroTranslations
            .Where(t => storyHeroes.Select(sh => sh.Id).Contains(t.StoryHeroId))
            .ToListAsync(ct);

        var items = rawItems.Select(item =>
        {
            var (name, story) = ep.ResolveText(item, locale, heroTranslations, storyHeroes, storyHeroTranslations, epicHeroDefinitions, epicHeroTranslations, legacyHeroIdMapping);
            var imageUrl = ep.ResolveImageUrl(item, heroDefinitions, storyHeroes, epicHeroDefinitions, legacyHeroIdMapping);
            return new BestiaryItemDto(
                item.Id,
                name,
                imageUrl,
                story,
                item.DiscoveredAt,
                item.BestiaryType
            );
        }).ToList();

        var res = new BestiaryResponse(items);
        return TypedResults.Ok(res);
    }

    private string ResolveImageUrl(dynamic item, List<HeroDefinitionDefinition> heroDefinitions, List<StoryHero> storyHeroes, List<EpicHeroDefinition> epicHeroDefinitions, Dictionary<string, string> legacyHeroIdMapping)
    {
        var bestiaryType = item.BestiaryType as string;
        var armsKey = item.ArmsKey as string;
        var bodyKey = item.BodyKey as string;
        var headKey = item.HeadKey as string;

        if (bestiaryType == "storyhero")
        {
            var epicDefId = armsKey;
            if (epicHeroDefinitions.All(h => h.Id != epicDefId) && legacyHeroIdMapping.TryGetValue(armsKey!, out var mappedId))
                epicDefId = mappedId;

            return epicHeroDefinitions.FirstOrDefault(h => h.Id == epicDefId)?.ImageUrl
                   ?? storyHeroes.FirstOrDefault(h => h.HeroId == armsKey)?.ImageUrl
                   ?? $"images/tol/stories/seed@alchimalia.com/heroes/{armsKey}.png";
        }

        return bestiaryType switch
        {
            "treeofheroes" => heroDefinitions.FirstOrDefault(h => h.Id == armsKey)?.Image
                              ?? armsKey + ".jpg",
            _ => (armsKey == "—" ? "None" : armsKey) + (bodyKey == "—" ? "None" : bodyKey) + (headKey == "—" ? "None" : headKey) + ".jpg"
        };
    }

    private (string name, string story) ResolveText(
        dynamic item,
        string locale,
        List<HeroDefinitionDefinitionTranslation> heroTranslations,
        List<StoryHero> storyHeroes,
        List<StoryHeroTranslation> storyHeroTranslations,
        List<EpicHeroDefinition> epicHeroDefinitions,
        List<EpicHeroDefinitionTranslation> epicHeroTranslations,
        Dictionary<string, string> legacyHeroIdMapping)
    {
        var bestiaryType = item.BestiaryType as string;
        var armsKey = item.ArmsKey as string;

        if (bestiaryType == "treeofheroes")
        {
            var normalizedLang = locale.ToLowerInvariant();
            var translation = heroTranslations.FirstOrDefault(t =>
                    t.HeroDefinitionDefinitionId == armsKey && t.LanguageCode.ToLowerInvariant() == normalizedLang)
                ?? heroTranslations.FirstOrDefault(t => t.HeroDefinitionDefinitionId == armsKey);

            return (translation?.Name ?? armsKey ?? string.Empty, translation?.Story ?? string.Empty);
        }

        if (bestiaryType == "storyhero")
        {
            var normalizedLang = locale.ToLowerInvariant();

            // Resolve the EpicHeroDefinition ID: direct match first, then legacy mapping
            var epicDefId = armsKey;
            var epicHero = epicHeroDefinitions.FirstOrDefault(h => h.Id == epicDefId);
            if (epicHero == null && armsKey != null && legacyHeroIdMapping.TryGetValue(armsKey, out var mappedId))
            {
                epicDefId = mappedId;
                epicHero = epicHeroDefinitions.FirstOrDefault(h => h.Id == epicDefId);
            }

            // Primary: EpicHeroDefinition + EpicHeroDefinitionTranslation (published from Story Creator)
            if (epicHero != null)
            {
                var epicTranslation = epicHeroTranslations.FirstOrDefault(t =>
                        t.EpicHeroDefinitionId == epicDefId && t.LanguageCode.ToLowerInvariant() == normalizedLang)
                    ?? epicHeroTranslations.FirstOrDefault(t => t.EpicHeroDefinitionId == epicDefId);

                if (epicTranslation != null)
                {
                    return (epicTranslation.Name ?? epicHero.Name, epicTranslation.Description ?? epicTranslation.GreetingText ?? string.Empty);
                }

                // EpicHeroDefinition found but no translations yet — use its display Name
                return (epicHero.Name, string.Empty);
            }

            // Fallback: StoryHeroes + StoryHeroTranslations (legacy)
            var storyHero = storyHeroes.FirstOrDefault(sh => sh.HeroId == armsKey);
            var storyTranslation = storyHero != null
                ? storyHeroTranslations.FirstOrDefault(t =>
                    t.StoryHeroId == storyHero.Id && t.LanguageCode.ToLowerInvariant() == normalizedLang)
                    ?? storyHeroTranslations.FirstOrDefault(t => t.StoryHeroId == storyHero.Id)
                : null;

            if (storyTranslation != null)
            {
                return (storyTranslation.Name ?? armsKey ?? string.Empty, storyTranslation.Description ?? storyTranslation.GreetingText ?? string.Empty);
            }

            return (armsKey ?? string.Empty, string.Empty);
        }

        // discovery: use stored text (no JSON)
        return (item.Name as string ?? string.Empty, item.Story as string ?? string.Empty);
    }
}


