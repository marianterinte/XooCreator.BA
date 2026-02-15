using XooCreator.BA.Features.HeroStoryRewards.DTOs;

namespace XooCreator.BA.Features.HeroStoryRewards.Services;

public interface IHeroStoryRewardsService
{
    /// <summary>
    /// Awards story rewards (tokens) to the user. Non-blocking: completion is always true;
    /// validation issues are returned as warnings and do not prevent awarding when possible.
    /// </summary>
    Task<CompleteStoryRewardResponse> AwardStoryRewardsAsync(Guid userId, CompleteStoryRewardRequest request, CancellationToken ct = default);
}
