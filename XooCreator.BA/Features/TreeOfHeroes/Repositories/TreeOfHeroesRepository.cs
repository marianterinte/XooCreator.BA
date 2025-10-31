using Microsoft.EntityFrameworkCore;
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
    Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request);
    Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0);
    Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0);
    Task<bool> AwardTokensAsync(Guid userId, IEnumerable<TokenReward> tokenRewards);
    Task<bool> SaveHeroProgressAsync(Guid userId, string heroId);
    Task<List<string>> AutoUnlockNodesBasedOnPrerequisitesAsync(Guid userId);
}

public class TreeOfHeroesRepository : ITreeOfHeroesRepository
{
    private readonly XooDbContext _context;
    private readonly IHeroTreeProvider _heroTreeProvider;

    public TreeOfHeroesRepository(XooDbContext context, IHeroTreeProvider heroTreeProvider)
    {
        _context = context;
        _heroTreeProvider = heroTreeProvider;
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

    public async Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request)
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
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0)
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

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0)
    {
        var list = new List<TokenReward>();
        if (courage > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "courage", Quantity = courage });
        if (curiosity > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "curiosity", Quantity = curiosity });
        if (thinking > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "thinking", Quantity = thinking });
        if (creativity > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "creativity", Quantity = creativity });
        if (safety > 0) list.Add(new TokenReward { Type = TokenFamily.Personality, Value = "safety", Quantity = safety });
        if (list.Count == 0) return true;
        return await AwardTokensAsync(userId, list);
    }

    public async Task<bool> AwardTokensAsync(Guid userId, IEnumerable<TokenReward> tokenRewards)
    {
        foreach (var reward in tokenRewards)
        {
            var type = reward.Type;
            var value = reward.Value.Trim();
            var qty = reward.Quantity;

            // Persist into generic ledger once; no recursion
            var typeString = type.ToString();
            var balance = await _context.UserTokenBalances
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Type == typeString && b.Value == value);

            if (balance == null)
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
            }
            else
            {
                balance.Quantity += qty;
                balance.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<HeroDefinitionDto>> GetHeroDefinitionsAsync(string locale)
    {
        return await _context.HeroDefinitions
            .Include(hd => hd.Translations)
            .Select(hd => new HeroDefinitionDto
            {
                Id = hd.Id,
                Name = hd.Translations.FirstOrDefault(t => t.LanguageCode == locale).Name ?? hd.Id,
                Description = hd.Translations.FirstOrDefault(t => t.LanguageCode == locale).Description ?? string.Empty,
                Story = hd.Translations.FirstOrDefault(t => t.LanguageCode == locale).Story ?? string.Empty,
                Image = hd.Image,
                Type = hd.Type,
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
        var heroDefinition = await _context.HeroDefinitions
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
            Type = heroDefinition.Type,
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
        var heroTree = await _heroTreeProvider.GetHeroTree(LanguageCode.RoRo);
        var heroTreeStructure = await _heroTreeProvider.GetHeroTreeStructure();

        var heroImages = heroTree?.Nodes.Select(n => new HeroImageDto
        {
            Id = n.Id,
            Name = n.Name,
            Image = n.Image
        }).ToList() ?? new List<HeroImageDto>();

        var canonicalHybridByPair = new Dictionary<string, string>();
        if (heroTreeStructure != null)
        {
            foreach (var node in heroTreeStructure.Nodes)
            {
                if (node.Prerequisites != null && node.Prerequisites.Count == 2)
                {
                    var key = string.Join("|", node.Prerequisites.OrderBy(p => p));
                    canonicalHybridByPair[key] = node.Id;
                }
            }
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
            BaseHeroIds = heroTree?.BaseHeroIds ?? new List<string>(),
            CanonicalHybridByPair = canonicalHybridByPair
        };

        return config;
    }

    public async Task<bool> SaveHeroProgressAsync(Guid userId, string heroId)
    {
        // Check if hero progress already exists
        var existingProgress = await _context.HeroProgress
            .FirstOrDefaultAsync(hp => hp.UserId == userId && hp.HeroId == heroId);

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
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<string>> AutoUnlockNodesBasedOnPrerequisitesAsync(Guid userId)
    {
        var newlyUnlockedNodes = new List<string>();

        // Get all hero definitions
        var allHeroDefinitions = await _context.HeroDefinitions.ToListAsync();
        
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
            var prerequisites = JsonSerializer.Deserialize<List<string>>(heroDef.PrerequisitesJson) ?? new List<string>();
            
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
            await _context.SaveChangesAsync();
        }

        return newlyUnlockedNodes;
    }
}
