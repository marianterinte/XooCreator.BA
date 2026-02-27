using XooCreator.BA.Data.Enums;
using System.Text.Json.Serialization;

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
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StoryStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public string? CreationNotes { get; set; }
    public string? OwnerEmail { get; set; }
    public bool IsOwnedByCurrentUser { get; set; }
    public bool IsEvaluative { get; set; } = false;
    public bool IsPartOfEpic { get; set; } = false;
    public bool IsFullyInteractive { get; set; } = false;
    /// <summary>Available language codes for this story (e.g. ro-ro, en-us).</summary>
    public List<string>? AvailableLanguages { get; set; }
    /// <summary>Language codes that have audio support (e.g. ro-ro, en-us).</summary>
    public List<string>? AudioLanguages { get; set; }
}


