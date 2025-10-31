using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.Library.DTOs;

public class GetUserOwnedStoriesResponse
{
    public List<OwnedStoryDto> Stories { get; set; } = new();
    public int TotalCount { get; set; }
}

public class OwnedStoryDto
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string? StoryTopic { get; set; }
    public StoryType StoryType { get; set; }
    public StoryStatus Status { get; set; }
    public DateTime PurchasedAt { get; set; }
    public decimal PurchasePrice { get; set; }
}

public class GetUserCreatedStoriesResponse
{
    public List<CreatedStoryDto> Stories { get; set; } = new();
    public int TotalCount { get; set; }
}

public class CreatedStoryDto
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string? StoryTopic { get; set; }
    public StoryType StoryType { get; set; }
    public StoryStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public string? CreationNotes { get; set; }
}


