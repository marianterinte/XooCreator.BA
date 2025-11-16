using XooCreator.BA.Features.StoryFeedback.DTOs;

namespace XooCreator.BA.Features.StoryFeedback.Services;

public interface IStoryFeedbackService
{
    Task<SubmitStoryFeedbackResponse> SubmitFeedbackAsync(Guid userId, string email, SubmitStoryFeedbackRequest request, CancellationToken ct = default);
    Task<SetFeedbackPreferenceResponse> SetPreferenceAsync(Guid userId, SetFeedbackPreferenceRequest request, CancellationToken ct = default);
    Task<CheckFeedbackStatusResponse> CheckStatusAsync(Guid userId, string storyId, CancellationToken ct = default);
    Task<GetAllFeedbacksResponse> GetAllFeedbacksAsync(CancellationToken ct = default);
}

