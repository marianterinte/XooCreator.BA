using XooCreator.BA.Features.Stories;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;
using XooCreator.BA.Features.TreeOfLight.DTOs;
using XooCreator.BA.Features.TreeOfLight.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Data;
using XooCreator.BA.Data.SeedData.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Features.Stories.Repositories;

namespace XooCreator.BA.Features.TreeOfLight.Services;

public interface ITreeOfLightService
{
    Task<List<TreeConfigurationDto>> GetAllConfigurationsAsync();
    Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId, string configId);
    Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId, string configId);
    Task<CompleteStoryResponse> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, string configId);
    Task<ResetProgressResponse> ResetUserProgressAsync(Guid userId);
    
    Task<HeroMessageDto?> GetHeroMessageAsync(string heroId, string regionId);
    Task<HeroClickMessageDto?> GetHeroClickMessageAsync(string heroId);
}

public class TreeOfLightService : ITreeOfLightService
{
    private readonly ITreeOfLightRepository _repository;
    private readonly IStoriesRepository _storiesRepository;
    private readonly ITreeOfHeroesRepository _treeOfHeroesRepository;
    private readonly IUserContextService _userContext;
    private readonly XooDbContext _dbContext;
    private readonly ITreeOfLightTranslationService _translationService;

    public TreeOfLightService(ITreeOfLightRepository repository, IStoriesRepository storiesRepository, ITreeOfHeroesRepository treeOfHeroesRepository, IUserContextService userContext, XooDbContext dbContext, ITreeOfLightTranslationService translationService)
    {
        _repository = repository;
        _storiesRepository = storiesRepository;
        _treeOfHeroesRepository = treeOfHeroesRepository;
        _userContext = userContext;
        _dbContext = dbContext;
        _translationService = translationService;
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
            var newlyUnlockedHeroes = new List<UnlockedHeroDto>();

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
                
                var discoveryTokens = effectiveTokens.Where(t => t.Type == TokenFamily.Discovery).ToList();
                if (discoveryTokens.Count > 0)
                {
                    var totalDiscoveryCredits = discoveryTokens.Sum(t => t.Quantity);
                    await UpdateDiscoveryCreditsAsync(userId, totalDiscoveryCredits, request.StoryId);
                }
            }

            var unlockedRegions = await CheckAndUnlockRegionsAsync(userId, request.StoryId, configId);
            newlyUnlockedRegions.AddRange(unlockedRegions);

            var unlockedHeroes = await CheckAndUnlockHeroesAsync(userId, request.StoryId);
            newlyUnlockedHeroes.AddRange(unlockedHeroes);

            var updatedTokens = await _treeOfHeroesRepository.GetUserTokensAsync(userId);

            return new CompleteStoryResponse
            {
                Success = true,
                NewlyUnlockedRegions = newlyUnlockedRegions,
                NewlyUnlockedHeroes = newlyUnlockedHeroes,
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

    private async Task<List<UnlockedHeroDto>> CheckAndUnlockHeroesAsync(Guid userId, string storyId)
    {
        var newlyUnlockedHeroes = new List<UnlockedHeroDto>();

        var storyUnlockedHeroes = await GetUnlockedHeroesFromStoryDefinitionAsync(storyId);
        foreach (var heroId in storyUnlockedHeroes)
        {
            var isAlreadyUnlocked = await _repository.IsHeroUnlockedAsync(userId, heroId);
            if (isAlreadyUnlocked)
            {
                continue;
            }

            var unlocked = await _repository.UnlockHeroAsync(userId, heroId, "STORY_COMPLETION");
            if (unlocked)
            {
                await SaveStoryHeroToBestiaryAsync(userId, heroId);
                
                var heroImageUrl = await GetStoryHeroImageUrlAsync(heroId);
                
                newlyUnlockedHeroes.Add(new UnlockedHeroDto
                {
                    HeroId = heroId,
                    ImageUrl = heroImageUrl
                });
            }
        }

        var storyHeroes = await _repository.GetStoryHeroesAsync();
        
        foreach (var storyHero in storyHeroes)
        {
            var isAlreadyUnlocked = await _repository.IsHeroUnlockedAsync(userId, storyHero.HeroId);
            if (isAlreadyUnlocked)
            {
                continue;
            }

            var unlockConditions = JsonSerializer.Deserialize<UnlockConditions>(storyHero.UnlockConditionsJson);
            if (unlockConditions?.Type == "story_completion" && unlockConditions.RequiredStories != null)
            {
                if (unlockConditions.RequiredStories.Contains(storyId))
                {
                    var completedStories = await _repository.GetStoryProgressAsync(userId, "puf-puf-journey-v1"); // Use default config
                    var completedStoryIds = new HashSet<string>(completedStories.Select(cs => cs.StoryId));
                    
                    var allRequiredStoriesCompleted = unlockConditions.RequiredStories.All(requiredStoryId => completedStoryIds.Contains(requiredStoryId));
                    if (allRequiredStoriesCompleted)
                    {
                        var unlocked = await _repository.UnlockHeroAsync(userId, storyHero.HeroId, "STORY_COMPLETION");
                        if (unlocked)
                        {
                            await SaveStoryHeroToBestiaryAsync(userId, storyHero.HeroId);
                            
                            newlyUnlockedHeroes.Add(new UnlockedHeroDto
                            {
                                HeroId = storyHero.HeroId,
                                ImageUrl = storyHero.ImageUrl
                            });
                        }
                    }
                }
            }
        }

        return newlyUnlockedHeroes;
    }

    private async Task<List<string>> GetUnlockedHeroesFromStoryDefinitionAsync(string storyId)
    {
        var definition = await _dbContext.StoryDefinitions
            .Include(d => d.UnlockedHeroes)
            .FirstOrDefaultAsync(d => d.StoryId == storyId);

        return definition?.UnlockedHeroes.Select(h => h.HeroId).ToList() ?? new List<string>();
    }

    private async Task SaveStoryHeroToBestiaryAsync(Guid userId, string heroId)
    {
        try
        {
            var heroNameKey = $"story_hero_{heroId}_name";
            var heroStoryKey = $"story_hero_{heroId}_story";

            var existingBestiaryItem = await _dbContext.BestiaryItems
                .FirstOrDefaultAsync(bi => bi.ArmsKey == heroId);

            BestiaryItem bestiaryItem;
            if (existingBestiaryItem == null)
            {
                bestiaryItem = new BestiaryItem
                {
                    Id = Guid.NewGuid(),
                    ArmsKey = heroId, // Use HeroId as the identifier
                    BodyKey = "—", 
                    HeadKey = "—",
                    Name = heroNameKey, // Store translation key instead of translated text
                    Story = heroStoryKey, // Store translation key instead of translated text
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.BestiaryItems.Add(bestiaryItem);
            }
            else
            {
                bestiaryItem = existingBestiaryItem;
            }

            var existingUserBestiary = await _dbContext.UserBestiary
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BestiaryItemId == bestiaryItem.Id && ub.BestiaryType == "storyhero");

            if (existingUserBestiary == null)
            {
                var userBestiary = new UserBestiary
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BestiaryItemId = bestiaryItem.Id,
                    BestiaryType = "storyhero",
                    DiscoveredAt = DateTime.UtcNow
                };
                _dbContext.UserBestiary.Add(userBestiary);
            }

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save story hero {heroId} to bestiary: {ex.Message}");
        }
    }

    private async Task<string> GetStoryHeroImageUrlAsync(string heroId)
    {
        var hero = await _dbContext.StoryHeroes.FirstOrDefaultAsync(h => h.HeroId == heroId);
        return hero?.ImageUrl ?? $"images/tol/stories/seed@alchimalia.com/heroes/{heroId}.png";
    }

    private async Task UpdateDiscoveryCreditsAsync(Guid userId, double discoveryCredits, string storyId)
    {
        var wallet = await _dbContext.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
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
            wallet.DiscoveryBalance += discoveryCredits;
            wallet.UpdatedAt = DateTime.UtcNow;
        }
        
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

    public async Task<HeroMessageDto?> GetHeroMessageAsync(string heroId, string regionId)
    {
        var heroMessage = await _repository.GetHeroMessageAsync(heroId, regionId);
        if (heroMessage == null) return null;

        var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
        var translations = await _translationService.GetTranslationsAsync(locale);
        
        var message = translations.GetValueOrDefault(heroMessage.MessageKey, string.Empty);
        if (string.IsNullOrEmpty(message)) return null;

        return new HeroMessageDto
        {
            HeroId = heroMessage.HeroId,
            RegionId = heroMessage.RegionId,
            Message = message,
            AudioUrl = heroMessage.AudioUrl
        };
    }

    public async Task<HeroClickMessageDto?> GetHeroClickMessageAsync(string heroId)
    {
        var heroClickMessage = await _repository.GetHeroClickMessageAsync(heroId);
        if (heroClickMessage == null) return null;

        var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
        var translations = await _translationService.GetTranslationsAsync(locale);
        
        var message = translations.GetValueOrDefault(heroClickMessage.MessageKey, string.Empty);
        if (string.IsNullOrEmpty(message)) return null;

        return new HeroClickMessageDto
        {
            HeroId = heroClickMessage.HeroId,
            Message = message,
            AudioUrl = heroClickMessage.AudioUrl
        };
    }
}

// Helper classes for JSON deserialization