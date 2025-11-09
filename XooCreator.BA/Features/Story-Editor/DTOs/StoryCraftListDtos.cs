using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.DTOs;

public record StoryCraftListItemDto
{
    public required string StoryId { get; init; }
    public required string Lang { get; init; }
    public required string Title { get; init; }
    public string? CoverImageUrl { get; init; }
    public required string Status { get; init; } // FE semantic
    public required DateTime UpdatedAt { get; init; }
    public required string OwnerEmail { get; init; }
    public bool IsOwnedByCurrentUser { get; init; }
}

public record ListStoryCraftsResponse
{
    public List<StoryCraftListItemDto> Stories { get; init; } = new();
    public int TotalCount { get; init; }
}


