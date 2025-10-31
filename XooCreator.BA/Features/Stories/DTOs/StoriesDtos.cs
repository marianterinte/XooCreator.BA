using XooCreator.BA.Features.TreeOfLight;
using XooCreator.BA.Features.TreeOfLight.DTOs;

namespace XooCreator.BA.Features.Stories.DTOs;

public record StoryContentDto
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? CoverImageUrl { get; init; }
    public List<string> UnlockedStoryHeroes { get; init; } = new();
    public List<StoryTileDto> Tiles { get; init; } = new();
}

public record StoryTileDto
{
    public required string Type { get; init; } // "page" or "quiz"
    public required string Id { get; init; }
    
    // Page fields
    public string? Caption { get; init; }
    public string? Text { get; init; }
    public string? ImageUrl { get; init; }
    public string? AudioUrl { get; init; }
    
    // Quiz fields
    public string? Question { get; init; }
    public List<StoryAnswerDto> Answers { get; init; } = new();
}

public record StoryAnswerDto
{
    public required string Id { get; init; }
    public required string Text { get; init; }
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

