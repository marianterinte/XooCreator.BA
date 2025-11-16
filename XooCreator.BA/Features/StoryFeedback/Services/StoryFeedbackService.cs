using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryFeedback.DTOs;
using XooCreator.BA.Features.StoryFeedback.Repositories;

namespace XooCreator.BA.Features.StoryFeedback.Services;

public class StoryFeedbackService : IStoryFeedbackService
{
    private readonly IStoryFeedbackRepository _repository;

    public StoryFeedbackService(IStoryFeedbackRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubmitStoryFeedbackResponse> SubmitFeedbackAsync(Guid userId, string email, SubmitStoryFeedbackRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.StoryId))
            return new SubmitStoryFeedbackResponse { Success = false, ErrorMessage = "StoryId is required" };

        // Allow feedback if either feedbackText is provided or there are selections in whatCouldBeBetter
        var hasFeedbackText = !string.IsNullOrWhiteSpace(request.FeedbackText);
        var hasSelections = request.WhatCouldBeBetter != null && request.WhatCouldBeBetter.Count > 0;
        
        if (!hasFeedbackText && !hasSelections)
            return new SubmitStoryFeedbackResponse { Success = false, ErrorMessage = "Either feedback text or improvement selections are required" };

        var feedback = await _repository.CreateFeedbackAsync(
            userId, 
            request.StoryId, 
            email, 
            request.FeedbackText ?? string.Empty, 
            request.WhatCouldBeBetter ?? new List<string>(), 
            ct);
        
        if (feedback == null)
            return new SubmitStoryFeedbackResponse { Success = false, ErrorMessage = "Feedback already submitted for this story" };

        return new SubmitStoryFeedbackResponse { Success = true, FeedbackId = feedback.Id };
    }

    public async Task<SetFeedbackPreferenceResponse> SetPreferenceAsync(Guid userId, SetFeedbackPreferenceRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.StoryId))
            return new SetFeedbackPreferenceResponse { Success = false, ErrorMessage = "StoryId is required" };

        FeedbackPreferenceType preferenceType;
        if (request.PreferenceType.Equals("never", StringComparison.OrdinalIgnoreCase))
            preferenceType = FeedbackPreferenceType.Never;
        else if (request.PreferenceType.Equals("later", StringComparison.OrdinalIgnoreCase))
            preferenceType = FeedbackPreferenceType.Later;
        else
            return new SetFeedbackPreferenceResponse { Success = false, ErrorMessage = "Invalid preference type" };

        var preference = await _repository.CreateOrUpdatePreferenceAsync(userId, request.StoryId, preferenceType, ct);
        
        if (preference == null)
            return new SetFeedbackPreferenceResponse { Success = false, ErrorMessage = "Failed to set preference" };

        return new SetFeedbackPreferenceResponse { Success = true };
    }

    public async Task<CheckFeedbackStatusResponse> CheckStatusAsync(Guid userId, string storyId, CancellationToken ct = default)
    {
        var hasSubmitted = await _repository.HasUserSubmittedFeedbackAsync(userId, storyId, ct);
        var hasDeclined = await _repository.HasUserDeclinedFeedbackAsync(userId, storyId, ct);
        var hasPostponed = await _repository.HasUserPostponedFeedbackAsync(userId, storyId, ct);

        // Show modal if:
        // - User hasn't submitted feedback
        // - User hasn't declined feedback
        // - User hasn't postponed (or if they postponed, we can show again after some time - for now, we show again)
        var shouldShow = !hasSubmitted && !hasDeclined;

        return new CheckFeedbackStatusResponse
        {
            ShouldShowModal = shouldShow,
            HasSubmittedFeedback = hasSubmitted,
            HasDeclinedFeedback = hasDeclined,
            HasPostponedFeedback = hasPostponed
        };
    }

    public async Task<GetAllFeedbacksResponse> GetAllFeedbacksAsync(CancellationToken ct = default)
    {
        var feedbacks = await _repository.GetAllFeedbacksAsync(ct);
        
        var feedbackDtos = feedbacks.Select(f => new FeedbackDto
        {
            Id = f.Id,
            UserId = f.UserId,
            UserFirstName = f.User?.FirstName ?? string.Empty,
            UserLastName = f.User?.LastName ?? string.Empty,
            UserEmail = f.Email,
            StoryId = f.StoryId,
            FeedbackText = f.FeedbackText,
            WhatCouldBeBetter = f.WhatCouldBeBetter ?? new List<string>(),
            CreatedAt = f.CreatedAt
        }).ToList();

        return new GetAllFeedbacksResponse
        {
            Success = true,
            Feedbacks = feedbackDtos
        };
    }
}

