using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfLight;
using System.Text.Json;

namespace XooCreator.BA.Features.CreatureBuilder.Endpoints;

[Endpoint]
public sealed class DiscoverEndpoint
{
    private readonly XooDbContext _db;
    private readonly IUserContextService _userContext;
    private readonly ITreeOfLightTranslationService _translationService;

    public DiscoverEndpoint(XooDbContext db, IUserContextService userContext, ITreeOfLightTranslationService translationService)
    {
        _db = db;
        _userContext = userContext;
        _translationService = translationService;
    }

    [Route("/api/{locale}/creature-builder/discover")] // POST
    [Authorize]
    public static async Task<Results<Ok<DiscoverResponseDto>, UnauthorizedHttpResult, BadRequest<DiscoverResponseDto>>> HandlePost(
        [FromRoute] string locale,
        [FromServices] DiscoverEndpoint ep,
        [FromBody] DiscoverRequestDto request,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        // Normalize input (trim, keep case as provided in seed)
        var arms = request.Combination.Arms?.Trim() ?? string.Empty;
        var body = request.Combination.Body?.Trim() ?? string.Empty;
        var head = request.Combination.Head?.Trim() ?? string.Empty;

        // 1) Find matching discovery item
        var item = await ep._db.BestiaryItems.FirstOrDefaultAsync(d =>
            d.ArmsKey == arms && d.BodyKey == body && d.HeadKey == head, ct);

        if (item == null)
        {
            return TypedResults.BadRequest(new DiscoverResponseDto(false, false, null, "Combination not found", null));
        }

        // 2) Check if already discovered
        var existing = await ep._db.UserBestiary
            .AnyAsync(ud => ud.UserId == userId.Value && ud.BestiaryItemId == item.Id, ct);
        bool alreadyDiscovered = existing;
        if (!alreadyDiscovered)
        {
            // Spend one discovery credit only when this combination is first discovered
            var wallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);
            if (wallet == null)
                return TypedResults.BadRequest(new DiscoverResponseDto(false, false, null, "Wallet not found", null));
            if (wallet.DiscoveryBalance <= 0)
                return TypedResults.BadRequest(new DiscoverResponseDto(false, false, null, "Insufficient discovery credits", wallet.DiscoveryBalance));

            wallet.DiscoveryBalance -= 1;
            wallet.UpdatedAt = DateTime.UtcNow;

            ep._db.UserBestiary.Add(new UserBestiary
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                BestiaryItemId = item.Id,
                BestiaryType = "discovery",
                DiscoveredAt = DateTime.UtcNow
            });
            await ep._db.SaveChangesAsync(ct);
        }

        // Build response item
        // Build image path from combination keys (Name or parts) with .jpg
        string normalize(string s) => s == "—" ? "None" : s;
        string imageUrl = $"{normalize(item.ArmsKey)}{normalize(item.BodyKey)}{normalize(item.HeadKey)}.jpg";

        // Translate the creature name and story based on locale
        var translatedName = ep.TranslateDiscoveryCreature(item.Name, locale, item.ArmsKey, item.BodyKey, item.HeadKey);
        var translatedStory = ep.TranslateDiscoveryCreature(item.Story, locale, item.ArmsKey, item.BodyKey, item.HeadKey);

        var responseItem = new DiscoverItemDto(
            item.Id,
            translatedName,
            imageUrl,
            translatedStory
        );

        // Get updated wallet balance
        var updatedWallet = await ep._db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);
        var updatedBalance = updatedWallet?.DiscoveryBalance ?? 0;

        return TypedResults.Ok(new DiscoverResponseDto(true, alreadyDiscovered, responseItem, null, updatedBalance));
    }

    private string TranslateDiscoveryCreature(string text, string locale, string? armsKey, string? bodyKey, string? headKey)
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

// Helper class for JSON deserialization
public class DiscoveryCreature
{
    public string Combination { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImagePrompt { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
    public string ImageFileName { get; set; } = string.Empty;
}
