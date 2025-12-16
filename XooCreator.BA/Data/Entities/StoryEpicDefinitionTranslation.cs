using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for StoryEpicDefinition (Name, Description per language)
/// Similar to StoryDefinitionTranslation pattern
/// </summary>
public class StoryEpicDefinitionTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string StoryEpicDefinitionId { get; set; } // FK to StoryEpicDefinition.Id (string, not Guid)
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case, e.g., "ro-ro", "en-us"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Translated epic name
    
    [MaxLength(1000)]
    public string? Description { get; set; } // Optional description in this language
    
    // Navigation property
    public StoryEpicDefinition StoryEpicDefinition { get; set; } = null!;
}

