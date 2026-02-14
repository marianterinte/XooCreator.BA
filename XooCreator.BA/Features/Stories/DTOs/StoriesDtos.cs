using XooCreator.BA.Features.TreeOfLight;
using XooCreator.BA.Features.TreeOfLight.DTOs;

namespace XooCreator.BA.Features.Stories.DTOs;

public record StoryContentDto
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? CoverImageUrl { get; init; }
    public string? Summary { get; init; }
    public string? StoryTopic { get; init; }
    public string? StoryType { get; init; }
    public bool IsEvaluative { get; init; } = false; // If true, this story contains quizzes that should be evaluated
    public List<string> UnlockedStoryHeroes { get; init; } = new();
    public List<string> DialogParticipants { get; init; } = new();
    public List<StoryTileDto> Tiles { get; init; } = new();
    public string? OwnerEmail { get; init; }
    public List<string>? AvailableLanguages { get; init; } // Available language codes for this story (e.g., ["ro-ro", "en-us", "de-de"])
}

public record StoryTileDto
{
    public required string Type { get; init; } // "page" or "quiz"
    public required string Id { get; init; }
    public string? BranchId { get; init; }
    
    // Page fields
    public string? Caption { get; init; }
    public string? Text { get; init; }
    public string? ImageUrl { get; init; }
    public string? AudioUrl { get; init; }
    public string? VideoUrl { get; init; }
    
    // Quiz fields
    public string? Question { get; init; }
    public List<StoryAnswerDto> Answers { get; init; } = new();

    // Dialog fields
    public string? DialogRootNodeId { get; init; }
    public List<StoryDialogNodeDto> DialogNodes { get; init; } = new();
    
    // Character selection specific: selected heroes for this tile (subset of dialogParticipants)
    public List<string>? AvailableHeroIds { get; init; }
}

public record StoryDialogNodeDto
{
    public required string NodeId { get; init; }
    public required string SpeakerType { get; init; }
    public string? SpeakerHeroId { get; init; }
    public string? Text { get; init; }
    public string? AudioUrl { get; init; }
    public List<StoryDialogOptionDto> Options { get; init; } = new();
    public int? X { get; init; }
    public int? Y { get; init; }
}

public record StoryDialogOptionDto
{
    public required string Id { get; init; }
    public required string NextNodeId { get; init; }
    public string? Text { get; init; }
    public string? JumpToTileId { get; init; }
    public string? SetBranchId { get; init; }
    public List<TokenReward> Tokens { get; init; } = new();
}

public record StoryAnswerDto
{
    public required string Id { get; init; }
    public required string Text { get; init; }
    public bool IsCorrect { get; init; } = false; // True if this is the correct answer for the quiz
    public List<TokenReward> Tokens { get; init; } = new();
}

public record UserStoryProgressDto
{
    public required string StoryId { get; init; }
    public required string TileId { get; init; }
    public DateTime ReadAt { get; init; }
}

public record GetStoriesResponse
{
    public List<StoryContentDto> Stories { get; init; } = new();
}

public record CheckStoryIdResponse
{
    public required bool Exists { get; init; }
}

public record GetStoryByIdResponse
{
    public StoryContentDto? Story { get; init; }
    public List<UserStoryProgressDto> UserProgress { get; init; } = new();
    public bool IsCompleted { get; init; } = false;
    public DateTime? CompletedAt { get; init; }
}

public record MarkTileAsReadRequest
{
    public required string StoryId { get; init; }
    public required string TileId { get; init; }
}

public record MarkTileAsReadResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record ResetStoryProgressRequest
{
    public required string StoryId { get; init; }
}

public record ResetStoryProgressResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record StoryCompletionInfo
{
    public bool IsCompleted { get; init; }
    public DateTime? CompletedAt { get; init; }
}

