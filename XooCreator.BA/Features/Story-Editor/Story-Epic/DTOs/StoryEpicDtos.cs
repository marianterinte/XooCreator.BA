namespace XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

/// <summary>
/// Translation data for a StoryEpic in a specific language
/// </summary>
public record StoryEpicTranslationDto
{
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Co-author of an epic or story. Either UserId (Alchimalia user) or DisplayName (free text).
/// </summary>
public record EpicCoAuthorDto
{
    public Guid? Id { get; init; } // Optional for new items when saving
    public Guid? UserId { get; init; } // Set when co-author is an Alchimalia user
    public string DisplayName { get; init; } = string.Empty; // From User when UserId set; free text when UserId null
}

public record StoryEpicDto
{
    public required string Id { get; init; }
    public required string Name { get; init; } // Name in requested/default language
    public string? Description { get; init; } // Description in requested/default language
    public string? CoverImageUrl { get; init; }
    public string Status { get; init; } = "draft";
    public DateTime? PublishedAtUtc { get; init; }
    public List<StoryEpicRegionDto> Regions { get; init; } = new();
    public List<StoryEpicStoryNodeDto> Stories { get; init; } = new();
    public List<StoryEpicUnlockRuleDto> Rules { get; init; } = new();
    public List<StoryEpicHeroReferenceDto> Heroes { get; init; } = new(); // Hero references with unlock stories
    public List<StoryEpicTranslationDto> Translations { get; init; } = new(); // All translations
    public List<string> TopicIds { get; init; } = new(); // Topic IDs
    public List<string> AgeGroupIds { get; init; } = new(); // Age Group IDs
    public List<EpicCoAuthorDto> CoAuthors { get; init; } = new(); // Co-authors (user or free text)
    
    // Helper: Get name in a specific language (falls back to first available)
    public string GetName(string languageCode)
    {
        var translation = Translations.FirstOrDefault(t => t.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
        return translation?.Name ?? Translations.FirstOrDefault()?.Name ?? Name;
    }
    
    // Helper: Get description in a specific language
    public string? GetDescription(string languageCode)
    {
        var translation = Translations.FirstOrDefault(t => t.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
        return translation?.Description ?? Translations.FirstOrDefault()?.Description ?? Description;
    }
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
    public string? ToStoryId { get; init; }
    public List<string> RequiredStories { get; init; } = new();
    public int? MinCount { get; init; }
    public string? StoryId { get; init; }
    public int SortOrder { get; init; }
}

public record StoryEpicHeroReferenceDto
{
    public required string HeroId { get; init; }
    public string? StoryId { get; init; } // Story care deblochează hero-ul (opțional - null = available from start)
    // Optional: informații pentru UI (nu sunt salvate în DB, doar pentru display)
    public string? HeroName { get; init; }
    public string? HeroImageUrl { get; init; }
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
    public string? ImageUrl { get; init; } // For regions: region image; For stories: reward image (when not completed) or cover image (when completed)
    public string? CoverImageUrl { get; init; } // Story cover image (only for stories, used when story is completed)
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
    public string? OwnerEmail { get; init; } // Owner's email (for admin filtering)
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

public record UnlockedHeroDto
{
    public required string HeroId { get; init; }
    public required string ImageUrl { get; init; }
}

public record EpicProgressStateDto
{
    public List<EpicCompletedStoryDto> CompletedStories { get; init; } = new();
    public List<string> UnlockedRegions { get; init; } = new();
    public List<string> UnlockedStories { get; init; } = new();
    public List<UnlockedHeroDto> UnlockedHeroes { get; init; } = new();
}

// Complete Epic Story Request/Response DTOs
public record CompleteEpicStoryRequest
{
    public string? SelectedAnswer { get; init; }
    public List<TokenRewardDto>? Tokens { get; init; }
}

// Intermediate DTO for deserialization (type as string)
public record TokenRewardDto
{
    public string Type { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public int Quantity { get; init; }
}

public record CompleteEpicStoryResponse
{
    public required bool Success { get; init; }
    public required string EpicId { get; init; }
    public required string StoryId { get; init; }
    public List<string> NewlyUnlockedRegions { get; init; } = new();
    public List<UnlockedHeroDto> NewlyUnlockedHeroes { get; init; } = new();
    public string? StoryCoverImageUrl { get; init; }
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



