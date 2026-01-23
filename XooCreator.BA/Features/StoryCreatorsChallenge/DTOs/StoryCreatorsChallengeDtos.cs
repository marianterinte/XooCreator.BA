namespace XooCreator.BA.Features.StoryCreatorsChallenge.DTOs;

public record StoryCreatorsChallengeTranslationDto
{
    public required string LanguageCode { get; init; }
    public required string Topic { get; init; }
    public string? Description { get; init; } // General description of the challenge
}

public record ChallengeItemRewardDto
{
    public required string TokenType { get; init; } // TokenFamily enum value as string
    public required string TokenValue { get; init; }
    public required int Quantity { get; init; }
    public int SortOrder { get; init; } = 0;
}

public record ChallengeItemTranslationDto
{
    public required string LanguageCode { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; } // Item description
}

public record ChallengeItemDto
{
    public required string ItemId { get; init; }
    public int SortOrder { get; init; } = 0;
    public List<ChallengeItemTranslationDto> Translations { get; init; } = new();
    public List<ChallengeItemRewardDto> Rewards { get; init; } = new();
}

public record StoryCreatorsChallengeDto
{
    public required string ChallengeId { get; init; }
    public string Status { get; init; } = "active";
    public int SortOrder { get; init; } = 0;
    public DateTime? EndDate { get; init; } // End date (optional)
    public string? CoverImageUrl { get; init; } // Challenge cover image URL
    public string? CoverImageRelPath { get; init; } // Relative path in blob storage
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<StoryCreatorsChallengeTranslationDto> Translations { get; init; } = new();
    public List<ChallengeItemDto> Items { get; init; } = new();
}

// For list view (admin dashboard)
public record StoryCreatorsChallengeListItemDto
{
    public required string ChallengeId { get; init; }
    public string Status { get; init; } = "active";
    public int SortOrder { get; init; } = 0;
    public DateTime? EndDate { get; init; } // End date
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string Topic { get; init; } = string.Empty; // Topic in current language
    public int ItemsCount { get; init; } = 0;
    public bool IsExpired { get; init; } // Helper - computed from EndDate
    public int SubscriptionsCount { get; init; } = 0; // Number of users subscribed
    public int SubmissionsCount { get; init; } = 0; // Number of story submissions
}

// For public API (CCC page)
public record PublicChallengeDto
{
    public required string ChallengeId { get; init; }
    public required string Topic { get; init; } // In current language
    public string? Description { get; init; } // General description
    public DateTime? EndDate { get; init; } // End date (visible on public page)
    public bool IsExpired { get; init; } // Helper - computed from EndDate
    public string? CoverImageUrl { get; init; } // Challenge cover image URL
    public List<PublicChallengeItemDto> Items { get; init; } = new();
}

public record PublicChallengeItemDto
{
    public required string ItemId { get; init; }
    public required string Title { get; init; } // In current language
    public string? Description { get; init; } // Item description
    public List<ChallengeItemRewardDto> Rewards { get; init; } = new();
}

// Subscription DTOs
public record ChallengeSubscriptionDto
{
    public required string ChallengeId { get; init; }
    public Guid UserId { get; init; }
    public DateTime SubscribedAt { get; init; }
    public bool IsSubscribed { get; init; } // Helper for current user
}

// Submission DTOs
public record ChallengeSubmissionDto
{
    public required string ChallengeId { get; init; }
    public required string StoryId { get; init; }
    public Guid UserId { get; init; }
    public string? StoryTitle { get; init; } // From StoryDefinition
    public string? StoryCoverImageUrl { get; init; } // From StoryDefinition
    public DateTime SubmittedAt { get; init; }
    public int LikesCount { get; init; }
    public bool IsWinner { get; init; }
    public bool IsCurrentUserSubmission { get; init; } // Helper
}

// Leaderboard DTO
public record ChallengeLeaderboardDto
{
    public required string ChallengeId { get; init; }
    public List<ChallengeSubmissionDto> Submissions { get; init; } = new(); // Ordered by LikesCount DESC
    public ChallengeSubmissionDto? Winner { get; init; } // Top submission or IsWinner=true
}

// Public leaderboard (for CCC page)
public record PublicChallengeLeaderboardDto
{
    public required string ChallengeId { get; init; }
    public List<PublicSubmissionDto> Submissions { get; init; } = new();
    public PublicSubmissionDto? Winner { get; init; }
}

public record PublicSubmissionDto
{
    public required string StoryId { get; init; }
    public required string StoryTitle { get; init; }
    public string? StoryCoverImageUrl { get; init; }
    public int LikesCount { get; init; }
    public bool IsWinner { get; init; }
}
