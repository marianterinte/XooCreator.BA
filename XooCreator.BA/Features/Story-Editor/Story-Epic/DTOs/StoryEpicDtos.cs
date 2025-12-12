namespace XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

public record StoryEpicDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CoverImageUrl { get; init; }
    public string Status { get; init; } = "draft";
    public DateTime? PublishedAtUtc { get; init; }
    public List<StoryEpicRegionDto> Regions { get; init; } = new();
    public List<StoryEpicStoryNodeDto> Stories { get; init; } = new();
    public List<StoryEpicUnlockRuleDto> Rules { get; init; } = new();
}

public record StoryEpicRegionDto
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public string? ImageUrl { get; init; }
    public int SortOrder { get; init; }
    public bool IsLocked { get; init; }
    public bool IsStartupRegion { get; init; }
    public double? X { get; init; }
    public double? Y { get; init; }
}

public record StoryEpicStoryNodeDto
{
    public required string StoryId { get; init; }
    public required string RegionId { get; init; }
    public string? RewardImageUrl { get; init; }
    public int SortOrder { get; init; }
    public double? X { get; init; }
    public double? Y { get; init; }
}

public record StoryEpicUnlockRuleDto
{
    public required string Type { get; init; }
    public required string FromId { get; init; }
    public required string ToRegionId { get; init; }
    public List<string> RequiredStories { get; init; } = new();
    public int? MinCount { get; init; }
    public string? StoryId { get; init; }
    public int SortOrder { get; init; }
}

public record StoryEpicStateDto
{
    public StoryEpicDto Epic { get; init; } = null!;
    public StoryEpicPreviewDto Preview { get; init; } = null!;
}

public record StoryEpicPreviewDto
{
    public List<StoryEpicPreviewNodeDto> Nodes { get; init; } = new();
    public List<StoryEpicPreviewEdgeDto> Edges { get; init; } = new();
}

public record StoryEpicPreviewNodeDto
{
    public required string Id { get; init; }
    public required string Type { get; init; } // "region" or "story"
    public required string Label { get; init; }
    public string? ImageUrl { get; init; }
    public double? X { get; init; }
    public double? Y { get; init; }
}

public record StoryEpicPreviewEdgeDto
{
    public required string FromId { get; init; }
    public required string ToId { get; init; }
    public string? Type { get; init; } // "unlock" or "contains"
}

public record StoryEpicListItemDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CoverImageUrl { get; init; }
    public string Status { get; init; } = "draft";
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    public int StoryCount { get; init; }
    public int RegionCount { get; init; }
    
    // Review workflow fields for list display
    public Guid? AssignedReviewerUserId { get; init; }
    public bool IsAssignedToCurrentUser { get; init; } // Computed in service based on current user
    public bool IsOwnedByCurrentUser { get; init; } // Computed in service based on current user
}

public record StoryEpicStoryOptionDto
{
    public required string StoryId { get; init; }
    public required string Title { get; init; }
    public string? CoverImageUrl { get; init; }
}

public record GetStoryEpicStoryOptionsResponse
{
    public List<StoryEpicStoryOptionDto> Stories { get; init; } = new();
}

public record StoryEpicPublishResponse
{
    public required string EpicId { get; init; }
    public required string Status { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
}

// Epic Progress DTOs
public record EpicProgressDto
{
    public required string RegionId { get; init; }
    public bool IsUnlocked { get; init; }
    public DateTime? UnlockedAt { get; init; }
}

public record EpicStoryProgressDto
{
    public required string StoryId { get; init; }
    public string? SelectedAnswer { get; init; }
    public DateTime CompletedAt { get; init; }
}

public record EpicProgressStateDto
{
    public List<EpicCompletedStoryDto> CompletedStories { get; init; } = new();
    public List<string> UnlockedRegions { get; init; } = new();
}

public record EpicCompletedStoryDto
{
    public required string StoryId { get; init; }
    public string? SelectedAnswer { get; init; }
    public DateTime CompletedAt { get; init; }
}

// Epic State with Progress (for player)
public record StoryEpicStateWithProgressDto
{
    public StoryEpicDto Epic { get; init; } = null!;
    public StoryEpicPreviewDto Preview { get; init; } = null!;
    public EpicProgressStateDto Progress { get; init; } = null!;
}



