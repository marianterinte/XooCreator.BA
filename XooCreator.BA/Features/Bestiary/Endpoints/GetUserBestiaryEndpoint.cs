using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfLight;
using System.Text.Json;
using XooCreator.BA.Features.TreeOfLight.Services;

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
            var (name, story) = ep.ResolveText(item, locale, heroTranslations, storyHeroes, storyHeroTranslations);
            var imageUrl = ep.ResolveImageUrl(item, heroDefinitions, storyHeroes);
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

    private string ResolveImageUrl(dynamic item, List<HeroDefinitionDefinition> heroDefinitions, List<StoryHero> storyHeroes)
    {
        var bestiaryType = item.BestiaryType as string;
        var armsKey = item.ArmsKey as string;
        var bodyKey = item.BodyKey as string;
        var headKey = item.HeadKey as string;

        return bestiaryType switch
        {
            "treeofheroes" => heroDefinitions.FirstOrDefault(h => h.Id == armsKey)?.Image
                              ?? armsKey + ".jpg",
            "storyhero" => storyHeroes.FirstOrDefault(h => h.HeroId == armsKey)?.ImageUrl
                           ?? $"images/tol/stories/seed@alchimalia.com/heroes/{armsKey}.png",
            _ => (armsKey == "—" ? "None" : armsKey) + (bodyKey == "—" ? "None" : bodyKey) + (headKey == "—" ? "None" : headKey) + ".jpg"
        };
    }

    private (string name, string story) ResolveText(
        dynamic item,
        string locale,
        List<HeroDefinitionDefinitionTranslation> heroTranslations,
        List<StoryHero> storyHeroes,
        List<StoryHeroTranslation> storyHeroTranslations)
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
            var storyHero = storyHeroes.FirstOrDefault(sh => sh.HeroId == armsKey);
            var normalizedLang = locale.ToLowerInvariant();
            var translation = storyHeroTranslations.FirstOrDefault(t =>
                    t.StoryHeroId == storyHero?.Id && t.LanguageCode.ToLowerInvariant() == normalizedLang)
                ?? storyHeroTranslations.FirstOrDefault(t => t.StoryHeroId == storyHero?.Id);

            var name = translation?.Name ?? armsKey ?? string.Empty;
            var story = translation?.Description ?? translation?.GreetingText ?? string.Empty;
            return (name, story);
        }

        // discovery: use stored text (no JSON)
        return (item.Name as string ?? string.Empty, item.Story as string ?? string.Empty);
    }
}


