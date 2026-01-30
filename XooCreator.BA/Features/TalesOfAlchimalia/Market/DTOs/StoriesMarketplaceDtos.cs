namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

public record StoryDetailsDto
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? CoverImageUrl { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? CreatedByName { get; init; }
    public string? Summary { get; init; }
    public double PriceInCredits { get; init; }
    public string Region { get; init; } = string.Empty;
    public List<string> Characters { get; init; } = new();
    public List<string> Tags { get; init; } = new();
    public bool IsNew { get; init; }
    public bool IsPurchased { get; init; }
    public bool IsOwned { get; init; }
    public bool IsCompleted { get; init; }
    public int ProgressPercentage { get; init; }
    public int CompletedTiles { get; init; }
    public int TotalTiles { get; init; }
    public string? LastReadTileId { get; init; }
    public DateTime? LastReadAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<string> UnlockedStoryHeroes { get; init; } = new();
    public string? StoryTopic { get; init; }
    public string StoryType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid? UpdatedBy { get; init; }
    public List<string> AvailableLanguages { get; init; } = new();
    public List<AgeGroupInfoDto> AgeGroups { get; init; } = new(); // Age groups for this story
    // Review statistics
    public double AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public StoryReviewDto? UserReview { get; init; } // Current user's review if exists
    public int ReadersCount { get; init; }
    public int LikesCount { get; init; } // Total likes for this story
    public bool IsLiked { get; init; } // Whether current user has liked this story
    public bool IsEvaluative { get; init; } = false; // If true, this story contains quizzes that should be evaluated
    public bool IsPartOfEpic { get; init; } = false; // If true, this story is part of an epic (draft or published) and should not appear as independent story
}

public record AgeGroupInfoDto
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public int MinAge { get; init; }
    public int MaxAge { get; init; }
}

public record StoryMarketplaceItemDto
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? CoverImageUrl { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? CreatedByName { get; init; }
    public string? Summary { get; init; }
    public double PriceInCredits { get; init; }
    public string AgeRating { get; init; } = string.Empty;
    public List<string> Characters { get; init; } = new();
    public List<string> Tags { get; init; } = new(); // Topic IDs (e.g., ["classic_author", "edu_math"])
    public DateTime CreatedAt { get; init; }
    public string? StoryTopic { get; init; }
    public string StoryType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public List<string> AvailableLanguages { get; init; } = new(); // e.g., ["ro-ro", "en-us", "hu-hu"]
    public List<string> AgeGroupIds { get; init; } = new(); // Age group IDs (e.g., ["preschool_3_5", "early_school_6_8"])
    // Note: IsPurchased and IsOwned removed from list DTO for 100% global cache.
    // These properties remain in StoryDetailsDto (loaded per request when viewing story details).
    public int ReadersCount { get; init; }
    public int LikesCount { get; init; } // Total likes for this story
    public double AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public bool IsEvaluative { get; init; } = false;
}

public record PurchaseStoryRequest
{
    public required string StoryId { get; init; }
}

public record PurchaseStoryResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public double RemainingCredits { get; init; }
    public double CreditsSpent { get; init; }
}

public record GetMarketplaceStoriesResponse
{
    public List<StoryMarketplaceItemDto> Stories { get; init; } = new();
    public int TotalCount { get; init; }
    public bool HasMore { get; init; }
}

public record SearchStoriesRequest
{
    public string? SearchTerm { get; init; }
    public List<string> Regions { get; init; } = new();
    public List<string> AgeRatings { get; init; } = new();
    public List<string> AgeGroupIds { get; init; } = new(); // Filter by age group IDs (e.g., ["preschool_3_5"])
    public List<string> Characters { get; init; } = new();
    public List<string> Categories { get; init; } = new();
    public List<string> Difficulties { get; init; } = new();
    public List<string> Topics { get; init; } = new(); // Filter by topic IDs (e.g., ["alchimalia_universe", "classic_author"])
    public List<string> AvailableLanguages { get; init; } = new(); // Filter by available languages (e.g., ["ro-ro", "en-us", "hu-hu"])
    public bool? IsEvaluative { get; init; } // Filter by evaluative flag
    public string CompletionStatus { get; init; } = "all";
    public string SortBy { get; init; } = "sortOrder";
    public string SortOrder { get; init; } = "asc";
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public int? Skip { get; init; }
    public string SearchType { get; init; } = "title"; // "title", "author"
}

public record GetUserPurchasedStoriesResponse
{
    public List<StoryMarketplaceItemDto> PurchasedStories { get; init; } = new();
    public int TotalCount { get; init; }
}

// Story Review DTOs
public record StoryReviewDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? UserPicture { get; init; }
    public string StoryId { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsOwnReview { get; init; } // True if this is the current user's review
}

public record CreateStoryReviewRequest
{
    public required string StoryId { get; init; }
    public required int Rating { get; init; } // 1-5
    public string? Comment { get; init; }
}

public record CreateStoryReviewResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public StoryReviewDto? Review { get; init; }
}

public record UpdateStoryReviewRequest
{
    public required Guid ReviewId { get; init; }
    public required int Rating { get; init; } // 1-5
    public string? Comment { get; init; }
}

public record UpdateStoryReviewResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public StoryReviewDto? Review { get; init; }
}

public record DeleteStoryReviewResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record GetStoryReviewsRequest
{
    public required string StoryId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; } = "createdAt"; // createdAt, rating
    public string SortOrder { get; init; } = "desc"; // asc, desc
}

public record GetStoryReviewsResponse
{
    public List<StoryReviewDto> Reviews { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public bool HasMore { get; init; }
    public double AverageRating { get; init; }
    public Dictionary<int, int> RatingDistribution { get; init; } = new(); // Rating -> Count
}

public record GlobalReviewStatisticsResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int TotalReviews { get; init; }
    public double AverageRating { get; init; }
    public Dictionary<int, int> RatingDistribution { get; init; } = new();
}

public record ReadersSummaryResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int TotalReaders { get; init; }
    public List<ReadersLeaderboardItem> TopStories { get; init; } = new();
    public List<ReadersTrendPointDto> Trend { get; init; } = new();
    public List<ReadersCorrelationItemDto> RatingCorrelation { get; init; } = new();
}

public record ReadersLeaderboardItem(string StoryId, string Title, int ReadersCount, double AverageRating);

public record ReadersTrendPointDto(string Date, int ReadersCount);

public record ReadersCorrelationItemDto(string StoryId, string Title, int ReadersCount, int ReviewsCount, double AverageRating);

// Epic Marketplace DTOs
public record EpicMarketplaceItemDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CoverImageUrl { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? CreatedByName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    public int StoryCount { get; init; } // Number of stories in this epic
    public int RegionCount { get; init; } // Number of regions in this epic
    public int ReadersCount { get; init; } // Total readers across all stories in epic
    public double AverageRating { get; init; } // Average rating across all stories in epic
    public List<string> AvailableLanguages { get; init; } = new(); // e.g., ["ro-ro", "en-us", "hu-hu"]
}

public record EpicDetailsDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CoverImageUrl { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? CreatedByName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    public int StoryCount { get; init; }
    public int RegionCount { get; init; }
    public int ReadersCount { get; init; }
    public double AverageRating { get; init; } // From epic reviews, not story reviews
    public int TotalReviews { get; init; } // From epic reviews, not story reviews
    public EpicReviewDto? UserReview { get; init; } // Current user's review if exists
    public List<string> AvailableLanguages { get; init; } = new(); // e.g., ["ro-ro", "en-us", "hu-hu"]
}

public record GetMarketplaceEpicsResponse
{
    public List<EpicMarketplaceItemDto> Epics { get; init; } = new();
    public int TotalCount { get; init; }
    public bool HasMore { get; init; }
}

public record SearchEpicsRequest
{
    public string? SearchTerm { get; init; }
    public string SearchType { get; init; } = "title"; // "title", "author"
    public string SortBy { get; init; } = "publishedAt"; // publishedAt, name, readers, rating
    public string SortOrder { get; init; } = "desc"; // asc, desc
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

// Epic Review DTOs
public record EpicReviewDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? UserPicture { get; init; }
    public string EpicId { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsOwnReview { get; init; } // True if this is the current user's review
}

public record CreateEpicReviewRequest
{
    public required string EpicId { get; init; }
    public required int Rating { get; init; } // 1-5
    public string? Comment { get; init; }
}

public record CreateEpicReviewResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public EpicReviewDto? Review { get; init; }
}

public record UpdateEpicReviewRequest
{
    public required Guid ReviewId { get; init; }
    public required int Rating { get; init; } // 1-5
    public string? Comment { get; init; }
}

public record UpdateEpicReviewResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public EpicReviewDto? Review { get; init; }
}

public record DeleteEpicReviewResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record GetEpicReviewsRequest
{
    public required string EpicId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; } = "createdAt"; // createdAt, rating
    public string SortOrder { get; init; } = "desc"; // asc, desc
}

public record GetEpicReviewsResponse
{
    public List<EpicReviewDto> Reviews { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public bool HasMore { get; init; }
    public double AverageRating { get; init; }
    public Dictionary<int, int> RatingDistribution { get; init; } = new(); // Rating -> Count
}


