namespace XooCreator.BA.Features.ParentDashboard.DTOs;

/// <summary>
/// Response for getting all stories read by the user
/// </summary>
public record ReadStoriesResponse
{
    public List<ReadStoryDto> Stories { get; init; } = new();
    public int TotalCount { get; init; }
}

/// <summary>
/// DTO for a story that has been read by the user
/// </summary>
public record ReadStoryDto
{
    public string StoryId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? CoverImageUrl { get; init; }
    public int TotalTilesRead { get; init; }
    public int TotalTiles { get; init; }
    public int ProgressPercentage { get; init; }
    public DateTime? LastReadAt { get; init; }
    public bool IsCompleted { get; init; }
    public bool IsPartOfEpic { get; init; } = false; // If true, this story is part of an epic (draft or published) and should not appear as independent story
}

/// <summary>
/// Response for getting all evaluative stories with their latest results
/// </summary>
public record EvaluativeStoriesResponse
{
    public List<EvaluativeStoryDto> Stories { get; init; } = new();
    public int TotalCount { get; init; }
}

/// <summary>
/// DTO for an evaluative story with its latest evaluation result
/// </summary>
public record EvaluativeStoryDto
{
    public string StoryId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? CoverImageUrl { get; init; }
    public int TotalQuizzes { get; init; }
    public bool HasEvaluationResult { get; init; }
    public EvaluationResultDto? LatestResult { get; init; }
}

/// <summary>
/// DTO for evaluation result details
/// </summary>
public record EvaluationResultDto
{
    public int ScorePercentage { get; init; }
    public int CorrectAnswers { get; init; }
    public int TotalQuizzes { get; init; }
    public DateTime CompletedAt { get; init; }
}

/// <summary>
/// Response for getting detailed evaluation information for a specific story
/// </summary>
public record EvaluationDetailsResponse
{
    public string StoryId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public EvaluationResultDto LatestResult { get; init; } = null!;
    public List<QuizAnswerDetailDto> QuizDetails { get; init; } = new();
}

/// <summary>
/// DTO for individual quiz answer details
/// </summary>
public record QuizAnswerDetailDto
{
    public string TileId { get; init; } = string.Empty;
    public string Question { get; init; } = string.Empty;
    public string SelectedAnswerText { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
    public string? CorrectAnswerText { get; init; }
}

/// <summary>
/// Response for resetting all story progress (read progress and evaluation results)
/// </summary>
public record ResetAllProgressResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int ReadProgressDeleted { get; init; }
    public int EvaluationResultsDeleted { get; init; }
    public int QuizAnswersDeleted { get; init; }
}

/// <summary>
/// Response for getting child age preferences
/// </summary>
public record GetChildAgePreferencesResponse
{
    public List<string>? SelectedAgeGroupIds { get; init; }
    public bool AutoFilterStoriesByAge { get; init; }
}

/// <summary>
/// Request for updating child age preferences
/// </summary>
public record UpdateChildAgePreferencesRequest
{
    public List<string>? SelectedAgeGroupIds { get; init; }
    public bool? AutoFilterStoriesByAge { get; init; }
}

/// <summary>
/// Response for updating child age preferences
/// </summary>
public record UpdateChildAgePreferencesResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string>? SelectedAgeGroupIds { get; init; }
    public bool AutoFilterStoriesByAge { get; init; }
}