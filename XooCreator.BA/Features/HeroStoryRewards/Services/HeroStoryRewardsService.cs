using Microsoft.Extensions.Logging;
using XooCreator.BA.Features.HeroStoryRewards.DTOs;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;
using XooCreator.BA.Features.Stories.Mappers;
using TokenReward = XooCreator.BA.Features.TreeOfLight.DTOs.TokenReward;
using TokenFamily = XooCreator.BA.Features.TreeOfLight.DTOs.TokenFamily;

namespace XooCreator.BA.Features.HeroStoryRewards.Services;

public class HeroStoryRewardsService : IHeroStoryRewardsService
{
    private readonly ITreeOfHeroesRepository _treeOfHeroesRepository;
    private readonly ILogger<HeroStoryRewardsService> _logger;

    public HeroStoryRewardsService(
        ITreeOfHeroesRepository treeOfHeroesRepository,
        ILogger<HeroStoryRewardsService> logger)
    {
        _treeOfHeroesRepository = treeOfHeroesRepository;
        _logger = logger;
    }

    public async Task<CompleteStoryRewardResponse> AwardStoryRewardsAsync(Guid userId, CompleteStoryRewardRequest request, CancellationToken ct = default)
    {
        var warnings = new List<string>();

        if (request.Tokens == null || request.Tokens.Count == 0)
        {
            _logger.LogDebug("AwardStoryRewards: No tokens to award storyId={StoryId} source={Source} userId={UserId}",
                request.StoryId, request.Source, userId);
            return new CompleteStoryRewardResponse { Completed = true, TokensAwarded = false, Warnings = warnings };
        }

        var rewards = new List<TokenReward>();
        foreach (var t in request.Tokens)
        {
            if (t.Quantity <= 0)
            {
                warnings.Add($"Token {t.Type}:{t.Value} has quantity <= 0, skipped.");
                continue;
            }
            var family = StoryDefinitionMapper.MapFamily(t.Type?.Trim() ?? "");
            var value = (t.Value ?? "").Trim();
            if (string.IsNullOrEmpty(value))
            {
                warnings.Add($"Token with type {t.Type} has empty value, skipped.");
                continue;
            }
            rewards.Add(new TokenReward { Type = family, Value = value, Quantity = t.Quantity });
        }

        if (rewards.Count == 0)
        {
            return new CompleteStoryRewardResponse { Completed = true, TokensAwarded = false, Warnings = warnings };
        }

        try
        {
            await _treeOfHeroesRepository.AwardTokensAsync(userId, rewards);
            _logger.LogInformation("AwardStoryRewards: Awarded {Count} token types for storyId={StoryId} source={Source} userId={UserId}",
                rewards.Count, request.StoryId, request.Source, userId);
            return new CompleteStoryRewardResponse { Completed = true, TokensAwarded = true, Warnings = warnings };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AwardStoryRewards: Failed to persist tokens (non-blocking) storyId={StoryId} userId={UserId}", request.StoryId, userId);
            warnings.Add("Tokens could not be saved; story completion was not affected.");
            return new CompleteStoryRewardResponse { Completed = true, TokensAwarded = false, Warnings = warnings };
        }
    }
}
