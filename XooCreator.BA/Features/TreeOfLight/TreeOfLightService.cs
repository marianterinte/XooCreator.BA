using XooCreator.BA.Features.Stories;

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

    public TreeOfLightService(ITreeOfLightRepository repository, IStoriesRepository storiesRepository)
    {
        _repository = repository;
        _storiesRepository = storiesRepository;
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

            // Load story from database to get the correct tokens
            if (story != null && !string.IsNullOrEmpty(request.SelectedAnswer))
            {
                // Find the quiz tile and selected answer
                var quizTile = story.Tiles.FirstOrDefault(t => t.Type == "quiz");
                if (quizTile != null)
                {
                    var selectedAnswer = quizTile.Answers.FirstOrDefault(a => a.Id == request.SelectedAnswer);
                    if (selectedAnswer != null && selectedAnswer.Tokens.Count > 0)
                    {
                        // Award tokens based on the tokens from database
                        foreach (var tokenReward in selectedAnswer.Tokens)
                        {
                            await AwardTokensByType(userId, tokenReward.TokenType, tokenReward.Quantity);
                        }
                    }
                }
            }

            // Check for region unlocks based on story completion rules
            var unlockedRegions = await CheckAndUnlockRegionsAsync(userId, request.StoryId);
            newlyUnlockedRegions.AddRange(unlockedRegions);

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



    private async Task AwardTokensByType(Guid userId, string tokenType, int quantity)
    {
        // Award tokens based on token type
        switch (tokenType.ToLower())
        {
            case "token_courage":
            case "courage":
                await _repository.AwardTokensAsync(userId, courage: quantity);
                break;
            case "token_curiosity":
            case "curiosity":
                await _repository.AwardTokensAsync(userId, curiosity: quantity);
                break;
            case "token_thinking":
            case "thinking":
                await _repository.AwardTokensAsync(userId, thinking: quantity);
                break;
            case "token_creativity":
            case "creativity":
                await _repository.AwardTokensAsync(userId, creativity: quantity);
                break;
        }
    }

    private async Task AwardTokensByReward(Guid userId, string reward)
    {
        // Convert reward string to token type and quantity
        // Examples: "token_courage", "token_curiosity", "token_thinking", "token_creativity", "token_safety"
        switch (reward.ToLower())
        {
            case "token_courage":
                await _repository.AwardTokensAsync(userId, courage: 1);
                break;
            case "token_curiosity":
                await _repository.AwardTokensAsync(userId, curiosity: 1);
                break;
            case "token_thinking":
                await _repository.AwardTokensAsync(userId, thinking: 1);
                break;
            case "token_creativity":
                await _repository.AwardTokensAsync(userId, creativity: 1);
                break;
            case "token_safety":
                await _repository.AwardTokensAsync(userId, safety: 1);
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
