namespace XooCreator.BA.Features.TreeOfHeroes;

public interface ITreeOfHeroesService
{
    Task<UserTokensDto> GetUserTokensAsync(Guid userId);
    Task<List<HeroDto>> GetHeroProgressAsync(Guid userId);
    Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId);
    Task<UnlockHeroTreeNodeResponse> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request);
    Task<TransformToHeroResponse> TransformToHeroAsync(Guid userId, TransformToHeroRequest request);
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

    public async Task<UnlockHeroTreeNodeResponse> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request)
    {
        try
        {
            // Check if user has enough tokens
            var currentTokens = await _repository.GetUserTokensAsync(userId);
            
            if (currentTokens.Courage < request.TokensCostCourage ||
                currentTokens.Curiosity < request.TokensCostCuriosity ||
                currentTokens.Thinking < request.TokensCostThinking ||
                currentTokens.Creativity < request.TokensCostCreativity ||
                currentTokens.Safety < request.TokensCostSafety)
            {
                return new UnlockHeroTreeNodeResponse
                {
                    Success = false,
                    ErrorMessage = "Insufficient tokens"
                };
            }

            // Spend tokens
            var tokensSpent = await _repository.SpendTokensAsync(userId, 
                request.TokensCostCourage,
                request.TokensCostCuriosity,
                request.TokensCostThinking,
                request.TokensCostCreativity,
                request.TokensCostSafety);

            if (!tokensSpent)
            {
                return new UnlockHeroTreeNodeResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to spend tokens"
                };
            }

            // Unlock the node
            var nodeUnlocked = await _repository.UnlockHeroTreeNodeAsync(userId, request);
            if (!nodeUnlocked)
            {
                return new UnlockHeroTreeNodeResponse
                {
                    Success = false,
                    ErrorMessage = "Node already unlocked or error occurred"
                };
            }

            var updatedTokens = await _repository.GetUserTokensAsync(userId);

            return new UnlockHeroTreeNodeResponse
            {
                Success = true,
                UpdatedTokens = updatedTokens
            };
        }
        catch (Exception ex)
        {
            return new UnlockHeroTreeNodeResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<TransformToHeroResponse> TransformToHeroAsync(Guid userId, TransformToHeroRequest request)
    {
        try
        {
            // Check hero transformation requirements based on heroId
            var tokens = await _repository.GetUserTokensAsync(userId);
            var canTransform = CanTransformToHero(request.HeroId, tokens);

            if (!canTransform)
            {
                return new TransformToHeroResponse
                {
                    Success = false,
                    ErrorMessage = "Requirements not met for this hero transformation"
                };
            }

            // Hero unlocking is now handled by frontend localStorage
            // No need to unlock in backend

            var unlockedHero = new HeroDto
            {
                HeroId = request.HeroId,
                HeroType = "HERO_TREE_UNLOCK",
                UnlockedAt = DateTime.UtcNow
            };

            return new TransformToHeroResponse
            {
                Success = true,
                UnlockedHero = unlockedHero
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

    private static bool CanTransformToHero(string heroId, UserTokensDto tokens)
    {
        return heroId.ToUpper() switch
        {
            "LEO" => tokens.Courage >= 8,
            "FOX" => tokens.Curiosity >= 8,
            "OWL" => tokens.Thinking >= 8,
            "DRAGONFLY" => tokens.Creativity >= 8,
            "WOLF_SAGE" => tokens.Courage >= 4 && tokens.Thinking >= 4,
            "TRICKSTER_SPRITE" => tokens.Curiosity >= 4 && tokens.Creativity >= 4,
            "UNICORN" => tokens.Courage >= 3 && tokens.Curiosity >= 3 && tokens.Thinking >= 3 && tokens.Creativity >= 3,
            _ => false
        };
    }
}
