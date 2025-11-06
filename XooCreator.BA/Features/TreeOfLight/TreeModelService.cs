using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.SeedData.DTOs;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;
using XooCreator.BA.Features.TreeOfLight.DTOs;
using XooCreator.BA.Features.TreeOfLight.Repositories;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight.Services;

public interface ITreeModelService
{
    Task<TreeStateDto> GetTreeStateAsync(Guid userId, string? configId = null);
    Task InitializeTreeModelAsync();
}

public class TreeModelService : ITreeModelService
{
    private readonly ITreeModelRepository _treeModelRepository;
    private readonly ITreeOfLightRepository _tolRepository;
    private readonly ITreeOfLightTranslationService _translationService;
    private readonly IUserContextService _userContext;
    private readonly XooDbContext _dbContext;

    public TreeModelService(ITreeModelRepository treeModelRepository, ITreeOfLightRepository tolRepository, ITreeOfLightTranslationService translationService, IUserContextService userContext, XooDbContext dbContext)
    {
        _treeModelRepository = treeModelRepository;
        _tolRepository = tolRepository;
        _translationService = translationService;
        _userContext = userContext;
        _dbContext = dbContext;
    }

    public async Task<TreeStateDto> GetTreeStateAsync(Guid userId, string? configId = null)
    {
        var allConfigs = await _treeModelRepository.GetAllConfigurationsAsync();
        var config = string.IsNullOrEmpty(configId) 
            ? allConfigs.FirstOrDefault(c => c.IsDefault) ?? allConfigs.First() 
            : allConfigs.FirstOrDefault(c => c.Id == configId);

        if (config == null)
        {
            throw new Exception("Tree configuration not found.");
        }

        var regions = await _treeModelRepository.GetAllRegionsAsync(config.Id);
        var storyNodes = await _treeModelRepository.GetAllStoryNodesAsync(config.Id);
        var unlockRules = await _treeModelRepository.GetAllUnlockRulesAsync(config.Id);

        var locale = _userContext.GetRequestLocaleOrDefault("ro-ro");
        var regionTranslations = await _translationService.GetTranslationsAsync(locale);

        var completedStories = await _tolRepository.GetStoryProgressAsync(userId, config.Id);
        var userTokens = new UserTokensDto { Courage = 0, Curiosity = 0, Thinking = 0, Creativity = 0, Safety = 0 };
        
        var unlockedRegions = EvaluateUnlockedRegions(completedStories, unlockRules);
        var unlockedHeroes = await EvaluateUnlockedHeroesAsync(userId, completedStories);

        return new TreeStateDto
        {
            Configurations = allConfigs.Select(c => new TreeConfigurationDto { Id = c.Id, Name = c.Name, IsDefault = c.IsDefault }).ToList(),
            Configuration = new TreeConfigurationDto { Id = config.Id, Name = config.Name, IsDefault = config.IsDefault },
            Model = new TreeModelDto
            {
                Regions = regions.Select(r => new TreeRegionDto
                {
                    Id = r.Id,
                    Label = regionTranslations.GetValueOrDefault($"region_{r.Id}_label", r.Id),
                    ImageUrl = r.ImageUrl,
                    SortOrder = r.SortOrder,
                    IsLocked = r.IsLocked
                }).ToList(),
                
                Stories = storyNodes.Select(sn => new TreeStoryDto
                {
                    Id = sn.StoryId,
                    Label = sn.StoryDefinition?.Translations
                        ?.FirstOrDefault(t => t.LanguageCode == _userContext.GetRequestLocaleOrDefault("ro-ro"))
                        ?.Title ?? sn.StoryDefinition?.Title ?? sn.StoryId,
                    RegionId = sn.RegionId,
                    RewardImageUrl = sn.RewardImageUrl,
                    SortOrder = sn.SortOrder
                }).ToList(),
                
                Rules = unlockRules.Select(ur => new TreeUnlockRuleDto
                {
                    Type = ur.Type,
                    FromId = ur.FromId,
                    ToRegionId = ur.ToRegionId,
                    RequiredStories = ur.RequiredStoriesCsv?.Split(',').ToList() ?? new List<string>(),
                    MinCount = ur.MinCount,
                    StoryId = ur.StoryId
                }).ToList()
            },
            
            Progress = new TreeProgressStateDto
            {
                CompletedStories = completedStories.Select(cs => new CompletedStoryDto
                {
                    StoryId = cs.StoryId,
                    SelectedAnswer = cs.SelectedAnswer,
                    Tokens = cs.Tokens,
                    CompletedAt = cs.CompletedAt
                }).ToList(),
                
                UnlockedRegions = unlockedRegions,
                UnlockedHeroes = unlockedHeroes,
                UserTokens = userTokens
            }
        };
    }

    public async Task InitializeTreeModelAsync()
    {
        await _treeModelRepository.SeedTreeModelAsync();
    }

    private List<string> EvaluateUnlockedRegions(List<StoryProgressDto> completedStories, List<TreeUnlockRule> unlockRules)
    {
        var completedStoryIds = new HashSet<string>(completedStories.Select(cs => cs.StoryId));
        var unlockedRegions = new HashSet<string> { "gateway" };
        
        bool changed;
        do 
        {
            changed = false;
            
            foreach (var rule in unlockRules)
            {
                if (unlockedRegions.Contains(rule.ToRegionId))
                    continue;
                
                bool shouldUnlock = rule.Type switch
                {
                    "story" => completedStoryIds.Contains(rule.StoryId ?? rule.FromId),
                    "all" => rule.RequiredStoriesCsv?.Split(',')
                        .All(storyId => completedStoryIds.Contains(storyId.Trim())) == true,
                    "any" => rule.RequiredStoriesCsv?.Split(',')
                        .Count(storyId => completedStoryIds.Contains(storyId.Trim())) >= (rule.MinCount ?? 1),
                    _ => false
                };
                
                if (shouldUnlock)
                {
                    unlockedRegions.Add(rule.ToRegionId);
                    changed = true;
                }
            }
        } 
        while (changed);
        
        return unlockedRegions.ToList();
    }

    /// <summary>
    /// Evaluates which heroes should be unlocked based on completed stories
    /// Heroes are now independent - stories decide which heroes unlock via unlockedStoryHeroes array
    /// This method checks all completed stories and unlocks heroes from their unlockedStoryHeroes arrays
    /// </summary>
    private async Task<List<UnlockedHeroDto>> EvaluateUnlockedHeroesAsync(Guid userId, List<StoryProgressDto> completedStories)
    {
        var unlockedHeroes = new List<UnlockedHeroDto>();
        var unlockedHeroIds = new HashSet<string>();

        // For each completed story, check its unlockedStoryHeroes array
        foreach (var completedStory in completedStories)
        {
            var storyUnlockedHeroes = await GetUnlockedHeroesFromStoryJsonAsync(completedStory.StoryId);
            foreach (var heroId in storyUnlockedHeroes)
            {
                if (!unlockedHeroIds.Contains(heroId))
                {
                    unlockedHeroIds.Add(heroId);
                    
                    var isAlreadyUnlocked = await _tolRepository.IsHeroUnlockedAsync(userId, heroId);
                    if (!isAlreadyUnlocked)
                    {
                        await _tolRepository.UnlockHeroAsync(userId, heroId, "STORY_COMPLETION");
                        // Save to bestiary when unlocking
                        await SaveStoryHeroToBestiaryAsync(userId, heroId);
                    }

                    // Get hero image URL
                    var storyHeroes = await _tolRepository.GetStoryHeroesAsync();
                    var storyHero = storyHeroes.FirstOrDefault(sh => sh.HeroId == heroId);
                    
                    unlockedHeroes.Add(new UnlockedHeroDto
                    {
                        HeroId = heroId,
                        ImageUrl = storyHero?.ImageUrl ?? string.Empty
                    });
                }
            }
        }

        // Also include already unlocked heroes
        var allStoryHeroes = await _tolRepository.GetStoryHeroesAsync();
        foreach (var storyHero in allStoryHeroes)
        {
            var isAlreadyUnlocked = await _tolRepository.IsHeroUnlockedAsync(userId, storyHero.HeroId);
            if (isAlreadyUnlocked && !unlockedHeroIds.Contains(storyHero.HeroId))
            {
                unlockedHeroes.Add(new UnlockedHeroDto
                {
                    HeroId = storyHero.HeroId,
                    ImageUrl = storyHero.ImageUrl
                });
            }
        }

        return unlockedHeroes;
    }

    private async Task<List<string>> GetUnlockedHeroesFromStoryJsonAsync(string storyId)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "i18n", "en-us", $"{storyId}.json"),
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "i18n", "ro-ro", $"{storyId}.json"),
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "i18n", "hu-hu", $"{storyId}.json")
            };

            foreach (var filePath in candidates)
            {
                if (File.Exists(filePath))
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    var storyData = JsonSerializer.Deserialize<StoryJsonData>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                    return storyData?.UnlockedStoryHeroes ?? new List<string>();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading story JSON for {storyId}: {ex.Message}");
        }

        return new List<string>();
    }

    /// <summary>
    /// Saves a story hero to the bestiary when unlocked
    /// Same logic as TreeOfLightService.SaveStoryHeroToBestiaryAsync
    /// </summary>
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
}

// Note: StoryJsonData is defined in TreeOfLightService.cs (same namespace)