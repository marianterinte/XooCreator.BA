namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeOfLightService
{
    Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId);
    Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId);
    Task<UserTokensDto> GetUserTokensAsync(Guid userId);
    Task<List<HeroDto>> GetHeroProgressAsync(Guid userId);
    Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId);
    Task<CompleteStoryResponse> CompleteStoryAsync(Guid userId, CompleteStoryRequest request);
    Task<UnlockHeroTreeNodeResponse> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request);
    Task<TransformToHeroResponse> TransformToHeroAsync(Guid userId, TransformToHeroRequest request);
}

public class TreeOfLightService : ITreeOfLightService
{
    private readonly ITreeOfLightRepository _repository;

    public TreeOfLightService(ITreeOfLightRepository repository)
    {
        _repository = repository;
    }

    public Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId)
    {
        return _repository.GetTreeProgressAsync(userId);
    }

    public Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId)
    {
        return _repository.GetStoryProgressAsync(userId);
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

    public async Task<CompleteStoryResponse> CompleteStoryAsync(Guid userId, CompleteStoryRequest request)
    {
        try
        {
            // Complete the story
            var storyCompleted = await _repository.CompleteStoryAsync(userId, request);
            if (!storyCompleted)
            {
                return new CompleteStoryResponse
                {
                    Success = false,
                    ErrorMessage = "Story already completed or error occurred"
                };
            }

            var newlyUnlockedRegions = new List<string>();

            // Award tokens and unlock heroes based on story completion logic
            switch (request.StoryId)
            {
                case "root-s1":
                    await _repository.AwardTokensAsync(userId, courage: 1);
                    await _repository.UnlockHeroAsync(userId, "COMPANION_PUFPUF", "STORY_REWARD", request.StoryId);
                    await _repository.UnlockRegionAsync(userId, "farm");
                    newlyUnlockedRegions.Add("farm");
                    break;
                
                case "farm-s1":
                case "farm-s2":
                case "farm-s3":
                    // Award tokens based on answer choice
                    await AwardTokensForAnswer(userId, request.SelectedAnswer);
                    
                    // Check if farm is complete (all 3 stories done)
                    var farmStories = await _repository.GetStoryProgressAsync(userId);
                    var farmCompleted = farmStories.Count(s => s.StoryId.StartsWith("farm-")) >= 3;
                    
                    if (farmCompleted)
                    {
                        // Unlock next regions based on collected tokens
                        var tokens = await _repository.GetUserTokensAsync(userId);
                        if (tokens.Courage >= 2)
                        {
                            await _repository.UnlockRegionAsync(userId, "sahara");
                            newlyUnlockedRegions.Add("sahara");
                        }
                        if (tokens.Curiosity + tokens.Creativity >= 2)
                        {
                            await _repository.UnlockRegionAsync(userId, "dreamland");
                            newlyUnlockedRegions.Add("dreamland");
                        }
                    }
                    break;
                
                // Add other story cases as needed
            }

            // Award story-specific hero
            if (!string.IsNullOrEmpty(request.RewardReceived))
            {
                await _repository.UnlockHeroAsync(userId, request.RewardReceived, "STORY_REWARD", request.StoryId);
            }

            var updatedTokens = await _repository.GetUserTokensAsync(userId);

            return new CompleteStoryResponse
            {
                Success = true,
                NewlyUnlockedRegions = newlyUnlockedRegions,
                UpdatedTokens = updatedTokens
            };
        }
        catch (Exception ex)
        {
            return new CompleteStoryResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
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
                currentTokens.Creativity < request.TokensCostCreativity)
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
                request.TokensCostCreativity);

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

            // Unlock the hero
            var heroUnlocked = await _repository.UnlockHeroAsync(userId, request.HeroId, "HERO_TREE_UNLOCK");
            if (!heroUnlocked)
            {
                return new TransformToHeroResponse
                {
                    Success = false,
                    ErrorMessage = "Hero already unlocked or error occurred"
                };
            }

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

    private async Task AwardTokensForAnswer(Guid userId, string? answer)
    {
        // Award different tokens based on answer choice
        switch (answer?.ToLower())
        {
            case "courage":
            case "brave":
                await _repository.AwardTokensAsync(userId, courage: 1);
                break;
            case "curiosity":
            case "curious":
                await _repository.AwardTokensAsync(userId, curiosity: 1);
                break;
            case "thinking":
            case "think":
                await _repository.AwardTokensAsync(userId, thinking: 1);
                break;
            case "creativity":
            case "creative":
                await _repository.AwardTokensAsync(userId, creativity: 1);
                break;
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
