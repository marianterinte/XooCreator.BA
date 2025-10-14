using XooCreator.BA.Features.Stories;
using XooCreator.BA.Features.TreeOfHeroes;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Data;
using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeOfLightService
{
    Task<List<TreeConfigurationDto>> GetAllConfigurationsAsync();
    Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId, string configId);
    Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId, string configId);
    Task<CompleteStoryResponse> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, string configId);
    Task<ResetProgressResponse> ResetUserProgressAsync(Guid userId);
}

public class TreeOfLightService : ITreeOfLightService
{
    private readonly ITreeOfLightRepository _repository;
    private readonly IStoriesRepository _storiesRepository;
    private readonly ITreeOfHeroesRepository _treeOfHeroesRepository;
    private readonly IUserContextService _userContext;
    private readonly XooDbContext _dbContext;

    public TreeOfLightService(ITreeOfLightRepository repository, IStoriesRepository storiesRepository, ITreeOfHeroesRepository treeOfHeroesRepository, IUserContextService userContext, XooDbContext dbContext)
    {
        _repository = repository;
        _storiesRepository = storiesRepository;
        _treeOfHeroesRepository = treeOfHeroesRepository;
        _userContext = userContext;
        _dbContext = dbContext;
    }

    public async Task<List<TreeConfigurationDto>> GetAllConfigurationsAsync()
    {
        var configs = await _repository.GetAllConfigurationsAsync();
        return configs.Select(c => new TreeConfigurationDto { Id = c.Id, Name = c.Name, IsDefault = c.IsDefault }).ToList();
    }

    public Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId, string configId)
    {
        return _repository.GetTreeProgressAsync(userId, configId);
    }

    public Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId, string configId)
    {
        return _repository.GetStoryProgressAsync(userId, configId);
    }


    public async Task<CompleteStoryResponse> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, string configId)
    {
        try
        {
            var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
            var story = await _storiesRepository.GetStoryByIdAsync(request.StoryId, locale);

            var storyCompleted = await _repository.CompleteStoryAsync(userId, request, story, configId);
            if (!storyCompleted)
            {
                return new CompleteStoryResponse
                {
                    Success = false,
                    ErrorMessage = "Story already completed or error occurred"
                };
            }

            var newlyUnlockedRegions = new List<string>();

            var effectiveTokens = new List<TokenReward>();

            if (story != null && !string.IsNullOrEmpty(request.SelectedAnswer))
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
                
                // Check for Discovery type tokens and update wallet
                var discoveryTokens = effectiveTokens.Where(t => t.Type == TokenFamily.Discovery).ToList();
                if (discoveryTokens.Count > 0)
                {
                    var totalDiscoveryCredits = discoveryTokens.Sum(t => t.Quantity);
                    await UpdateDiscoveryCreditsAsync(userId, totalDiscoveryCredits, request.StoryId);
                }
            }

            var unlockedRegions = await CheckAndUnlockRegionsAsync(userId, request.StoryId, configId);
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
     
    private async Task<List<string>> CheckAndUnlockRegionsAsync(Guid userId, string storyId, string configId)
    {
        var newlyUnlockedRegions = new List<string>();

        switch (storyId)
        {
            case "root-s1":
                await _repository.UnlockRegionAsync(userId, "farm", configId);
                newlyUnlockedRegions.Add("farm");
                break;

            case "farm-s1":
            case "farm-s2":
            case "farm-s3":
                var farmStories = await _repository.GetStoryProgressAsync(userId, configId);
                var farmCompleted = farmStories.Count(s => s.StoryId.StartsWith("farm-")) >= 3;

                if (farmCompleted)
                {
                    var tokens = await _treeOfHeroesRepository.GetUserTokensAsync(userId);
                    if (tokens.Courage >= 2)
                    {
                        await _repository.UnlockRegionAsync(userId, "sahara", configId);
                        newlyUnlockedRegions.Add("sahara");
                    }
                    if (tokens.Curiosity + tokens.Creativity >= 2)
                    {
                        await _repository.UnlockRegionAsync(userId, "dreamland", configId);
                        newlyUnlockedRegions.Add("dreamland");
                    }
                }
                break;
        }

        return newlyUnlockedRegions;
    }

    private async Task UpdateDiscoveryCreditsAsync(Guid userId, int discoveryCredits, string storyId)
    {
        // Get or create user's wallet
        var wallet = await _dbContext.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
            // Create wallet if it doesn't exist
            wallet = new CreditWallet
            {
                UserId = userId,
                Balance = 0,
                DiscoveryBalance = discoveryCredits,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.CreditWallets.Add(wallet);
        }
        else
        {
            // Add discovery credits to existing wallet
            wallet.DiscoveryBalance += discoveryCredits;
            wallet.UpdatedAt = DateTime.UtcNow;
        }
        
        // Record the credit grant transaction
        var transaction = new CreditTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = discoveryCredits,
            Type = CreditTransactionType.Grant,
            Reference = $"Story Completion Reward - {storyId}",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.CreditTransactions.Add(transaction);
        
        await _dbContext.SaveChangesAsync();
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