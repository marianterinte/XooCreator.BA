namespace XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

/// <summary>
/// Translation data for an EpicHero in a specific language
/// </summary>
public record EpicHeroTranslationDto
{
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? GreetingText { get; init; }
    public string? GreetingAudioUrl { get; init; } // Audio URL per language
}

/// <summary>
/// Full EpicHero DTO with all translations
/// </summary>
public record EpicHeroDto
{
    public required string Id { get; init; }
    public string? ImageUrl { get; init; }
    public string? GreetingAudioUrl { get; init; } // Common for all languages
    public string Status { get; init; } = "draft";
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }

    /// <summary>Topic IDs (e.g. alchimalia_universe, classic_author) for tagging.</summary>
    public List<string> TopicIds { get; init; } = new();

    // Review workflow fields (similar to StoryCraft/EditableStoryDto)
    public Guid? AssignedReviewerUserId { get; init; }
    public Guid? ReviewedByUserId { get; init; }
    public Guid? ApprovedByUserId { get; init; }
    public string? ReviewNotes { get; init; }
    public DateTime? ReviewStartedAt { get; init; }
    public DateTime? ReviewEndedAt { get; init; }

    // Translations per language
    public List<EpicHeroTranslationDto> Translations { get; init; } = new();
    
    // Helper: Get name in a specific language (falls back to first available)
    public string GetName(string languageCode)
    {
        var translation = Translations.FirstOrDefault(t => t.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
        return translation?.Name ?? Translations.FirstOrDefault()?.Name ?? string.Empty;
    }
    
    // Helper: Get description in a specific language
    public string? GetDescription(string languageCode)
    {
        var translation = Translations.FirstOrDefault(t => t.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
        return translation?.Description ?? Translations.FirstOrDefault()?.Description;
    }
    
    // Helper: Get greeting text in a specific language
    public string? GetGreetingText(string languageCode)
    {
        var translation = Translations.FirstOrDefault(t => t.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
        return translation?.GreetingText ?? Translations.FirstOrDefault()?.GreetingText;
    }
    
    // Helper: Get greeting audio URL in a specific language
    public string? GetGreetingAudioUrl(string languageCode)
    {
        var translation = Translations.FirstOrDefault(t => t.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
        return translation?.GreetingAudioUrl ?? Translations.FirstOrDefault()?.GreetingAudioUrl;
    }
}

/// <summary>
/// EpicHero list item with single language (default or specified)
/// </summary>
public record EpicHeroListItemDto
{
    public required string Id { get; init; }
    public required string Name { get; init; } // Name in the requested/default language
    public string? ImageUrl { get; init; }
    public string? GreetingText { get; init; } // Greeting text in the requested/default language
    public string? GreetingAudioUrl { get; init; }
    public string Status { get; init; } = "draft";
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }

    public List<string> TopicIds { get; init; } = new();

    // Review workflow fields for list display
    public Guid? AssignedReviewerUserId { get; init; }
    public bool IsAssignedToCurrentUser { get; init; } // Computed in service based on current user
    public bool IsOwnedByCurrentUser { get; init; } // Computed in service based on current user
    public string? OwnerEmail { get; init; } // Owner's email (for admin filtering)
}

