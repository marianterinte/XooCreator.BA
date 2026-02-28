using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;
using XooCreator.BA.Features.TreeOfLight;
using XooCreator.BA.Features.TreeOfLight.DTOs;
using XooCreator.BA.Services;

namespace XooCreator.BA.Features.TreeOfHeroes.Repositories;

public interface ITreeOfHeroesRepository
{
    Task<UserTokensDto> GetUserTokensAsync(Guid userId);
    Task<List<HeroDto>> GetHeroProgressAsync(Guid userId);
    Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId);
    Task<List<HeroDefinitionDto>> GetHeroDefinitionsAsync(string locale);
    Task<HeroDefinitionDto?> GetHeroDefinitionByIdAsync(string heroId, string locale);
    Task<TreeOfHeroesConfigDto> GetTreeOfHeroesConfigAsync();
    Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request, CancellationToken ct = default);
    Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0, CancellationToken ct = default);
    Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0, CancellationToken ct = default);
    Task<bool> AwardTokensAsync(Guid userId, IEnumerable<TokenReward> tokenRewards, CancellationToken ct = default);
    Task<bool> SaveHeroProgressAsync(Guid userId, string heroId, CancellationToken ct = default);
    Task<List<string>> AutoUnlockNodesBasedOnPrerequisitesAsync(Guid userId, CancellationToken ct = default);
    Task<ResetPersonalityTokensResult> ResetPersonalityTokensAsync(Guid userId, CancellationToken ct = default);
    Task<UserAlchimalianProfile?> GetUserAlchimalianProfileAsync(Guid userId);
    Task SaveUserAlchimalianProfileAsync(Guid userId, string? selectedHeroId, CancellationToken ct = default);
    /// <summary>Add heroId to discovered list; optionally set as selected. Returns true if updated.</summary>
    Task<bool> DiscoverHeroAsync(Guid userId, string heroId, bool setAsSelected, CancellationToken ct = default);
}

public class TreeOfHeroesRepository : ITreeOfHeroesRepository
{
    private const string TreeConfigCacheKey = "tree_of_heroes_config";
    private readonly XooDbContext _context;
    private readonly IMemoryCache _cache;

    public TreeOfHeroesRepository(XooDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<UserTokensDto> GetUserTokensAsync(Guid userId)
    {
        // Aggregate balances for the 5 core tokens from the generic ledger
        var balances = await _context.UserTokenBalances
            .Where(b => b.UserId == userId && b.Type == TokenFamily.Personality.ToString() &&
                (b.Value == "courage" || b.Value == "curiosity" || b.Value == "thinking" || b.Value == "creativity" || b.Value == "safety"))
            .ToListAsync();

        int Sum(string value) => balances.Where(b => b.Value == value).Sum(b => b.Quantity);

        return new UserTokensDto
        {
            Courage = Sum("courage"),
            Curiosity = Sum("curiosity"),
            Thinking = Sum("thinking"),
            Creativity = Sum("creativity"),
            Safety = Sum("safety")
        };
    }

    public async Task<List<HeroDto>> GetHeroProgressAsync(Guid userId)
    {
        return await _context.HeroProgress
            .Where(hp => hp.UserId == userId)
            .Select(hp => new HeroDto
            {
                HeroId = hp.HeroId,
                HeroType = hp.HeroType,
                SourceStoryId = hp.SourceStoryId,
                UnlockedAt = hp.UnlockedAt
            })
            .ToListAsync();
    }

    public async Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId)
    {
        return await _context.HeroTreeProgress
            .Where(htp => htp.UserId == userId)
            .Select(htp => new HeroTreeNodeDto
            {
                NodeId = htp.NodeId,
                TokensCostCourage = htp.TokensCostCourage,
                TokensCostCuriosity = htp.TokensCostCuriosity,
                TokensCostThinking = htp.TokensCostThinking,
                TokensCostCreativity = htp.TokensCostCreativity,
                TokensCostSafety = htp.TokensCostSafety,
                UnlockedAt = htp.UnlockedAt
            })
            .ToListAsync();
    }

    public async Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request, CancellationToken ct = default)
    {
        var existingNode = await _context.HeroTreeProgress
            .FirstOrDefaultAsync(htp => htp.UserId == userId && htp.NodeId == request.NodeId);

        if (existingNode != null)
        {
            return false; // Already unlocked
        }

        var heroTreeNode = new HeroTreeProgress
        {
            UserId = userId,
            NodeId = request.NodeId,
            TokensCostCourage = request.TokensCostCourage,
            TokensCostCuriosity = request.TokensCostCuriosity,
            TokensCostThinking = request.TokensCostThinking,
            TokensCostCreativity = request.TokensCostCreativity,
            TokensCostSafety = request.TokensCostSafety,
            UnlockedAt = DateTime.UtcNow
        };

        _context.HeroTreeProgress.Add(heroTreeNode);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0, CancellationToken ct = default)
    {
        // Check balances
        var current = await GetUserTokensAsync(userId);
        if (current.Courage < courage || current.Curiosity < curiosity || current.Thinking < thinking || current.Creativity < creativity || current.Safety < safety)
        {
            return false;
        }

        // Decrement from generic ledger (Type == Personality for core tokens)
        async Task Decrement(string value, int amount)
        {
            if (amount <= 0) return;
            var row = await _context.UserTokenBalances.FirstOrDefaultAsync(b => b.UserId == userId && b.Type == TokenFamily.Personality.ToString() && b.Value == value);
            if (row == null) return; // should not happen due to check above
            row.Quantity -= amount;
            row.UpdatedAt = DateTime.UtcNow;
        }

        await Decrement("courage", courage);
        await Decrement("curiosity", curiosity);
        await Decrement("thinking", thinking);
        await Decrement("creativity", creativity);
        await Decrement("safety", safety);

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0, CancellationToken ct = default)
    {
        var list = new List<TokenReward>();
        if (courage > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "courage", Quantity = courage });
        if (curiosity > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "curiosity", Quantity = curiosity });
        if (thinking > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "thinking", Quantity = thinking });
        if (creativity > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "creativity", Quantity = creativity });
        if (safety > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "safety", Quantity = safety });
        if (list.Count == 0) return true;
        return await AwardTokensAsync(userId, list, ct);
    }

    public async Task<bool> AwardTokensAsync(Guid userId, IEnumerable<TokenReward> tokenRewards, CancellationToken ct = default)
    {
        var rewardsList = tokenRewards.ToList();
        if (rewardsList.Count == 0)
            return true;

        // Batch load token balances for this user and relevant types (single query)
        var rewardTypes = rewardsList.Select(r => r.Type.ToString()).Distinct().ToList();
        var existingBalances = await _context.UserTokenBalances
            .Where(b => b.UserId == userId && rewardTypes.Contains(b.Type))
            .ToListAsync();
        var balanceByTypeValue = existingBalances.ToDictionary(b => (b.Type, b.Value));

        foreach (var reward in rewardsList)
        {
            var type = reward.Type;
            var value = reward.Value.Trim();
            var qty = reward.Quantity;
            var typeString = type.ToString();

            if (!balanceByTypeValue.TryGetValue((typeString, value), out var balance))
            {
                balance = new UserTokenBalance
                {
                    UserId = userId,
                    Type = typeString,
                    Value = value,
                    Quantity = qty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.UserTokenBalances.Add(balance);
                balanceByTypeValue[(typeString, value)] = balance;
            }
            else
            {
                balance.Quantity += qty;
                balance.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<HeroDefinitionDto>> GetHeroDefinitionsAsync(string locale)
    {
        return await _context.HeroDefinitionDefinitions
            .Include(hd => hd.Translations)
            .Select(hd => new HeroDefinitionDto
            {
                Id = hd.Id,
                Name = hd.Translations.FirstOrDefault(t => t.LanguageCode == locale).Name ?? hd.Id,
                Description = hd.Translations.FirstOrDefault(t => t.LanguageCode == locale).Description ?? string.Empty,
                Story = hd.Translations.FirstOrDefault(t => t.LanguageCode == locale).Story ?? string.Empty,
                Image = hd.Image,
                CourageCost = hd.CourageCost,
                CuriosityCost = hd.CuriosityCost,
                ThinkingCost = hd.ThinkingCost,
                CreativityCost = hd.CreativityCost,
                SafetyCost = hd.SafetyCost,
                PrerequisitesJson = hd.PrerequisitesJson,
                RewardsJson = hd.RewardsJson,
                IsUnlocked = hd.IsUnlocked,
                PositionX = hd.PositionX,
                PositionY = hd.PositionY
            })
            .ToListAsync();
    }

    public async Task<HeroDefinitionDto?> GetHeroDefinitionByIdAsync(string heroId, string locale)
    {
        var heroDefinition = await _context.HeroDefinitionDefinitions
            .Include(hd => hd.Translations)
            .FirstOrDefaultAsync(hd => hd.Id == heroId);

        if (heroDefinition == null)
            return null;

        var translation = heroDefinition.Translations.FirstOrDefault(t => t.LanguageCode == locale);

        return new HeroDefinitionDto
        {
            Id = heroDefinition.Id,
            Name = translation?.Name ?? heroDefinition.Id,
            Description = translation?.Description ?? string.Empty,
            Story = translation?.Story ?? string.Empty,
            Image = heroDefinition.Image,
            CourageCost = heroDefinition.CourageCost,
            CuriosityCost = heroDefinition.CuriosityCost,
            ThinkingCost = heroDefinition.ThinkingCost,
            CreativityCost = heroDefinition.CreativityCost,
            SafetyCost = heroDefinition.SafetyCost,
            PrerequisitesJson = heroDefinition.PrerequisitesJson,
            RewardsJson = heroDefinition.RewardsJson,
            IsUnlocked = heroDefinition.IsUnlocked,
            PositionX = heroDefinition.PositionX,
            PositionY = heroDefinition.PositionY
        };
    }

    public async Task<TreeOfHeroesConfigDto> GetTreeOfHeroesConfigAsync()
    {
        if (_cache.TryGetValue(TreeConfigCacheKey, out TreeOfHeroesConfigDto? cached) && cached != null)
            return cached;

        var definitions = await _context.HeroDefinitionDefinitions
            .Include(d => d.Translations)
            .ToListAsync();

        var heroImages = definitions.Select(d => new HeroImageDto
        {
            Id = d.Id,
            Name = d.Translations.FirstOrDefault()?.Name ?? d.Id,
            Image = d.Image
        }).ToList();

        var canonicalHybridByPair = new Dictionary<string, string>();
        foreach (var def in definitions)
        {
            var prereqs = SafeParsePrerequisites(def.PrerequisitesJson);
            if (prereqs.Count == 2)
            {
                var key = string.Join("|", prereqs.OrderBy(p => p));
                if (!canonicalHybridByPair.ContainsKey(key))
                {
                    canonicalHybridByPair[key] = def.Id;
                }
            }
        }

        var rootId = definitions.Any(d => d.Id == "alchimalia_hero") ? "alchimalia_hero" : "seed";
        var baseDefs = definitions
            .Where(d =>
            {
                var prereqs = SafeParsePrerequisites(d.PrerequisitesJson);
                return prereqs.Count == 1 && prereqs[0] == rootId;
            })
            .ToList();
        var baseHeroIds = baseDefs.Select(d => d.Id).ToList();

        // Trait -> base hero ID (level 1): base hero has exactly one cost = 1
        var traitToBaseHeroId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var d in baseDefs)
        {
            var trait = GetTraitFromCosts(d.CourageCost, d.CuriosityCost, d.ThinkingCost, d.CreativityCost, d.SafetyCost);
            if (trait != null)
                traitToBaseHeroId[trait] = d.Id;
        }

        // Trait -> [level1HeroId, level2HeroId, level3HeroId]
        var heroIdsByTraitAndLevel = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var defById = definitions.ToDictionary(d => d.Id, StringComparer.OrdinalIgnoreCase);
        foreach (var (trait, baseId) in traitToBaseHeroId)
        {
            var chain = new List<string> { baseId };
            var currentId = baseId;
            for (var level = 2; level <= 3; level++)
            {
                var next = definitions.FirstOrDefault(d =>
                {
                    var prereqs = SafeParsePrerequisites(d.PrerequisitesJson);
                    return prereqs.Count == 1 && prereqs[0].Equals(currentId, StringComparison.OrdinalIgnoreCase);
                });
                if (next == null) break;
                chain.Add(next.Id);
                currentId = next.Id;
            }
            heroIdsByTraitAndLevel[trait] = chain;
        }

        var config = new TreeOfHeroesConfigDto
        {
            Tokens = new List<TokenConfigDto>
            {
                new() { Id = "token_courage", Label = "Curaj", Trait = "courage", Icon = "🦁", Angle = -Math.PI * 0.15 },
                new() { Id = "token_curiosity", Label = "Curiozitate", Trait = "curiosity", Icon = "🔍", Angle = -Math.PI * 0.35 },
                new() { Id = "token_thinking", Label = "Gândire", Trait = "thinking", Icon = "🧠", Angle = -Math.PI * 0.55 },
                new() { Id = "token_creativity", Label = "Creativitate", Trait = "creativity", Icon = "🎨", Angle = -Math.PI * 0.75 },
                new() { Id = "token_safety", Label = "Siguranță", Trait = "safety", Icon = "🛡️", Angle = -Math.PI * 0.95 }
            },
            HeroImages = heroImages,
            BaseHeroIds = baseHeroIds,
            CanonicalHybridByPair = canonicalHybridByPair,
            TraitToBaseHeroId = traitToBaseHeroId,
            HeroIdsByTraitAndLevel = heroIdsByTraitAndLevel
        };

        _cache.Set(TreeConfigCacheKey, config, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
            Size = 10
        });

        return config;
    }

    public async Task<ResetPersonalityTokensResult> ResetPersonalityTokensAsync(Guid userId, CancellationToken ct = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(ct);
        
        try
        {
            // Get all hero tree progress for user to calculate tokens to return
            var heroTreeProgress = await _context.HeroTreeProgress
                .Where(htp => htp.UserId == userId)
                .ToListAsync(ct);

            // Calculate total tokens consumed
            int totalCourage = heroTreeProgress.Sum(h => h.TokensCostCourage);
            int totalCuriosity = heroTreeProgress.Sum(h => h.TokensCostCuriosity);
            int totalThinking = heroTreeProgress.Sum(h => h.TokensCostThinking);
            int totalCreativity = heroTreeProgress.Sum(h => h.TokensCostCreativity);
            int totalSafety = heroTreeProgress.Sum(h => h.TokensCostSafety);

            // Return tokens to user
            if (totalCourage > 0 || totalCuriosity > 0 || totalThinking > 0 || totalCreativity > 0 || totalSafety > 0)
            {
                await AwardTokensAsync(userId, totalCourage, totalCuriosity, totalThinking, totalCreativity, totalSafety, ct);
            }

            // Delete hero tree progress
            if (heroTreeProgress.Count > 0)
            {
                _context.HeroTreeProgress.RemoveRange(heroTreeProgress);
            }

            // Delete hero progress
            var heroProgress = await _context.HeroProgress
                .Where(hp => hp.UserId == userId)
                .ToListAsync();
            
            if (heroProgress.Count > 0)
            {
                _context.HeroProgress.RemoveRange(heroProgress);
            }

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return new ResetPersonalityTokensResult
            {
                Success = true,
                TokensReturned = totalCourage + totalCuriosity + totalThinking + totalCreativity + totalSafety
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            return new ResetPersonalityTokensResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                TokensReturned = 0
            };
        }
    }

    public async Task<bool> SaveHeroProgressAsync(Guid userId, string heroId, CancellationToken ct = default)
    {
        // Check if hero progress already exists
        var existingProgress = await _context.HeroProgress
            .FirstOrDefaultAsync(hp => hp.UserId == userId && hp.HeroId == heroId, ct);

        if (existingProgress != null)
        {
            // Hero already transformed, no need to save again
            return true;
        }

        // Create new hero progress record
        var heroProgress = new HeroProgress
        {
            UserId = userId,
            HeroId = heroId,
            HeroType = "HERO_TREE_TRANSFORMATION",
            UnlockedAt = DateTime.UtcNow
        };

        _context.HeroProgress.Add(heroProgress);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<string>> AutoUnlockNodesBasedOnPrerequisitesAsync(Guid userId, CancellationToken ct = default)
    {
        var newlyUnlockedNodes = new List<string>();

        // Get all hero definitions
        var allHeroDefinitions = await _context.HeroDefinitionDefinitions.ToListAsync();
        
        // Get user's current progress (transformed heroes)
        var userHeroProgress = await _context.HeroProgress
            .Where(hp => hp.UserId == userId)
            .Select(hp => hp.HeroId)
            .ToListAsync();

        // Get already unlocked nodes
        var alreadyUnlockedNodes = await _context.HeroTreeProgress
            .Where(htp => htp.UserId == userId)
            .Select(htp => htp.NodeId)
            .ToListAsync();

        // Base hero IDs (these are the only heroes that can be unlocked when seed is transformed)
        var baseHeroIds = new[] { "hero_brave_puppy", "hero_curious_cat", "hero_wise_owl", "hero_playful_horse", "hero_cautious_hedgehog" };

        // Check each hero definition to see if it should be unlocked
        foreach (var heroDef in allHeroDefinitions)
        {
            // Skip if already unlocked
            if (alreadyUnlockedNodes.Contains(heroDef.Id))
                continue;

            // Parse prerequisites
            var prerequisites = SafeParsePrerequisites(heroDef.PrerequisitesJson);
            
            // Special logic for base heroes: only unlock when seed is transformed
            if (baseHeroIds.Contains(heroDef.Id))
            {
                if (userHeroProgress.Contains("seed"))
                {
                    // Unlock this base hero
                    var heroTreeNode = new HeroTreeProgress
                    {
                        UserId = userId,
                        NodeId = heroDef.Id,
                        TokensCostCourage = heroDef.CourageCost,
                        TokensCostCuriosity = heroDef.CuriosityCost,
                        TokensCostThinking = heroDef.ThinkingCost,
                        TokensCostCreativity = heroDef.CreativityCost,
                        TokensCostSafety = heroDef.SafetyCost,
                        UnlockedAt = DateTime.UtcNow
                    };

                    _context.HeroTreeProgress.Add(heroTreeNode);
                    newlyUnlockedNodes.Add(heroDef.Id);
                }
            }
            else
            {
                // For hybrids and legendary: check if all prerequisites are TRANSFORMED
                bool allPrerequisitesMet = prerequisites.All(prereq => 
                    userHeroProgress.Contains(prereq));
                
                if (allPrerequisitesMet)
                {
                    // Unlock this node
                    var heroTreeNode = new HeroTreeProgress
                    {
                        UserId = userId,
                        NodeId = heroDef.Id,
                        TokensCostCourage = heroDef.CourageCost,
                        TokensCostCuriosity = heroDef.CuriosityCost,
                        TokensCostThinking = heroDef.ThinkingCost,
                        TokensCostCreativity = heroDef.CreativityCost,
                        TokensCostSafety = heroDef.SafetyCost,
                        UnlockedAt = DateTime.UtcNow
                    };

                    _context.HeroTreeProgress.Add(heroTreeNode);
                    newlyUnlockedNodes.Add(heroDef.Id);
                }
            }
        }

        // Save all newly unlocked nodes
        if (newlyUnlockedNodes.Any())
        {
            await _context.SaveChangesAsync(ct);
        }

        return newlyUnlockedNodes;
    }

    public async Task<UserAlchimalianProfile?> GetUserAlchimalianProfileAsync(Guid userId)
    {
        return await _context.UserAlchimalianProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task SaveUserAlchimalianProfileAsync(Guid userId, string? selectedHeroId, CancellationToken ct = default)
    {
        var profile = await _context.UserAlchimalianProfiles.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        var now = DateTime.UtcNow;
        if (profile == null)
        {
            profile = new UserAlchimalianProfile
            {
                UserId = userId,
                SelectedHeroId = selectedHeroId,
                DiscoveredHeroIdsJson = "[]",
                UpdatedAt = now
            };
            _context.UserAlchimalianProfiles.Add(profile);
        }
        else
        {
            profile.SelectedHeroId = selectedHeroId;
            profile.UpdatedAt = now;
        }
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> DiscoverHeroAsync(Guid userId, string heroId, bool setAsSelected, CancellationToken ct = default)
    {
        var profile = await _context.UserAlchimalianProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        var list = SafeParseDiscoveredHeroIds(profile?.DiscoveredHeroIdsJson);
        if (list.Contains(heroId, StringComparer.OrdinalIgnoreCase) && !setAsSelected)
            return true; // already discovered, no change
        if (!list.Contains(heroId, StringComparer.OrdinalIgnoreCase))
            list.Add(heroId);
        var now = DateTime.UtcNow;
        if (profile == null)
        {
            profile = new UserAlchimalianProfile
            {
                UserId = userId,
                SelectedHeroId = setAsSelected ? heroId : null,
                DiscoveredHeroIdsJson = JsonSerializer.Serialize(list),
                UpdatedAt = now
            };
            _context.UserAlchimalianProfiles.Add(profile);
        }
        else
        {
            profile.DiscoveredHeroIdsJson = JsonSerializer.Serialize(list);
            if (setAsSelected)
                profile.SelectedHeroId = heroId;
            profile.UpdatedAt = now;
        }
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private static List<string> SafeParseDiscoveredHeroIds(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<string>();
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json!) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static string? GetTraitFromCosts(int courage, int curiosity, int thinking, int creativity, int safety)
    {
        if (courage == 1 && curiosity == 0 && thinking == 0 && creativity == 0 && safety == 0) return "courage";
        if (courage == 0 && curiosity == 1 && thinking == 0 && creativity == 0 && safety == 0) return "curiosity";
        if (courage == 0 && curiosity == 0 && thinking == 1 && creativity == 0 && safety == 0) return "thinking";
        if (courage == 0 && curiosity == 0 && thinking == 0 && creativity == 1 && safety == 0) return "creativity";
        if (courage == 0 && curiosity == 0 && thinking == 0 && creativity == 0 && safety == 1) return "safety";
        return null;
    }

    private static List<string> SafeParsePrerequisites(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<string>();
        var s = json!.Trim();
        try
        {
            // Accept JSON array: ["alchimalia_hero"] or ["hero_curious_cat","hero_wise_owl"]
            if (s.StartsWith('['))
                return JsonSerializer.Deserialize<List<string>>(s) ?? new List<string>();
            // Accept single quoted string from seed: "alchimalia_hero" -> treat as single prereq
            if (s.StartsWith('"') && s.EndsWith('"'))
            {
                var single = JsonSerializer.Deserialize<string>(s);
                return string.IsNullOrEmpty(single) ? new List<string>() : new List<string> { single! };
            }
            return new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
