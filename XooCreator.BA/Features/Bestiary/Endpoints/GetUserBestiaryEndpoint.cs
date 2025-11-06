using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.SeedData.DTOs;
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
            ep.TranslateText(item.Name, item.BestiaryType, locale, translations, item.ArmsKey, item.BodyKey, item.HeadKey, "name"),
            ep.GetImageUrl(item.BestiaryType, item.ArmsKey, item.BodyKey, item.HeadKey),
            ep.TranslateText(item.Story, item.BestiaryType, locale, translations, item.ArmsKey, item.BodyKey, item.HeadKey, "story"),
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
            // Try to load from individual JSON file (new structure)
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var heroFilePath = Path.Combine(baseDir, "Data", "SeedData", "StoryHeroes", $"{heroId}.json");
            
            if (File.Exists(heroFilePath))
            {
                var json = File.ReadAllText(heroFilePath);
                var heroData = JsonSerializer.Deserialize<StoryHeroSeedData>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (!string.IsNullOrEmpty(heroData?.ImageUrl))
                {
                    return heroData.ImageUrl;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading story hero image path for {heroId}: {ex.Message}");
        }

        return $"images/tol/stories/heroes/{heroId}.png"; // Fallback
    }

    private string TranslateText(string text, string bestiaryType, string locale, Dictionary<string, string> translations, string? armsKey = null, string? bodyKey = null, string? headKey = null, string? fieldHint = null)
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
        else if (bestiaryType == "treeofheroes")
        {
            return GetTreeOfHeroesTranslation(text, locale, armsKey, fieldHint);
        }
        
        return text;
    }

    private string GetStoryHeroTranslation(string heroId, string field, string locale)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // Try the individual hero files in StoryHeroes/i18n (new location)
            var heroFilePath = Path.Combine(baseDir, "Data", "SeedData", "StoryHeroes", "i18n", locale, $"{heroId}.json");
            
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
            
            // Fallback to English if not found in requested locale
            if (locale != "en-us")
            {
                // Try English individual hero file
                var fallbackHeroFilePath = Path.Combine(baseDir, "Data", "SeedData", "StoryHeroes", "i18n", "en-us", $"{heroId}.json");
                
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
                    // Return the appropriate field from the translation file
                    // We need to determine if we're translating name or story
                    // by checking if the original text matches the name pattern
                    return GetTranslatedField(text, creature);
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
                        // Return the appropriate field from the translation file
                        return GetTranslatedField(text, creature);
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

    private string GetTreeOfHeroesTranslation(string text, string locale, string? armsKey, string? fieldHint = null)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            string translationKey;
            string heroId;
            string field;
            
            // If text is already a complete translation key (e.g., "hero_tree_brave_puppy_name"), use it directly
            if (text.StartsWith("hero_tree_") && (text.EndsWith("_name") || text.EndsWith("_story") || text.EndsWith("_description")))
            {
                translationKey = text;
            }
            else
            {
                // Text is just the heroId (e.g., "hero_tree_guardian" or just "guardian"), need to construct full key
                // First, try to get heroId from text or armsKey
                if (text.StartsWith("hero_tree_"))
                {
                    // Extract heroId from text (remove "hero_tree_" prefix)
                    heroId = text.Substring("hero_tree_".Length);
                }
                else if (!string.IsNullOrEmpty(armsKey))
                {
                    // Use armsKey as heroId (armsKey contains the hero ID for tree of heroes)
                    heroId = armsKey;
                }
                else
                {
                    // Fallback: use text as heroId
                    heroId = text;
                }
                
                if (string.IsNullOrEmpty(heroId))
                {
                    return text; // If we can't get the hero ID, return original text
                }
                
                // Use fieldHint if provided (from TranslateText call), otherwise determine from text
                field = fieldHint ?? GetTreeOfHeroesField(text);
                translationKey = $"hero_tree_{heroId}_{field}";
            }
            
            // Try to get the translation for the requested locale
            // Note: hero-tree.json is now in StoryHeroes/i18n (moved from BookOfHeroes/i18n)
            var heroTreeFilePath = Path.Combine(baseDir, "Data", "SeedData", "StoryHeroes", "i18n", locale, "hero-tree.json");
            
            if (File.Exists(heroTreeFilePath))
            {
                var json = File.ReadAllText(heroTreeFilePath);
                var heroTreeData = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
                
                if (heroTreeData?.ContainsKey(translationKey) == true)
                {
                    return heroTreeData[translationKey];
                }
            }
            
            // If not found in requested locale, try English fallback
            if (locale != "en-us")
            {
                var fallbackFilePath = Path.Combine(baseDir, "Data", "SeedData", "StoryHeroes", "i18n", "en-us", "hero-tree.json");
                
                if (File.Exists(fallbackFilePath))
                {
                    var json = File.ReadAllText(fallbackFilePath);
                    var heroTreeData = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (heroTreeData?.ContainsKey(translationKey) == true)
                    {
                        return heroTreeData[translationKey];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading tree of heroes translation for {text}: {ex.Message}");
        }

        return text; // Fallback to original text
    }

    private string GetTreeOfHeroesField(string text)
    {
        // If text is a translation key (e.g., "hero_tree_brave_puppy_name"), extract the field
        if (text.StartsWith("hero_tree_") && text.Contains("_"))
        {
            var parts = text.Split('_');
            if (parts.Length >= 4)
            {
                // Format: hero_tree_{heroId}_{field}
                var field = parts[parts.Length - 1]; // Last part is the field (name, story, description)
                return field;
            }
        }
        
        // Fallback: Simple heuristic: if the text is relatively short (likely a name), return "name"
        // Otherwise, return "story"
        if (text.Length < 50) // Names are typically shorter
        {
            return "name";
        }
        else
        {
            return "story";
        }
    }

    private string BuildCombination(string? armsKey, string? bodyKey, string? headKey)
    {
        // Convert "—" back to "None" to match the combination format
        var arms = armsKey == "—" ? "None" : armsKey ?? "None";
        var body = bodyKey == "—" ? "None" : bodyKey ?? "None";
        var head = headKey == "—" ? "None" : headKey ?? "None";
        
        return $"{arms}{body}{head}";
    }

    private string GetTranslatedField(string originalText, DiscoveryCreature creature)
    {
        // We need to determine if we're translating name or story
        // The original text comes from the database and could be in any language
        // We need to check which field it matches better
        
        // If the original text is shorter and matches the name pattern, it's likely a name
        // If it's longer and contains more descriptive text, it's likely a story
        
        // Simple heuristic: if the text is relatively short (likely a name), return the name
        // Otherwise, return the story
        if (originalText.Length < 50) // Names are typically shorter
        {
            return creature.Name;
        }
        else
        {
            return creature.Story;
        }
    }
}


public class DiscoveryCreature
{
    public string Combination { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImagePrompt { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
    public string ImageFileName { get; set; } = string.Empty;
}


