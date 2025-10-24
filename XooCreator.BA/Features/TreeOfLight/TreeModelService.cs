using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TreeOfHeroes;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfLight;

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

    public TreeModelService(ITreeModelRepository treeModelRepository, ITreeOfLightRepository tolRepository, ITreeOfLightTranslationService translationService, IUserContextService userContext)
    {
        _treeModelRepository = treeModelRepository;
        _tolRepository = tolRepository;
        _translationService = translationService;
        _userContext = userContext;
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

    private async Task<List<UnlockedHeroDto>> EvaluateUnlockedHeroesAsync(Guid userId, List<StoryProgressDto> completedStories)
    {
        var completedStoryIds = new HashSet<string>(completedStories.Select(cs => cs.StoryId));
        var unlockedHeroes = new List<UnlockedHeroDto>();

        var storyHeroes = await _tolRepository.GetStoryHeroesAsync();
        
        foreach (var storyHero in storyHeroes)
        {
            var isAlreadyUnlocked = await _tolRepository.IsHeroUnlockedAsync(userId, storyHero.HeroId);
            if (isAlreadyUnlocked)
            {
                unlockedHeroes.Add(new UnlockedHeroDto
                {
                    HeroId = storyHero.HeroId,
                    ImageUrl = storyHero.ImageUrl
                });
                continue;
            }

            var unlockConditions = JsonSerializer.Deserialize<UnlockConditions>(storyHero.UnlockConditionJson);
            if (unlockConditions?.Type == "story_completion" && unlockConditions.RequiredStories != null)
            {
                var allRequiredStoriesCompleted = unlockConditions.RequiredStories.All(storyId => completedStoryIds.Contains(storyId));
                if (allRequiredStoriesCompleted)
                {
                    await _tolRepository.UnlockHeroAsync(userId, storyHero.HeroId, "STORY_COMPLETION");
                    unlockedHeroes.Add(new UnlockedHeroDto
                    {
                        HeroId = storyHero.HeroId,
                        ImageUrl = storyHero.ImageUrl
                    });
                }
            }
        }

        return unlockedHeroes;
    }
}