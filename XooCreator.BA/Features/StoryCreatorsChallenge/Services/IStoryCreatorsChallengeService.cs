using XooCreator.BA.Features.StoryCreatorsChallenge.DTOs;

namespace XooCreator.BA.Features.StoryCreatorsChallenge.Services;

public interface IStoryCreatorsChallengeService
{
    Task<List<StoryCreatorsChallengeListItemDto>> GetAllChallengesAsync(string? languageCode, CancellationToken ct);
    Task<StoryCreatorsChallengeDto?> GetChallengeByIdAsync(string challengeId, string? languageCode, CancellationToken ct);
    Task<StoryCreatorsChallengeDto> CreateChallengeAsync(StoryCreatorsChallengeDto dto, Guid userId, CancellationToken ct);
    Task<StoryCreatorsChallengeDto> UpdateChallengeAsync(string challengeId, StoryCreatorsChallengeDto dto, Guid userId, CancellationToken ct);
    Task<bool> DeleteChallengeAsync(string challengeId, CancellationToken ct);
    Task<PublicChallengeDto?> GetActiveChallengeAsync(string languageCode, CancellationToken ct);
    Task<StoryCreatorsChallengeDto> ExtendChallengeEndDateAsync(string challengeId, DateTime newEndDate, Guid userId, CancellationToken ct);

    // Subscription methods
    Task<ChallengeSubscriptionDto> SubscribeToChallengeAsync(string challengeId, Guid userId, CancellationToken ct);
    Task<bool> UnsubscribeFromChallengeAsync(string challengeId, Guid userId, CancellationToken ct);
    Task<bool> IsSubscribedAsync(string challengeId, Guid userId, CancellationToken ct);
    Task<List<ChallengeSubscriptionDto>> GetUserSubscriptionsAsync(Guid userId, CancellationToken ct);

    // Submission methods
    Task<ChallengeSubmissionDto> SubmitStoryToChallengeAsync(string challengeId, string storyId, Guid userId, CancellationToken ct);
    Task<bool> RemoveSubmissionAsync(string challengeId, string storyId, Guid userId, CancellationToken ct);
    Task<List<ChallengeSubmissionDto>> GetUserSubmissionsAsync(string challengeId, Guid userId, CancellationToken ct);
    Task<ChallengeLeaderboardDto> GetChallengeLeaderboardAsync(string challengeId, Guid? currentUserId, CancellationToken ct);
    Task<PublicChallengeLeaderboardDto> GetPublicLeaderboardAsync(string challengeId, CancellationToken ct);

    // Winner determination
    Task<ChallengeSubmissionDto?> DetermineWinnerAsync(string challengeId, CancellationToken ct);
    Task<bool> SetWinnerManuallyAsync(string challengeId, string storyId, CancellationToken ct);
}
