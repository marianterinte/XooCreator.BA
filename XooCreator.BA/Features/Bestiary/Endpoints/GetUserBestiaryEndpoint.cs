using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfLight;
using System.Text.Json;

namespace XooCreator.BA.Features.Bestiary.Endpoints;

[Endpoint]
public sealed class GetUserBestiaryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IUserContextService _userContext;
    private readonly ITreeOfLightTranslationService _translationService;

    public GetUserBestiaryEndpoint(XooDbContext db, IUserContextService userContext, ITreeOfLightTranslationService translationService)
    {
        _db = db;
        _userContext = userContext;
        _translationService = translationService;
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

        var translations = await ep._translationService.GetTranslationsAsync(locale);

        var items = rawItems.Select(item => new BestiaryItemDto(
            item.Id,
            ep.TranslateText(item.Name, item.BestiaryType, locale, translations),
            ep.GetImageUrl(item.BestiaryType, item.ArmsKey, item.BodyKey, item.HeadKey),
            ep.TranslateText(item.Story, item.BestiaryType, locale, translations),
            item.DiscoveredAt,
            item.BestiaryType
        )).ToList();

        var res = new BestiaryResponse(items);
        return TypedResults.Ok(res);
    }

    private string GetImageUrl(string bestiaryType, string armsKey, string bodyKey, string headKey)
    {
        return bestiaryType switch
        {
            "treeofheroes" => armsKey + ".jpg",  // For Tree of Heroes, use ArmsKey (which contains HeroId) + .jpg
            "storyhero" => GetStoryHeroImagePath(armsKey),  // For Story Heroes, use specific path based on hero
            _ => (armsKey == "—" ? "None" : armsKey) + (bodyKey == "—" ? "None" : bodyKey) + (headKey == "—" ? "None" : headKey) + ".jpg"  // For Discovery creatures
        };
    }

    private string GetStoryHeroImagePath(string heroId)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var storyHeroesPath = Path.Combine(baseDir, "Data", "SeedData", "SharedConfigs", "story-heroes.json");
            
            if (File.Exists(storyHeroesPath))
            {
                var json = File.ReadAllText(storyHeroesPath);
                var storyHeroesData = JsonSerializer.Deserialize<StoryHeroesConfig>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                var hero = storyHeroesData?.StoryHeroes?.FirstOrDefault(h => h.HeroId == heroId);
                if (hero != null)
                {
                    return hero.ImageUrl;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading story hero image path for {heroId}: {ex.Message}");
        }

        return $"images/tol/stories/heroes/{heroId}.png"; // Fallback
    }

    private string TranslateText(string text, string bestiaryType, string locale, Dictionary<string, string> translations)
    {
        if (bestiaryType == "storyhero" && text.StartsWith("story_hero_"))
        {
            var keyParts = text.Split('_');
            
            if (keyParts.Length >= 4)
            {
                var heroId = keyParts[2]; // "puf-puf"
                var field = keyParts[3]; // "name" or "story"
                
                return GetStoryHeroTranslation(heroId, field, locale);
            }
        }
        
        return text;
    }

    private string GetStoryHeroTranslation(string heroId, string field, string locale)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            var heroFilePath = Path.Combine(baseDir, "Resources", "BookOfHeroes", locale, $"{heroId}.json");
            
            if (File.Exists(heroFilePath))
            {
                var json = File.ReadAllText(heroFilePath);
                var heroData = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (heroData?.ContainsKey(field) == true)
                {
                    return heroData[field];
                }
            }
            
            if (locale != "en-us")
            {
                var fallbackFilePath = Path.Combine(baseDir, "Resources", "BookOfHeroes", "en-us", $"{heroId}.json");
                
                if (File.Exists(fallbackFilePath))
                {
                    var json = File.ReadAllText(fallbackFilePath);
                    var heroData = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                    if (heroData?.ContainsKey(field) == true)
                    {
                        return heroData[field];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading story hero translation for {heroId}.{field}: {ex.Message}");
        }

        return $"story_hero_{heroId}_{field}"; // Fallback to key
    }
}

// Helper classes for JSON deserialization
public class StoryHeroesConfig
{
    public List<StoryHeroConfig>? StoryHeroes { get; set; }
}

public class StoryHeroConfig
{
    public string Id { get; set; } = string.Empty;
    public string HeroId { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public object? UnlockConditions { get; set; }
}


