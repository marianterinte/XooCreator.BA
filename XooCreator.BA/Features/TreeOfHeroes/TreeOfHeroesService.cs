namespace XooCreator.BA.Features.TreeOfHeroes;

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

    public TreeOfHeroesService(ITreeOfHeroesRepository repository)
    {
        _repository = repository;
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
