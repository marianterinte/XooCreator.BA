using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;

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
}

public class TreeOfHeroesService : ITreeOfHeroesService
{
    private readonly ITreeOfHeroesRepository _repository;
    private readonly XooDbContext _db;

    public TreeOfHeroesService(ITreeOfHeroesRepository repository, XooDbContext db)
    {
        _repository = repository;
        _db = db;
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

    public Task<List<HeroDefinitionDto>> GetHeroDefinitionsAsync(string locale)
    {
        return _repository.GetHeroDefinitionsAsync(locale);
    }

    public Task<HeroDefinitionDto?> GetHeroDefinitionByIdAsync(string heroId, string locale)
    {
        return _repository.GetHeroDefinitionByIdAsync(heroId, locale);
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
                Console.WriteLine($"Failed to save hero to bestiary: {ex.Message}");
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

}
