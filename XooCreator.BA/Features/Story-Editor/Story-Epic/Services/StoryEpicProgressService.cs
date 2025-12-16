using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryEpicProgressService : IStoryEpicProgressService
{
    private readonly IStoryEpicService _epicService;
    private readonly IEpicProgressRepository _progressRepository;
    private readonly IEpicHeroRepository _heroRepository;
    private readonly XooDbContext _context;

    public StoryEpicProgressService(
        IStoryEpicService epicService,
        IEpicProgressRepository progressRepository,
        IEpicHeroRepository heroRepository,
        XooDbContext context)
    {
        _epicService = epicService;
        _progressRepository = progressRepository;
        _heroRepository = heroRepository;
        _context = context;
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

        // Evaluate unlocked heroes based on completed stories
        // This includes heroes from StoryEpicHeroReferences AND heroes unlocked by completed stories (from StoryDefinitionUnlockedHeroes)
        var completedStoryIds = storyProgress.Select(sp => sp.StoryId).ToList();
        var unlockedHeroes = await EvaluateUnlockedHeroesAsync(
            epicState.Epic.Heroes,
            completedStoryIds,
            ct
        );

        // Also get heroes unlocked by completed stories themselves
        var storyUnlockedHeroes = await GetUnlockedHeroesFromStoriesAsync(completedStoryIds, ct);
        
        // Merge heroes from epic references and story definitions, avoiding duplicates
        var allUnlockedHeroIds = new HashSet<string>(unlockedHeroes.Select(h => h.HeroId));
        foreach (var storyHero in storyUnlockedHeroes)
        {
            if (!allUnlockedHeroIds.Contains(storyHero.HeroId))
            {
                unlockedHeroes.Add(storyHero);
                allUnlockedHeroIds.Add(storyHero.HeroId);
            }
        }

        // Build progress state
        var progressState = new EpicProgressStateDto
        {
            CompletedStories = storyProgress.Select(sp => new EpicCompletedStoryDto
            {
                StoryId = sp.StoryId,
                SelectedAnswer = sp.SelectedAnswer,
                CompletedAt = sp.CompletedAt
            }).ToList(),
            UnlockedRegions = unlockedRegions,
            UnlockedHeroes = unlockedHeroes
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
            return new CompleteEpicStoryResult { Success = false, NewlyUnlockedRegions = new List<string>(), NewlyUnlockedHeroes = new List<UnlockedHeroDto>(), StoryCoverImageUrl = null };
        }

        // Get story cover image URL
        var storyDefinition = await _context.StoryDefinitions
            .FirstOrDefaultAsync(sd => sd.StoryId == storyId && sd.IsActive, ct);
        var storyCoverImageUrl = storyDefinition?.CoverImageUrl;

        // Get epic state to evaluate new unlocked regions and heroes
        var epicState = await _epicService.GetEpicStateAsync(epicId, ct);
        if (epicState == null)
        {
            return new CompleteEpicStoryResult { Success = false, NewlyUnlockedRegions = new List<string>(), NewlyUnlockedHeroes = new List<UnlockedHeroDto>(), StoryCoverImageUrl = storyCoverImageUrl };
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

        // Evaluate and unlock heroes based on completed story
        // This includes heroes from StoryEpicHeroReferences AND heroes unlocked by the story itself (from StoryDefinitionUnlockedHeroes)
        var newlyUnlockedHeroes = await EvaluateAndUnlockHeroesAsync(userId, epicId, storyId, epicState.Epic.Heroes, storyProgress.Select(sp => sp.StoryId).ToList(), ct);

        return new CompleteEpicStoryResult
        {
            Success = true,
            NewlyUnlockedRegions = newlyUnlockedRegions,
            NewlyUnlockedHeroes = newlyUnlockedHeroes,
            StoryCoverImageUrl = storyCoverImageUrl
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

    public async Task<ResetEpicProgressResult> ResetProgressAsync(string epicId, Guid userId, CancellationToken ct = default)
    {
        try
        {
            var success = await _progressRepository.ResetProgressAsync(userId, epicId);
            if (!success)
            {
                return new ResetEpicProgressResult
                {
                    Success = false,
                    ErrorMessage = "Failed to reset progress"
                };
            }

            return new ResetEpicProgressResult
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ResetEpicProgressResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Gets hero image URL from EpicHeroes (epic heroes created in Story Editor)
    /// </summary>
    private async Task<string> GetHeroImageUrlAsync(string heroId, CancellationToken ct = default)
    {
        var epicHero = await _heroRepository.GetAsync(heroId, ct);
        return epicHero?.ImageUrl ?? string.Empty;
    }

    private async Task<List<UnlockedHeroDto>> EvaluateUnlockedHeroesAsync(
        List<StoryEpicHeroReferenceDto> heroReferences,
        List<string> completedStoryIds,
        CancellationToken ct = default)
    {
        var completedStoryIdsSet = new HashSet<string>(completedStoryIds);
        var unlockedHeroes = new List<UnlockedHeroDto>();

        foreach (var heroRef in heroReferences)
        {
            // Hero is unlocked if:
            // 1. It has no StoryId (available from start), OR
            // 2. The story that unlocks it is completed
            bool isUnlocked = heroRef.StoryId == null || completedStoryIdsSet.Contains(heroRef.StoryId);

            if (isUnlocked)
            {
                // Get hero image URL from EpicHero
                var imageUrl = heroRef.HeroImageUrl ?? await GetHeroImageUrlAsync(heroRef.HeroId, ct);

                unlockedHeroes.Add(new UnlockedHeroDto
                {
                    HeroId = heroRef.HeroId,
                    ImageUrl = imageUrl
                });
            }
        }

        return unlockedHeroes;
    }

    private async Task<List<UnlockedHeroDto>> EvaluateAndUnlockHeroesAsync(
        Guid userId,
        string epicId,
        string completedStoryId,
        List<StoryEpicHeroReferenceDto> heroReferences,
        List<string> allCompletedStoryIds,
        CancellationToken ct = default)
    {
        var completedStoryIdsSet = new HashSet<string>(allCompletedStoryIds);
        var newlyUnlockedHeroes = new List<UnlockedHeroDto>();

        // Get previously unlocked heroes (before this story completion)
        var previousCompletedStories = allCompletedStoryIds.Where(id => id != completedStoryId).ToList();
        var previousUnlockedHeroes = await EvaluateUnlockedHeroesAsync(heroReferences, previousCompletedStories, ct);
        var previousUnlockedHeroIds = new HashSet<string>(previousUnlockedHeroes.Select(h => h.HeroId));

        // Check which heroes are unlocked by the completed story from StoryEpicHeroReferences
        foreach (var heroRef in heroReferences)
        {
            // Hero is unlocked by this story if StoryId matches
            if (heroRef.StoryId == completedStoryId)
            {
                // Check if hero was already unlocked
                if (!previousUnlockedHeroIds.Contains(heroRef.HeroId))
                {
                    // Get hero image URL from EpicHero
                    var imageUrl = heroRef.HeroImageUrl ?? await GetHeroImageUrlAsync(heroRef.HeroId, ct);

                    newlyUnlockedHeroes.Add(new UnlockedHeroDto
                    {
                        HeroId = heroRef.HeroId,
                        ImageUrl = imageUrl
                    });
                }
            }
        }

        // Also check heroes unlocked by the story itself (from StoryDefinitionUnlockedHeroes)
        var storyDefinition = await _context.StoryDefinitions
            .Include(sd => sd.UnlockedHeroes)
            .FirstOrDefaultAsync(sd => sd.StoryId == completedStoryId && sd.IsActive, ct);

        if (storyDefinition != null && storyDefinition.UnlockedHeroes != null && storyDefinition.UnlockedHeroes.Count > 0)
        {
            foreach (var unlockedHero in storyDefinition.UnlockedHeroes)
            {
                // Check if hero was already unlocked
                if (!previousUnlockedHeroIds.Contains(unlockedHero.HeroId))
                {
                    // Get hero image URL from EpicHero
                    var imageUrl = await GetHeroImageUrlAsync(unlockedHero.HeroId, ct);

                    newlyUnlockedHeroes.Add(new UnlockedHeroDto
                    {
                        HeroId = unlockedHero.HeroId,
                        ImageUrl = imageUrl
                    });
                }
            }
        }

        return newlyUnlockedHeroes;
    }

    /// <summary>
    /// Gets heroes unlocked by completed stories (from StoryDefinitionUnlockedHeroes)
    /// </summary>
    private async Task<List<UnlockedHeroDto>> GetUnlockedHeroesFromStoriesAsync(List<string> completedStoryIds, CancellationToken ct = default)
    {
        var unlockedHeroes = new List<UnlockedHeroDto>();

        if (completedStoryIds == null || completedStoryIds.Count == 0)
        {
            return unlockedHeroes;
        }

        // Get all StoryDefinitions for completed stories with their unlocked heroes
        var storyDefinitions = await _context.StoryDefinitions
            .Include(sd => sd.UnlockedHeroes)
            .Where(sd => completedStoryIds.Contains(sd.StoryId) && sd.IsActive)
            .ToListAsync(ct);

        foreach (var storyDefinition in storyDefinitions)
        {
            if (storyDefinition.UnlockedHeroes != null && storyDefinition.UnlockedHeroes.Count > 0)
            {
                foreach (var unlockedHero in storyDefinition.UnlockedHeroes)
                {
                    // Get hero image URL from EpicHero
                    var imageUrl = await GetHeroImageUrlAsync(unlockedHero.HeroId, ct);

                    unlockedHeroes.Add(new UnlockedHeroDto
                    {
                        HeroId = unlockedHero.HeroId,
                        ImageUrl = imageUrl
                    });
                }
            }
        }

        return unlockedHeroes;
    }
}

