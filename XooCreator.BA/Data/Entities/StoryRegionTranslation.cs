using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for StoryRegion (Name, Description per language)
/// Similar to StoryCraftTranslation pattern
/// </summary>
public class StoryRegionTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string StoryRegionId { get; set; } // FK to StoryRegion.Id (string, not Guid)
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case, e.g., "ro-ro", "en-us"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Translated region name
    
    public string? Description { get; set; } // Optional description in this language
    
    // Navigation property
    public StoryRegion StoryRegion { get; set; } = null!;
}

