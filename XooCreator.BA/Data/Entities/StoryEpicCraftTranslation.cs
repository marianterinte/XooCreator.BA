using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for StoryEpicCraft (Name, Description per language)
/// Similar to StoryCraftTranslation pattern
/// </summary>
public class StoryEpicCraftTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string StoryEpicCraftId { get; set; } // FK to StoryEpicCraft.Id (string, not Guid)
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case, e.g., "ro-ro", "en-us"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Translated epic name
    
    [MaxLength(1000)]
    public string? Description { get; set; } // Optional description in this language
    
    // Navigation property
    public StoryEpicCraft StoryEpicCraft { get; set; } = null!;
}

