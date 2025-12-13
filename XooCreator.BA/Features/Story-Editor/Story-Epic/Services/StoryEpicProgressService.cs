using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryEpicProgressService : IStoryEpicProgressService
{
    private readonly IStoryEpicService _epicService;
    private readonly IEpicProgressRepository _progressRepository;

    public StoryEpicProgressService(
        IStoryEpicService epicService,
        IEpicProgressRepository progressRepository)
    {
        _epicService = epicService;
        _progressRepository = progressRepository;
    }

    public async Task<StoryEpicStateWithProgressDto?> GetEpicStateWithProgressAsync(string epicId, Guid userId, CancellationToken ct = default)
    {
        // Get epic state (without progress)
        var epicState = await _epicService.GetEpicStateAsync(epicId, ct);
        if (epicState == null) return null;

        // Get user progress
        var epicProgress = await _progressRepository.GetEpicProgressAsync(userId, epicId);
        var storyProgress = await _progressRepository.GetEpicStoryProgressAsync(userId, epicId);

        // Evaluate unlocked regions based on completed stories and unlock rules
        var unlockedRegions = EvaluateUnlockedRegions(
            storyProgress.Select(sp => sp.StoryId).ToList(),
            epicState.Epic.Rules,
            epicState.Epic.Regions
        );

        // Build progress state
        var progressState = new EpicProgressStateDto
        {
            CompletedStories = storyProgress.Select(sp => new EpicCompletedStoryDto
            {
                StoryId = sp.StoryId,
                SelectedAnswer = sp.SelectedAnswer,
                CompletedAt = sp.CompletedAt
            }).ToList(),
            UnlockedRegions = unlockedRegions
        };

        return new StoryEpicStateWithProgressDto
        {
            Epic = epicState.Epic,
            Preview = epicState.Preview,
            Progress = progressState
        };
    }

    public async Task<CompleteEpicStoryResult> CompleteStoryAsync(string epicId, Guid userId, string storyId, string? selectedAnswer = null, CancellationToken ct = default)
    {
        // Get current unlocked regions BEFORE completing the story
        var currentProgress = await _progressRepository.GetEpicProgressAsync(userId, epicId);
        var currentUnlockedRegions = new HashSet<string>(
            currentProgress.Where(p => p.IsUnlocked).Select(p => p.RegionId)
        );

        // Complete the story
        var completed = await _progressRepository.CompleteStoryAsync(userId, epicId, storyId, selectedAnswer);
        
        if (!completed)
        {
            return new CompleteEpicStoryResult { Success = false, NewlyUnlockedRegions = new List<string>() };
        }

        // Get epic state to evaluate new unlocked regions
        var epicState = await _epicService.GetEpicStateAsync(epicId, ct);
        if (epicState == null)
        {
            return new CompleteEpicStoryResult { Success = false, NewlyUnlockedRegions = new List<string>() };
        }

        // Get updated story progress
        var storyProgress = await _progressRepository.GetEpicStoryProgressAsync(userId, epicId);

        // Evaluate and unlock new regions
        var unlockedRegions = EvaluateUnlockedRegions(
            storyProgress.Select(sp => sp.StoryId).ToList(),
            epicState.Epic.Rules,
            epicState.Epic.Regions
        );

        // Find newly unlocked regions (regions that are now unlocked but weren't before)
        var newlyUnlockedRegions = new List<string>();
        foreach (var regionId in unlockedRegions)
        {
            if (!currentUnlockedRegions.Contains(regionId))
            {
                newlyUnlockedRegions.Add(regionId);
                // Unlock the region in the database
                await _progressRepository.UnlockRegionAsync(userId, epicId, regionId);
            }
        }

        return new CompleteEpicStoryResult
        {
            Success = true,
            NewlyUnlockedRegions = newlyUnlockedRegions
        };
    }

    private List<string> EvaluateUnlockedRegions(
        List<string> completedStoryIds,
        List<StoryEpicUnlockRuleDto> unlockRules,
        List<StoryEpicRegionDto> regions)
    {
        var completedStoryIdsSet = new HashSet<string>(completedStoryIds);
        
        // Find startup region (always unlocked)
        var startupRegion = regions.FirstOrDefault(r => r.IsStartupRegion);
        var unlockedRegions = new HashSet<string>();
        
        if (startupRegion != null)
        {
            unlockedRegions.Add(startupRegion.Id);
        }
        else if (regions.Count > 0)
        {
            // If no startup region, unlock first region by default
            unlockedRegions.Add(regions.OrderBy(r => r.SortOrder).First().Id);
        }

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
                    "story" => completedStoryIdsSet.Contains(rule.StoryId ?? rule.FromId),
                    "all" => rule.RequiredStories != null && 
                             rule.RequiredStories.All(storyId => completedStoryIdsSet.Contains(storyId)),
                    "any" => rule.RequiredStories != null && 
                             rule.RequiredStories.Count(storyId => completedStoryIdsSet.Contains(storyId)) >= (rule.MinCount ?? 1),
                    _ => false
                };

                // Also check if the "from" region is unlocked (for region-to-region rules)
                if (!shouldUnlock && unlockedRegions.Contains(rule.FromId))
                {
                    // This is a region-to-region unlock, check if all required stories in the from region are completed
                    if (rule.Type == "all" && rule.RequiredStories != null)
                    {
                        shouldUnlock = rule.RequiredStories.All(storyId => completedStoryIdsSet.Contains(storyId));
                    }
                    else if (rule.Type == "any" && rule.RequiredStories != null)
                    {
                        shouldUnlock = rule.RequiredStories.Count(storyId => completedStoryIdsSet.Contains(storyId)) >= (rule.MinCount ?? 1);
                    }
                }

                if (shouldUnlock)
                {
                    unlockedRegions.Add(rule.ToRegionId);
                    changed = true;
                }
            }
        } while (changed);

        return unlockedRegions.ToList();
    }
}

