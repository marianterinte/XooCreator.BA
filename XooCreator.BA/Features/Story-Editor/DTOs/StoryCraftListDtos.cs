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
    public Guid? AssignedReviewerUserId { get; init; }
    public bool IsAssignedToCurrentUser { get; init; }
    /// <summary>Available language codes for this story (e.g. ro-ro, en-us).</summary>
    public List<string>? AvailableLanguages { get; init; }
    /// <summary>Language codes that have audio support.</summary>
    public List<string>? AudioLanguages { get; init; }
}

public record ListStoryCraftsResponse
{
    public List<StoryCraftListItemDto> Stories { get; init; } = new();
    public int TotalCount { get; init; }
}


