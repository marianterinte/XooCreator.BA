namespace XooCreator.BA.Features.StoryFeedback.DTOs;

public record SubmitStoryFeedbackRequest
{
    public string StoryId { get; init; } = string.Empty;
    public string FeedbackText { get; init; } = string.Empty; // Free text feedback
    public List<string> WhatCouldBeBetter { get; init; } = new(); // What could be improved (multiple choice)
}

public record SubmitStoryFeedbackResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public Guid? FeedbackId { get; init; }
}

public record SetFeedbackPreferenceRequest
{
    public string StoryId { get; init; } = string.Empty;
    public string PreferenceType { get; init; } = string.Empty; // "later", "never"
}

public record SetFeedbackPreferenceResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record CheckFeedbackStatusRequest
{
    public string StoryId { get; init; } = string.Empty;
}

public record CheckFeedbackStatusResponse
{
    public bool ShouldShowModal { get; init; }
    public bool HasSubmittedFeedback { get; init; }
    public bool HasDeclinedFeedback { get; init; }
    public bool HasPostponedFeedback { get; init; }
}

public record FeedbackDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserFirstName { get; init; } = string.Empty;
    public string UserLastName { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public string StoryId { get; init; } = string.Empty;
    public string FeedbackText { get; init; } = string.Empty;
    public List<string> WhatCouldBeBetter { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public record GetAllFeedbacksResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public List<FeedbackDto> Feedbacks { get; init; } = new();
}

