using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for StoryRegionCraft (Name, Description per language)
/// Similar to StoryRegionTranslation but for craft
/// </summary>
public class StoryRegionCraftTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string StoryRegionCraftId { get; set; } // FK to StoryRegionCraft.Id (string, not Guid)
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case, e.g., "ro-ro", "en-us"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Translated region name
    
    public string? Description { get; set; } // Optional description in this language
    
    // Navigation property
    public StoryRegionCraft StoryRegionCraft { get; set; } = null!;
}

