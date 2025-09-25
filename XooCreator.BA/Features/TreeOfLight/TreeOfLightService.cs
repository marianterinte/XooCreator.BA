using XooCreator.BA.Features.Stories;
using XooCreator.BA.Features.TreeOfHeroes;

namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeOfLightService
{
    Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId);
    Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId);
    Task<CompleteStoryResponse> CompleteStoryAsync(Guid userId, CompleteStoryRequest request);
    Task<ResetProgressResponse> ResetUserProgressAsync(Guid userId);
}

public class TreeOfLightService : ITreeOfLightService
{
    private readonly ITreeOfLightRepository _repository;
    private readonly IStoriesRepository _storiesRepository;
    private readonly ITreeOfHeroesRepository _treeOfHeroesRepository;

    public TreeOfLightService(ITreeOfLightRepository repository, IStoriesRepository storiesRepository, ITreeOfHeroesRepository treeOfHeroesRepository)
    {
        _repository = repository;
        _storiesRepository = storiesRepository;
        _treeOfHeroesRepository = treeOfHeroesRepository;
    }

    public Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId)
    {
        return _repository.GetTreeProgressAsync(userId);
    }

    public Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId)
    {
        return _repository.GetStoryProgressAsync(userId);
    }


    public async Task<CompleteStoryResponse> CompleteStoryAsync(Guid userId, CompleteStoryRequest request)
    {
        try
        {
            // Complete the story
            var story = await _storiesRepository.GetStoryByIdAsync(request.StoryId);

            var storyCompleted = await _repository.CompleteStoryAsync(userId, request, story);
            if (!storyCompleted)
            {
                return new CompleteStoryResponse
                {
                    Success = false,
                    ErrorMessage = "Story already completed or error occurred"
                };
            }

            var newlyUnlockedRegions = new List<string>();

            // Award tokens: prefer request.Tokens if provided; otherwise derive from story answer
            var effectiveTokens = new List<TokenReward>();

            if (request.Tokens != null && request.Tokens.Count > 0)
            {
                effectiveTokens.AddRange(request.Tokens);
            }
            else if (story != null && !string.IsNullOrEmpty(request.SelectedAnswer))
            {
                var quizTile = story.Tiles.FirstOrDefault(t => t.Type == "quiz");
                if (quizTile != null)
                {
                    var selectedAnswer = quizTile.Answers.FirstOrDefault(a => a.Id == request.SelectedAnswer);
                    if (selectedAnswer != null && selectedAnswer.Tokens.Count > 0)
                    {
                        effectiveTokens.AddRange(selectedAnswer.Tokens);
                    }
                }
            }

            if (effectiveTokens.Count > 0)
            {
                await _treeOfHeroesRepository.AwardTokensAsync(userId, effectiveTokens);
            }

            // Check for region unlocks based on story completion rules
            var unlockedRegions = await CheckAndUnlockRegionsAsync(userId, request.StoryId);
            newlyUnlockedRegions.AddRange(unlockedRegions);

            var updatedTokens = await _treeOfHeroesRepository.GetUserTokensAsync(userId);

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



    // Deprecated: kept for reference; new flow uses AwardTokensAsync(userId, IEnumerable<TokenReward>)
    private async Task AwardTokensByType(Guid userId, string tokenType, int quantity)
    {
        await _treeOfHeroesRepository.AwardTokensAsync(userId, new[] { new TokenReward { Type = "TreeOfHeroes", Value = tokenType, Quantity = quantity } });
    }

    private async Task AwardTokensByReward(Guid userId, string reward)
    {
        // Convert reward string to token type and quantity
        // Examples: "token_courage", "token_curiosity", "token_thinking", "token_creativity", "token_safety"
        switch (reward.ToLower())
        {
            case "token_courage":
                await _treeOfHeroesRepository.AwardTokensAsync(userId, courage: 1);
                break;
            case "token_curiosity":
                await _treeOfHeroesRepository.AwardTokensAsync(userId, curiosity: 1);
                break;
            case "token_thinking":
                await _treeOfHeroesRepository.AwardTokensAsync(userId, thinking: 1);
                break;
            case "token_creativity":
                await _treeOfHeroesRepository.AwardTokensAsync(userId, creativity: 1);
                break;
            case "token_safety":
                await _treeOfHeroesRepository.AwardTokensAsync(userId, safety: 1);
                break;
            // For other rewards (like fruits), do nothing - they're handled elsewhere
        }
    }

    private async Task<List<string>> CheckAndUnlockRegionsAsync(Guid userId, string storyId)
    {
        var newlyUnlockedRegions = new List<string>();
        
        // Check for region unlock rules based on story completion
        // This could be made more dynamic by loading rules from database
        switch (storyId)
        {
            case "root-s1":
                await _repository.UnlockRegionAsync(userId, "farm");
                newlyUnlockedRegions.Add("farm");
                break;
                
            case "farm-s1":
            case "farm-s2":
            case "farm-s3":
                // Check if farm is complete (all 3 stories done)
                var farmStories = await _repository.GetStoryProgressAsync(userId);
                var farmCompleted = farmStories.Count(s => s.StoryId.StartsWith("farm-")) >= 3;
                
                if (farmCompleted)
                {
                    // Unlock next regions based on collected tokens
                    var tokens = await _treeOfHeroesRepository.GetUserTokensAsync(userId);
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
        }
        
        return newlyUnlockedRegions;
    }


    public async Task<ResetProgressResponse> ResetUserProgressAsync(Guid userId)
    {
        try
        {
            await _repository.ResetUserProgressAsync(userId);
            
            return new ResetProgressResponse
            {
                Success = true,
                Message = "User progress has been successfully reset."
            };
        }
        catch (Exception ex)
        {
            return new ResetProgressResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
