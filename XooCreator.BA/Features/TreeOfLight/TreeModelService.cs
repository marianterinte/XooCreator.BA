using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeModelService
{
    Task<TreeStateDto> GetTreeStateAsync(Guid userId);
    Task InitializeTreeModelAsync();
}

public class TreeModelService : ITreeModelService
{
    private readonly ITreeModelRepository _treeModelRepository;
    private readonly ITreeOfLightRepository _tolRepository;

    public TreeModelService(ITreeModelRepository treeModelRepository, ITreeOfLightRepository tolRepository)
    {
        _treeModelRepository = treeModelRepository;
        _tolRepository = tolRepository;
    }

    public async Task<TreeStateDto> GetTreeStateAsync(Guid userId)
    {
        // Get tree model data
        var regions = await _treeModelRepository.GetAllRegionsAsync();
        var storyNodes = await _treeModelRepository.GetAllStoryNodesAsync();
        var unlockRules = await _treeModelRepository.GetAllUnlockRulesAsync();

        // Get user progress data
        var completedStories = await _tolRepository.GetStoryProgressAsync(userId);
        var userTokens = await _tolRepository.GetUserTokensAsync(userId);
        
        // Calculate unlocked regions using rule evaluator
        var unlockedRegions = EvaluateUnlockedRegions(completedStories, unlockRules);

        return new TreeStateDto
        {
            Model = new TreeModelDto
            {
                Regions = regions.Select(r => new TreeRegionDto
                {
                    Id = r.Id,
                    Label = r.Label,
                    ImageUrl = r.ImageUrl,
                    PufpufMessage = r.PufpufMessage,
                    SortOrder = r.SortOrder
                }).ToList(),
                
                Stories = storyNodes.Select(sn => new TreeStoryDto
                {
                    Id = sn.StoryId,
                    Label = sn.StoryDefinition?.Title ?? sn.StoryId,
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
        // Seed model uses 'gateway' as the initial/root region, so unlock it by default
        var unlockedRegions = new HashSet<string> { "gateway" };
        
        bool changed;
        do 
        {
            changed = false;
            
            foreach (var rule in unlockRules)
            {
                if (unlockedRegions.Contains(rule.ToRegionId))
                    continue; // Already unlocked
                
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
}
