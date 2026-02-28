using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.Bestiary.Services;
using XooCreator.BA.Features.HeroStoryRewards.DTOs;
using XooCreator.BA.Features.HeroStoryRewards.Services;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using TokenReward = XooCreator.BA.Features.TreeOfLight.DTOs.TokenReward;
using TokenRewardDto = XooCreator.BA.Features.HeroStoryRewards.DTOs.TokenRewardDto;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class StoryEpicProgressService : IStoryEpicProgressService
{
    private readonly IStoryEpicService _epicService;
    private readonly IEpicProgressRepository _progressRepository;
    private readonly IEpicHeroRepository _heroRepository;
    private readonly IHeroStoryRewardsService _heroStoryRewardsService;
    private readonly IStoryHeroBestiaryService _storyHeroBestiaryService;
    private readonly XooDbContext _context;

    public StoryEpicProgressService(
        IStoryEpicService epicService,
        IEpicProgressRepository progressRepository,
        IEpicHeroRepository heroRepository,
        IHeroStoryRewardsService heroStoryRewardsService,
        IStoryHeroBestiaryService storyHeroBestiaryService,
        XooDbContext context)
    {
        _epicService = epicService;
        _progressRepository = progressRepository;
        _heroRepository = heroRepository;
        _heroStoryRewardsService = heroStoryRewardsService;
        _storyHeroBestiaryService = storyHeroBestiaryService;
        _context = context;
    }

    public async Task<StoryEpicStateWithProgressDto?> GetEpicStateWithProgressAsync(string epicId, Guid userId, CancellationToken ct = default)
    {
        // Get PUBLISHED epic state for play mode (without progress)
        // This ensures we use the published version, not the draft
        var epicState = await _epicService.GetPublishedEpicStateAsync(epicId, ct);
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

        // Evaluate unlocked stories (story-to-story rules). This is an additional gate on top of region visibility.
        var unlockedStories = EvaluateUnlockedStories(
            storyProgress.Select(sp => sp.StoryId).ToList(),
            epicState.Epic.Rules,
            epicState.Epic.Stories
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
        var allowedEpicHeroIds = new HashSet<string>(epicState.Epic.Heroes.Select(h => h.HeroId));
        var storyUnlockedHeroes = await GetUnlockedHeroesFromStoriesAsync(completedStoryIds, allowedEpicHeroIds, ct);
        
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
            UnlockedStories = unlockedStories,
            UnlockedHeroes = unlockedHeroes
        };

        return new StoryEpicStateWithProgressDto
        {
            Epic = epicState.Epic,
            Preview = epicState.Preview,
            Progress = progressState
        };
    }

    public async Task<CompleteEpicStoryResult> CompleteStoryAsync(string epicId, Guid userId, string storyId, string? selectedAnswer = null, List<TokenReward>? tokens = null, CancellationToken ct = default)
    {
        // Check if story is already completed BEFORE attempting to complete it
        var existingStoryProgress = await _progressRepository.GetEpicStoryProgressAsync(userId, epicId);
        var isAlreadyCompleted = existingStoryProgress.Any(sp => sp.StoryId == storyId);
        
        if (isAlreadyCompleted)
        {
            // Story is already completed - return success without recalculating tokens, regions, or heroes
            var storyDefinition1 = await _context.StoryDefinitions
                .FirstOrDefaultAsync(sd => sd.StoryId == storyId && sd.IsActive, ct);
            var storyCoverImageUrl1 = storyDefinition1?.CoverImageUrl;
            
            return new CompleteEpicStoryResult 
            { 
                Success = true, 
                NewlyUnlockedRegions = new List<string>(), 
                NewlyUnlockedHeroes = new List<UnlockedHeroDto>(), 
                StoryCoverImageUrl = storyCoverImageUrl1 
            };
        }

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

        // Get PUBLISHED epic state to evaluate new unlocked regions and heroes
        // Use published version since we're in play mode completing a story
        var epicState = await _epicService.GetPublishedEpicStateAsync(epicId, ct);
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

        // Persist newly unlocked story heroes to Book of Heroes (UserBestiary with BestiaryType = storyhero)
        if (newlyUnlockedHeroes.Count > 0)
        {
            await _storyHeroBestiaryService.AddDiscoveredStoryHeroesAsync(userId, newlyUnlockedHeroes, ct);
        }

        // Award tokens via generic Hero Story Rewards pipeline (same as indie; non-blocking)
        if (tokens != null && tokens.Count > 0)
        {
            var rewardRequest = new CompleteStoryRewardRequest
            {
                StoryId = storyId,
                Source = "epic",
                EpicId = epicId,
                SelectedAnswer = selectedAnswer,
                Tokens = tokens.Select(t => new TokenRewardDto
                {
                    Type = t.Type.ToString(),
                    Value = t.Value,
                    Quantity = t.Quantity
                }).ToList()
            };
            await _heroStoryRewardsService.AwardStoryRewardsAsync(userId, rewardRequest, ct);
        }

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
                // Ignore story-targeted rules here; region unlock evaluation should only consider region-targeted rules.
                // Story-target rules use ToStoryId and in FE they may carry ToRegionId="" for backward compatibility.
                if (!string.IsNullOrWhiteSpace(rule.ToStoryId) || string.IsNullOrWhiteSpace(rule.ToRegionId))
                {
                    continue;
                }

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

    private List<string> EvaluateUnlockedStories(
        List<string> completedStoryIds,
        List<StoryEpicUnlockRuleDto> unlockRules,
        List<StoryEpicStoryNodeDto> stories)
    {
        var completed = new HashSet<string>(completedStoryIds ?? new List<string>());
        var storyIds = stories.Select(s => s.StoryId).ToHashSet();

        // Incoming rules targeting stories: ToStoryId != null/empty
        var storyTargetRules = unlockRules
            .Where(r => !string.IsNullOrWhiteSpace(r.ToStoryId))
            .Where(r => storyIds.Contains(r.ToStoryId!))
            .GroupBy(r => r.ToStoryId!)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Default: unlocked if no incoming story-targeted rules exist.
        var unlocked = new HashSet<string>();
        foreach (var storyId in storyIds)
        {
            if (!storyTargetRules.ContainsKey(storyId))
            {
                unlocked.Add(storyId);
            }
        }

        // If there are incoming rules, unlock if ANY rule is satisfied.
        foreach (var (targetStoryId, rulesForTarget) in storyTargetRules)
        {
            var satisfied = rulesForTarget.Any(rule =>
            {
                return rule.Type switch
                {
                    "story" => completed.Contains(rule.StoryId ?? rule.FromId),
                    "all" => rule.RequiredStories != null && rule.RequiredStories.Count > 0 &&
                             rule.RequiredStories.All(storyId => completed.Contains(storyId)),
                    "any" => rule.RequiredStories != null && rule.RequiredStories.Count > 0 &&
                             rule.RequiredStories.Count(storyId => completed.Contains(storyId)) >= (rule.MinCount ?? 1),
                    _ => false
                };
            });

            if (satisfied)
            {
                unlocked.Add(targetStoryId);
            }
        }

        return unlocked.ToList();
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
    /// Gets hero image URL from EpicHeroDefinitions (published) or EpicHeroCrafts (draft)
    /// </summary>
    private async Task<string> GetHeroImageUrlAsync(string heroId, CancellationToken ct = default)
    {
        // Try published definition first
        var epicHeroDefinition = await _heroRepository.GetDefinitionAsync(heroId, ct);
        if (epicHeroDefinition != null) return epicHeroDefinition.ImageUrl ?? string.Empty;
        
        // Fallback to craft (draft)
        var epicHeroCraft = await _heroRepository.GetCraftAsync(heroId, ct);
        return epicHeroCraft?.ImageUrl ?? string.Empty;
    }

    /// <summary>
    /// Batch-load hero image URLs from EpicHeroDefinitions then EpicHeroCrafts to avoid N+1.
    /// </summary>
    private async Task<Dictionary<string, string>> BatchGetHeroImageUrlsAsync(
        IEnumerable<string> heroIds, CancellationToken ct = default)
    {
        var idList = heroIds.Distinct().ToList();
        if (idList.Count == 0) return new Dictionary<string, string>();

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var definitionImages = await _context.EpicHeroDefinitions
            .AsNoTracking()
            .Where(h => idList.Contains(h.Id) && h.ImageUrl != null)
            .Select(h => new { h.Id, h.ImageUrl })
            .ToListAsync(ct);

        foreach (var h in definitionImages)
            result[h.Id] = h.ImageUrl ?? "";

        var missingIds = idList.Where(id => !result.ContainsKey(id)).ToList();
        if (missingIds.Count > 0)
        {
            var craftImages = await _context.EpicHeroCrafts
                .AsNoTracking()
                .Where(h => missingIds.Contains(h.Id) && h.ImageUrl != null)
                .Select(h => new { h.Id, h.ImageUrl })
                .ToListAsync(ct);

            foreach (var h in craftImages)
                result[h.Id] = h.ImageUrl ?? "";
        }

        return result;
    }

    private async Task<List<UnlockedHeroDto>> EvaluateUnlockedHeroesAsync(
        List<StoryEpicHeroReferenceDto> heroReferences,
        List<string> completedStoryIds,
        CancellationToken ct = default)
    {
        var completedStoryIdsSet = new HashSet<string>(completedStoryIds);
        var unlockedHeroes = new List<UnlockedHeroDto>();

        var allHeroIds = heroReferences
            .Where(h => string.IsNullOrWhiteSpace(h.HeroImageUrl))
            .Select(h => h.HeroId)
            .Distinct()
            .ToList();
        var heroImageMap = await BatchGetHeroImageUrlsAsync(allHeroIds, ct);

        foreach (var heroRef in heroReferences)
        {
            // Hero is unlocked if:
            // 1. It has no StoryId (available from start), OR
            // 2. The story that unlocks it is completed
            bool isUnlocked = heroRef.StoryId == null || completedStoryIdsSet.Contains(heroRef.StoryId);

            if (isUnlocked)
            {
                var imageUrl = heroRef.HeroImageUrl
                    ?? (heroImageMap.TryGetValue(heroRef.HeroId, out var cachedUrl) ? cachedUrl : string.Empty);

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

        var allowedEpicHeroIds = new HashSet<string>(heroReferences.Select(h => h.HeroId));

        if (storyDefinition != null && storyDefinition.UnlockedHeroes != null && storyDefinition.UnlockedHeroes.Count > 0)
        {
            foreach (var unlockedHero in storyDefinition.UnlockedHeroes)
            {
                // Check if hero was already unlocked
                if (!previousUnlockedHeroIds.Contains(unlockedHero.HeroId))
                {
                    // Only allow heroes that exist in the epic hero references AND resolve to a real hero.
                    // This prevents legacy story definitions from introducing orphan/legacy heroIds like "puf-puf".
                    if (!allowedEpicHeroIds.Contains(unlockedHero.HeroId))
                    {
                        continue;
                    }

                    // Get hero image URL from EpicHero
                    var imageUrl = await GetHeroImageUrlAsync(unlockedHero.HeroId, ct);
                    if (string.IsNullOrWhiteSpace(imageUrl))
                    {
                        continue;
                    }

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
    private async Task<List<UnlockedHeroDto>> GetUnlockedHeroesFromStoriesAsync(
        List<string> completedStoryIds,
        HashSet<string> allowedEpicHeroIds,
        CancellationToken ct = default)
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
                    if (!allowedEpicHeroIds.Contains(unlockedHero.HeroId))
                    {
                        continue;
                    }

                    // Get hero image URL from EpicHero
                    var imageUrl = await GetHeroImageUrlAsync(unlockedHero.HeroId, ct);
                    if (string.IsNullOrWhiteSpace(imageUrl))
                    {
                        continue;
                    }

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

