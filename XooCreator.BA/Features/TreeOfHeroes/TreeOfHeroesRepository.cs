using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TreeOfHeroes;

public interface ITreeOfHeroesRepository
{
    Task<UserTokensDto> GetUserTokensAsync(Guid userId);
    Task<List<HeroDto>> GetHeroProgressAsync(Guid userId);
    Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId);
    Task<List<HeroDefinitionDto>> GetHeroDefinitionsAsync();
    Task<HeroDefinitionDto?> GetHeroDefinitionByIdAsync(string heroId);
    Task<TreeOfHeroesConfigDto> GetTreeOfHeroesConfigAsync();
    Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request);
    Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0);
    Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0);
}

public class TreeOfHeroesRepository : ITreeOfHeroesRepository
{
    private readonly XooDbContext _context;

    public TreeOfHeroesRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<UserTokensDto> GetUserTokensAsync(Guid userId)
    {
        var userTokens = await _context.UserTokens
            .FirstOrDefaultAsync(ut => ut.UserId == userId);

        if (userTokens == null)
        {
            // Create default tokens if user doesn't have any - 5 tokens of each type by default
            userTokens = new Data.UserTokens
            {
                UserId = userId,
                Courage = 5,
                Curiosity = 5,
                Thinking = 5,
                Creativity = 5,
                Safety = 5
            };
            _context.UserTokens.Add(userTokens);
            await _context.SaveChangesAsync();
        }

        return new UserTokensDto
        {
            Courage = userTokens.Courage,
            Curiosity = userTokens.Curiosity,
            Thinking = userTokens.Thinking,
            Creativity = userTokens.Creativity,
            Safety = userTokens.Safety
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

        var heroTreeNode = new Data.HeroTreeProgress
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
        var userTokens = await _context.UserTokens
            .FirstOrDefaultAsync(ut => ut.UserId == userId);

        if (userTokens == null)
        {
            return false;
        }

        // Check if user has enough tokens
        if (userTokens.Courage < courage ||
            userTokens.Curiosity < curiosity ||
            userTokens.Thinking < thinking ||
            userTokens.Creativity < creativity ||
            userTokens.Safety < safety)
        {
            return false;
        }

        // Spend tokens
        userTokens.Courage -= courage;
        userTokens.Curiosity -= curiosity;
        userTokens.Thinking -= thinking;
        userTokens.Creativity -= creativity;
        userTokens.Safety -= safety;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0)
    {
        var userTokens = await _context.UserTokens
            .FirstOrDefaultAsync(ut => ut.UserId == userId);

        if (userTokens == null)
        {
            // Create new user tokens if they don't exist
            userTokens = new Data.UserTokens
            {
                UserId = userId,
                Courage = courage,
                Curiosity = curiosity,
                Thinking = thinking,
                Creativity = creativity,
                Safety = safety
            };
            _context.UserTokens.Add(userTokens);
        }
        else
        {
            // Add tokens to existing record
            userTokens.Courage += courage;
            userTokens.Curiosity += curiosity;
            userTokens.Thinking += thinking;
            userTokens.Creativity += creativity;
            userTokens.Safety += safety;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<HeroDefinitionDto>> GetHeroDefinitionsAsync()
    {
        return await _context.HeroDefinitions
            .Select(hd => new HeroDefinitionDto
            {
                Id = hd.Id,
                Name = hd.Name,
                Description = hd.Description,
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

    public async Task<HeroDefinitionDto?> GetHeroDefinitionByIdAsync(string heroId)
    {
        var heroDefinition = await _context.HeroDefinitions
            .FirstOrDefaultAsync(hd => hd.Id == heroId);

        if (heroDefinition == null)
            return null;

        return new HeroDefinitionDto
        {
            Id = heroDefinition.Id,
            Name = heroDefinition.Name,
            Description = heroDefinition.Description,
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

    public Task<TreeOfHeroesConfigDto> GetTreeOfHeroesConfigAsync()
    {
        // Static configuration - can be moved to database later
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
            HeroImages = new List<HeroImageDto>
            {
                new() { Id = "hero_brave_puppy", Name = "Cățelul Curajos", Image = "images/heroes/hero0.jpg" },
                new() { Id = "hero_curious_cat", Name = "Pisica Curioasă", Image = "images/heroes/hero1.jpg" },
                new() { Id = "hero_wise_owl", Name = "Bufnița Înțeleaptă", Image = "images/heroes/hero2.jpg" },
                new() { Id = "hero_playful_horse", Name = "Căluțul Jucăuș", Image = "images/hero-tree/herotree1.jpg" },
                new() { Id = "hero_cautious_hedgehog", Name = "Ariciul Precaut", Image = "images/hero-tree/herotree2.jpg" },
                new() { Id = "hero_creative_guardian", Name = "Gardianul Creativ", Image = "images/heroes/hybrid_giraffe-cat.jpg" },
                new() { Id = "hero_alchimalian_dragon", Name = "Dragonul Alchimalian", Image = "images/hero-tree/herotree3.jpg" }
            },
            BaseHeroIds = new List<string>
            {
                "hero_brave_puppy",
                "hero_curious_cat", 
                "hero_wise_owl",
                "hero_playful_horse",
                "hero_cautious_hedgehog"
            },
            CanonicalHybridByPair = new Dictionary<string, string>
            {
                { "hero_cautious_hedgehog|hero_playful_horse", "hero_creative_guardian" }
            }
        };

        return Task.FromResult(config);
    }
}
