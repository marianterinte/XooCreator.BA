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
            ep.TranslateText(item.Name, item.BestiaryType, locale, translations, item.ArmsKey, item.BodyKey, item.HeadKey),
            ep.GetImageUrl(item.BestiaryType, item.ArmsKey, item.BodyKey, item.HeadKey),
            ep.TranslateText(item.Story, item.BestiaryType, locale, translations, item.ArmsKey, item.BodyKey, item.HeadKey),
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

    private string TranslateText(string text, string bestiaryType, string locale, Dictionary<string, string> translations, string? armsKey = null, string? bodyKey = null, string? headKey = null)
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
        else if (bestiaryType == "discovery")
        {
            return GetDiscoveryTranslation(text, locale, armsKey, bodyKey, headKey);
        }
        
        return text;
    }

    private string GetStoryHeroTranslation(string heroId, string field, string locale)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // First try the individual hero files in BookOfHeroes/i18n
            var heroFilePath = Path.Combine(baseDir, "Data", "SeedData", "BookOfHeroes", "i18n", locale, $"{heroId}.json");
            
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
            
            // If not found, try the consolidated story-heroes.json file
            var storyHeroesFilePath = Path.Combine(baseDir, "Data", "SeedData", "Translations", locale, "story-heroes.json");
            
            if (File.Exists(storyHeroesFilePath))
            {
                var json = File.ReadAllText(storyHeroesFilePath);
                var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                var key = $"story_hero_{heroId}_{field}";
                if (translations?.ContainsKey(key) == true)
                {
                    return translations[key];
                }
            }
            
            // Fallback to English if not found in requested locale
            if (locale != "en-us")
            {
                // Try English individual hero file
                var fallbackHeroFilePath = Path.Combine(baseDir, "Data", "SeedData", "BookOfHeroes", "i18n", "en-us", $"{heroId}.json");
                
                if (File.Exists(fallbackHeroFilePath))
                {
                    var json = File.ReadAllText(fallbackHeroFilePath);
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
                
                // Try English consolidated story-heroes.json file
                var fallbackStoryHeroesFilePath = Path.Combine(baseDir, "Data", "SeedData", "Translations", "en-us", "story-heroes.json");
                
                if (File.Exists(fallbackStoryHeroesFilePath))
                {
                    var json = File.ReadAllText(fallbackStoryHeroesFilePath);
                    var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                    var key = $"story_hero_{heroId}_{field}";
                    if (translations?.ContainsKey(key) == true)
                    {
                        return translations[key];
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

    private string GetDiscoveryTranslation(string text, string locale, string? armsKey, string? bodyKey, string? headKey)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // Build the combination from the keys
            var combination = BuildCombination(armsKey, bodyKey, headKey);
            if (string.IsNullOrEmpty(combination))
            {
                return text; // If we can't build the combination, return original text
            }
            
            // Try to get the translation for the requested locale
            var discoveryFilePath = Path.Combine(baseDir, "Data", "SeedData", "Discovery", "i18n", locale, "discover-bestiary.json");
            
            if (File.Exists(discoveryFilePath))
            {
                var json = File.ReadAllText(discoveryFilePath);
                var discoveryData = JsonSerializer.Deserialize<List<DiscoveryCreature>>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                // Find the creature by combination
                var creature = discoveryData?.FirstOrDefault(c => c.Combination == combination);
                if (creature != null)
                {
                    // Return name or story based on what we're translating
                    return text == creature.Name ? creature.Name : creature.Story;
                }
            }
            
            // If not found in requested locale, try English fallback
            if (locale != "en-us")
            {
                var fallbackFilePath = Path.Combine(baseDir, "Data", "SeedData", "Discovery", "i18n", "en-us", "discover-bestiary.json");
                
                if (File.Exists(fallbackFilePath))
                {
                    var json = File.ReadAllText(fallbackFilePath);
                    var discoveryData = JsonSerializer.Deserialize<List<DiscoveryCreature>>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                    // Find the creature by combination in English
                    var creature = discoveryData?.FirstOrDefault(c => c.Combination == combination);
                    if (creature != null)
                    {
                        // Return name or story based on what we're translating
                        return text == creature.Name ? creature.Name : creature.Story;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading discovery translation for {text}: {ex.Message}");
        }

        return text; // Fallback to original text
    }

    private string BuildCombination(string? armsKey, string? bodyKey, string? headKey)
    {
        // Convert "—" back to "None" to match the combination format
        var arms = armsKey == "—" ? "None" : armsKey ?? "None";
        var body = bodyKey == "—" ? "None" : bodyKey ?? "None";
        var head = headKey == "—" ? "None" : headKey ?? "None";
        
        return $"{arms}{body}{head}";
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

public class DiscoveryCreature
{
    public string Combination { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImagePrompt { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
    public string ImageFileName { get; set; } = string.Empty;
}


