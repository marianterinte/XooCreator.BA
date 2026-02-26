using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;
using System.Text.Json;

namespace XooCreator.BA.Features.TreeOfHeroes.Services;

public interface ITreeOfHeroesService
{
    Task<UserTokensDto> GetUserTokensAsync(Guid userId);
    Task<List<HeroDto>> GetHeroProgressAsync(Guid userId);
    Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId);
    Task<List<HeroDefinitionDto>> GetHeroDefinitionsAsync(string locale);
    Task<HeroDefinitionDto?> GetHeroDefinitionByIdAsync(string heroId, string locale);
    Task<TreeOfHeroesConfigDto> GetTreeOfHeroesConfigAsync();
    Task<TransformToHeroResponse> TransformToHeroAsync(Guid userId, TransformToHeroRequest request, string locale);
    Task<AlchimalianHeroProfileDto> GetAlchimalianHeroProfileAsync(Guid userId, string locale);
    Task<UpdateAlchimalianHeroProfileResponse> UpdateAlchimalianHeroProfileAsync(Guid userId, UpdateAlchimalianHeroProfileRequest request, string locale);
    Task<AlchimalianHeroAvailableResponse> GetAlchimalianHeroAvailableAsync(Guid userId, string locale);
    Task<DiscoverAlchimalianHeroResponse> DiscoverAlchimalianHeroAsync(Guid userId, DiscoverAlchimalianHeroRequest request, string locale);
}

public class TreeOfHeroesService : ITreeOfHeroesService
{
    private readonly ITreeOfHeroesRepository _repository;
    private readonly XooDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TreeOfHeroesService> _logger;

    public TreeOfHeroesService(ITreeOfHeroesRepository repository, XooDbContext db, IMemoryCache cache, ILogger<TreeOfHeroesService> logger)
    {
        _repository = repository;
        _db = db;
        _cache = cache;
        _logger = logger;
    }

    public Task<UserTokensDto> GetUserTokensAsync(Guid userId)
    {
        return _repository.GetUserTokensAsync(userId);
    }

    public Task<List<HeroDto>> GetHeroProgressAsync(Guid userId)
    {
        return _repository.GetHeroProgressAsync(userId);
    }

    public Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId)
    {
        return _repository.GetHeroTreeProgressAsync(userId);
    }

    public async Task<List<HeroDefinitionDto>> GetHeroDefinitionsAsync(string locale)
    {
        var key = $"tree_of_heroes:definitions:{locale.ToLower()}";
        return await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            return await _repository.GetHeroDefinitionsAsync(locale);
        }) ?? new List<HeroDefinitionDto>();
    }

    public async Task<HeroDefinitionDto?> GetHeroDefinitionByIdAsync(string heroId, string locale)
    {
        // Optimization: Try to find it in the full list cache first to avoid a separate DB call/cache entry if possible
        var allDefinitions = await GetHeroDefinitionsAsync(locale);
        var definition = allDefinitions.FirstOrDefault(d => d.Id == heroId);
        
        if (definition != null) return definition;

        // Fallback to repo if not found immediately (though it should be there if the list is complete)
        return await _repository.GetHeroDefinitionByIdAsync(heroId, locale);
    }

    public Task<TreeOfHeroesConfigDto> GetTreeOfHeroesConfigAsync()
    {
        return _repository.GetTreeOfHeroesConfigAsync();
    }


    public async Task<TransformToHeroResponse> TransformToHeroAsync(Guid userId, TransformToHeroRequest request, string locale)
    {
        try
        {
            // Get hero definition to check costs
            var heroDefinition = await _repository.GetHeroDefinitionByIdAsync(request.HeroId, locale);
            if (heroDefinition == null)
            {
                return new TransformToHeroResponse
                {
                    Success = false,
                    ErrorMessage = "Hero not found"
                };
            }

            // Check if user has enough tokens
            var tokens = await _repository.GetUserTokensAsync(userId);
            
            if (tokens.Courage < heroDefinition.CourageCost ||
                tokens.Curiosity < heroDefinition.CuriosityCost ||
                tokens.Thinking < heroDefinition.ThinkingCost ||
                tokens.Creativity < heroDefinition.CreativityCost ||
                tokens.Safety < heroDefinition.SafetyCost)
            {
                return new TransformToHeroResponse
                {
                    Success = false,
                    ErrorMessage = "Insufficient tokens for this transformation"
                };
            }

            // Spend tokens for transformation
            var tokensSpent = await _repository.SpendTokensAsync(userId, 
                heroDefinition.CourageCost,
                heroDefinition.CuriosityCost,
                heroDefinition.ThinkingCost,
                heroDefinition.CreativityCost,
                heroDefinition.SafetyCost);

            if (!tokensSpent)
            {
                return new TransformToHeroResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to spend tokens"
                };
            }

            // Save hero progress to database (transform the hero)
            var heroProgressSaved = await _repository.SaveHeroProgressAsync(userId, request.HeroId);
            if (!heroProgressSaved)
            {
                return new TransformToHeroResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to save hero progress"
                };
            }

            // Save hero to bestiary
            try
            {
                // Build translation keys from heroId (pattern: hero_tree_{heroId}_name/story)
                // This matches the pattern used in hero-tree.json and translation files
                var nameKey = $"hero_tree_{request.HeroId}_name";
                var storyKey = $"hero_tree_{request.HeroId}_story";
                
                // Check if hero already exists in bestiary (by ArmsKey which contains heroId)
                var existingBestiaryItem = await _db.BestiaryItems
                    .FirstOrDefaultAsync(bi => bi.ArmsKey == request.HeroId);
                
                BestiaryItem bestiaryItem;
                if (existingBestiaryItem == null)
                {
                    // Create new bestiary item for this hero
                    // Store translation keys instead of translated text (like storyhero does)
                    bestiaryItem = new BestiaryItem
                    {
                        Id = Guid.NewGuid(),
                        ArmsKey = request.HeroId, // Use HeroId as the identifier
                        BodyKey = "—", 
                        HeadKey = "—",
                        Name = nameKey,
                        Story = storyKey,
                        CreatedAt = DateTime.UtcNow
                    };
                    _db.BestiaryItems.Add(bestiaryItem);
                }
                else
                {
                    // Update existing item to use keys if it doesn't already
                    if (!existingBestiaryItem.Name.StartsWith("hero_tree_", StringComparison.OrdinalIgnoreCase))
                    {
                        existingBestiaryItem.Name = nameKey;
                    }
                    if (!existingBestiaryItem.Story.StartsWith("hero_tree_", StringComparison.OrdinalIgnoreCase))
                    {
                        existingBestiaryItem.Story = storyKey;
                    }
                    bestiaryItem = existingBestiaryItem;
                }

                // Check if user already has this hero in their bestiary
                var existingUserBestiary = await _db.UserBestiary
                    .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BestiaryItemId == bestiaryItem.Id && ub.BestiaryType == "treeofheroes");

                if (existingUserBestiary == null)
                {
                    // Add to user's bestiary
                    var userBestiary = new UserBestiary
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        BestiaryItemId = bestiaryItem.Id,
                        BestiaryType = "treeofheroes",
                        DiscoveredAt = DateTime.UtcNow
                    };
                    _db.UserBestiary.Add(userBestiary);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't fail the transformation
                // The hero transformation was successful, bestiary save is secondary
                _logger.LogError(ex, "Failed to save hero to bestiary");
            }

            // Create hero DTO for response
            var unlockedHero = new HeroDto
            {
                HeroId = request.HeroId,
                HeroType = "HERO_TREE_TRANSFORMATION",
                UnlockedAt = DateTime.UtcNow
            };

            // Auto-unlock new nodes based on prerequisites
            var newlyUnlockedNodes = await _repository.AutoUnlockNodesBasedOnPrerequisitesAsync(userId);

            return new TransformToHeroResponse
            {
                Success = true,
                UnlockedHero = unlockedHero,
                NewlyUnlockedNodes = newlyUnlockedNodes,
                UpdatedTokens = await _repository.GetUserTokensAsync(userId)
            };
        }
        catch (Exception ex)
        {
            return new TransformToHeroResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<AlchimalianHeroProfileDto> GetAlchimalianHeroProfileAsync(Guid userId, string locale)
    {
        var profile = await _repository.GetUserAlchimalianProfileAsync(userId);
        string? imageUrl = null;
        if (!string.IsNullOrEmpty(profile?.SelectedHeroId))
        {
            var def = await GetHeroDefinitionByIdAsync(profile.SelectedHeroId, locale);
            imageUrl = def?.Image;
            if (!string.IsNullOrEmpty(imageUrl) && !imageUrl.StartsWith("/"))
                imageUrl = "/" + imageUrl;
        }
        var discoveredIds = ParseDiscoveredHeroIds(profile?.DiscoveredHeroIdsJson);
        // Legacy: if profile has selected hero but no discovered list yet, treat selected as discovered
        if (discoveredIds.Count == 0 && !string.IsNullOrEmpty(profile?.SelectedHeroId))
            discoveredIds = new List<string> { profile.SelectedHeroId };
        return new AlchimalianHeroProfileDto
        {
            SelectedHeroId = profile?.SelectedHeroId,
            SelectedHeroImageUrl = imageUrl,
            DiscoveredHeroIds = discoveredIds
        };
    }

    public async Task<UpdateAlchimalianHeroProfileResponse> UpdateAlchimalianHeroProfileAsync(Guid userId, UpdateAlchimalianHeroProfileRequest request, string locale)
    {
        var selectedHeroId = string.IsNullOrWhiteSpace(request.SelectedHeroId) ? null : request.SelectedHeroId!.Trim();
        if (selectedHeroId != null)
        {
            var def = await _repository.GetHeroDefinitionByIdAsync(selectedHeroId, locale);
            if (def == null)
                return new UpdateAlchimalianHeroProfileResponse { Success = false, ErrorMessage = "Hero not found" };
            var available = await GetAlchimalianHeroAvailableAsync(userId, locale);
            if (!available.AvailableHeroIds.Contains(selectedHeroId, StringComparer.OrdinalIgnoreCase))
                return new UpdateAlchimalianHeroProfileResponse { Success = false, ErrorMessage = "Hero is not available for your current tokens" };
        }
        await _repository.SaveUserAlchimalianProfileAsync(userId, selectedHeroId);
        var profile = await GetAlchimalianHeroProfileAsync(userId, locale);
        return new UpdateAlchimalianHeroProfileResponse { Success = true, Profile = profile };
    }

    private static readonly string[] PredominantOrder = { "courage", "curiosity", "thinking", "creativity", "safety" };
    private const int MinTokensToUnlockBase = 2;
    private static readonly int[] LevelThresholds = { 0, 10, 20 }; // 0-9 -> lvl1, 10-19 -> lvl2, 20+ -> lvl3

    public async Task<AlchimalianHeroAvailableResponse> GetAlchimalianHeroAvailableAsync(Guid userId, string locale)
    {
        var tokens = await _repository.GetUserTokensAsync(userId);
        var config = await _repository.GetTreeOfHeroesConfigAsync();

        var traitCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["courage"] = tokens.Courage,
            ["curiosity"] = tokens.Curiosity,
            ["thinking"] = tokens.Thinking,
            ["creativity"] = tokens.Creativity,
            ["safety"] = tokens.Safety
        };

        var availableHeroIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Base heroes (by trait + level from token count)
        foreach (var (trait, count) in traitCounts)
        {
            if (count < MinTokensToUnlockBase) continue;
            if (!config.HeroIdsByTraitAndLevel.TryGetValue(trait, out var chain) || chain.Count == 0) continue;
            var levelIndex = 0;
            for (var i = LevelThresholds.Length - 1; i >= 0; i--)
            {
                if (count >= LevelThresholds[i])
                {
                    levelIndex = Math.Min(i, chain.Count - 1);
                    break;
                }
            }
            availableHeroIds.Add(chain[levelIndex]);
        }

        // Hybrids: both base traits unlocked (>= MinTokensToUnlockBase), then pair -> hybrid
        var baseHeroIdsSet = new HashSet<string>(config.BaseHeroIds, StringComparer.OrdinalIgnoreCase);
        var availableBaseIds = new List<string>();
        foreach (var (trait, count) in traitCounts)
        {
            if (count < MinTokensToUnlockBase) continue;
            if (config.TraitToBaseHeroId.TryGetValue(trait, out var baseId) && baseHeroIdsSet.Contains(baseId))
                availableBaseIds.Add(baseId);
        }
        foreach (var a in availableBaseIds)
        {
            foreach (var b in availableBaseIds)
            {
                if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) continue;
                var key = string.Join("|", new[] { a, b }.OrderBy(x => x, StringComparer.Ordinal));
                if (config.CanonicalHybridByPair.TryGetValue(key, out var hybridId))
                    availableHeroIds.Add(hybridId);
            }
        }

        var list = availableHeroIds.ToList();

        // Recommended: predominant trait (by count, then PredominantOrder) + level
        string? recommendedHeroId = null;
        var maxCount = -1;
        var predominantTrait = (string?)null;
        foreach (var trait in PredominantOrder)
        {
            var c = traitCounts.GetValueOrDefault(trait, 0);
            if (c > maxCount && c >= MinTokensToUnlockBase)
            {
                maxCount = c;
                predominantTrait = trait;
            }
        }
        if (predominantTrait != null && config.HeroIdsByTraitAndLevel.TryGetValue(predominantTrait, out var recChain) && recChain.Count > 0)
        {
            var levelIndex = 0;
            for (var i = LevelThresholds.Length - 1; i >= 0; i--)
            {
                if (maxCount >= LevelThresholds[i])
                {
                    levelIndex = Math.Min(i, recChain.Count - 1);
                    break;
                }
            }
            recommendedHeroId = recChain[levelIndex];
            if (!list.Contains(recommendedHeroId, StringComparer.OrdinalIgnoreCase))
                list.Add(recommendedHeroId);
        }

        // Upgrade options: same as available (UI can show "?" for newly discoverable); no separate "discovered" state without token spend
        var upgradeOptionHeroIds = list.ToList();

        return new AlchimalianHeroAvailableResponse
        {
            AvailableHeroIds = list,
            RecommendedHeroId = recommendedHeroId,
            UpgradeOptionHeroIds = upgradeOptionHeroIds,
            Tokens = tokens
        };
    }

    public async Task<DiscoverAlchimalianHeroResponse> DiscoverAlchimalianHeroAsync(Guid userId, DiscoverAlchimalianHeroRequest request, string locale)
    {
        var heroId = string.IsNullOrWhiteSpace(request.HeroId) ? null : request.HeroId!.Trim();
        if (heroId == null)
            return new DiscoverAlchimalianHeroResponse { Success = false, ErrorMessage = "HeroId is required" };
        var available = await GetAlchimalianHeroAvailableAsync(userId, locale);
        if (!available.AvailableHeroIds.Contains(heroId, StringComparer.OrdinalIgnoreCase))
            return new DiscoverAlchimalianHeroResponse { Success = false, ErrorMessage = "Hero is not eligible for your current tokens" };
        await _repository.DiscoverHeroAsync(userId, heroId, request.SetAsSelected);
        var profile = await GetAlchimalianHeroProfileAsync(userId, locale);
        return new DiscoverAlchimalianHeroResponse { Success = true, Profile = profile };
    }

    private static IReadOnlyList<string> ParseDiscoveredHeroIds(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return Array.Empty<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json!) ?? new List<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

}
