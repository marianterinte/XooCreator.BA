namespace XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

/// <summary>
/// Translation data for a StoryRegion in a specific language
/// </summary>
public record StoryRegionTranslationDto
{
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Full StoryRegion DTO with all translations
/// </summary>
public record StoryRegionDto
{
    public required string Id { get; init; }
    public string? ImageUrl { get; init; }
    public string Status { get; init; } = "draft";
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    
    // Review workflow fields (similar to StoryCraft/EditableStoryDto)
    public Guid? AssignedReviewerUserId { get; init; }
    public Guid? ReviewedByUserId { get; init; }
    public Guid? ApprovedByUserId { get; init; }
    public string? ReviewNotes { get; init; }
    public DateTime? ReviewStartedAt { get; init; }
    public DateTime? ReviewEndedAt { get; init; }
    
    // Translations per language
    public List<StoryRegionTranslationDto> Translations { get; init; } = new();
    
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
}

/// <summary>
/// StoryRegion list item with single language (default or specified)
/// </summary>
public record StoryRegionListItemDto
{
    public required string Id { get; init; }
    public required string Name { get; init; } // Name in the requested/default language
    public string? ImageUrl { get; init; }
    public string Status { get; init; } = "draft";
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    
    // Review workflow fields for list display
    public Guid? AssignedReviewerUserId { get; init; }
    public bool IsAssignedToCurrentUser { get; init; } // Computed in service based on current user
    public bool IsOwnedByCurrentUser { get; init; } // Computed in service based on current user
    public string? OwnerEmail { get; init; } // Owner's email (for admin filtering)
}

