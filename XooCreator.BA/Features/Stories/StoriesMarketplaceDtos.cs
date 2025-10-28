using XooCreator.BA.Data;

namespace XooCreator.BA.Features.Stories;

/// <summary>
/// DTO for detailed story information in marketplace
/// </summary>
public record StoryDetailsDto
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? CoverImageUrl { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? Summary { get; init; }
    public int PriceInCredits { get; init; }
    public string Region { get; init; } = string.Empty;
    public string AgeRating { get; init; } = string.Empty;
    public string Difficulty { get; init; } = string.Empty;
    public List<string> Characters { get; init; } = new();
    public List<string> Tags { get; init; } = new();
    public bool IsFeatured { get; init; }
    public bool IsNew { get; init; }
    public int EstimatedReadingTime { get; init; }
    public bool IsPurchased { get; init; }
    public bool IsCompleted { get; init; }
    public int ProgressPercentage { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<string> UnlockedStoryHeroes { get; init; } = new();
    public string Category { get; init; } = string.Empty;
    public string StoryCategory { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid? UpdatedBy { get; init; }
}

/// <summary>
/// DTO for story marketplace item with purchase information
/// </summary>
public record StoryMarketplaceItemDto
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? CoverImageUrl { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? Summary { get; init; }
    public int PriceInCredits { get; init; }
    public string AgeRating { get; init; } = string.Empty;
    public List<string> Characters { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Request to purchase a story
/// </summary>
public record PurchaseStoryRequest
{
    public required string StoryId { get; init; }
}

/// <summary>
/// Response for story purchase
/// </summary>
public record PurchaseStoryResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int RemainingCredits { get; init; }
    public int CreditsSpent { get; init; }
}

/// <summary>
/// Response for marketplace stories
/// </summary>
public record GetMarketplaceStoriesResponse
{
    public List<StoryMarketplaceItemDto> Stories { get; init; } = new();
    public List<StoryMarketplaceItemDto> FeaturedStories { get; init; } = new();
    public List<string> AvailableRegions { get; init; } = new();
    public List<string> AvailableAgeRatings { get; init; } = new();
    public List<string> AvailableCharacters { get; init; } = new();
    public int TotalCount { get; init; }
    public bool HasMore { get; init; }
}

/// <summary>
/// Request to search and filter stories
/// </summary>
public record SearchStoriesRequest
{
    public string? SearchTerm { get; init; }
    public List<string> Regions { get; init; } = new();
    public List<string> AgeRatings { get; init; } = new();
    public List<string> Characters { get; init; } = new();
    public List<string> Categories { get; init; } = new();
    public List<string> Difficulties { get; init; } = new();
    public string CompletionStatus { get; init; } = "all"; // all, completed, in-progress, not-started
    public string SortBy { get; init; } = "sortOrder"; // title, date, difficulty, progress, sortOrder
    public string SortOrder { get; init; } = "asc"; // asc, desc
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Response for user's purchased stories
/// </summary>
public record GetUserPurchasedStoriesResponse
{
    public List<StoryMarketplaceItemDto> PurchasedStories { get; init; } = new();
    public int TotalCount { get; init; }
}
