using XooCreator.BA.Data;
using StoryFeedbackEntity = XooCreator.BA.Data.StoryFeedback;
using StoryFeedbackPreferenceEntity = XooCreator.BA.Data.StoryFeedbackPreference;

namespace XooCreator.BA.Features.StoryFeedback.Repositories;

public interface IStoryFeedbackRepository
{
    Task<bool> HasUserSubmittedFeedbackAsync(Guid userId, string storyId, CancellationToken ct = default);
    Task<bool> HasUserDeclinedFeedbackAsync(Guid userId, string storyId, CancellationToken ct = default);
    Task<bool> HasUserPostponedFeedbackAsync(Guid userId, string storyId, CancellationToken ct = default);
    Task<StoryFeedbackEntity?> CreateFeedbackAsync(Guid userId, string storyId, string email, string feedbackText, List<string> whatLiked, List<string> whatDisliked, List<string> whatCouldBeBetter, CancellationToken ct = default);
    Task<List<StoryFeedbackEntity>> GetAllFeedbacksAsync(CancellationToken ct = default);
    Task<StoryFeedbackPreferenceEntity?> CreateOrUpdatePreferenceAsync(Guid userId, string storyId, FeedbackPreferenceType preferenceType, CancellationToken ct = default);
}

