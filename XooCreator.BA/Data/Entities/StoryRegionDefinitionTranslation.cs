using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for StoryRegionDefinition (Name, Description per language)
/// Similar to StoryRegionTranslation but for definition (published)
/// </summary>
public class StoryRegionDefinitionTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string StoryRegionDefinitionId { get; set; } // FK to StoryRegionDefinition.Id (string, not Guid)
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case, e.g., "ro-ro", "en-us"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Translated region name
    
    public string? Description { get; set; } // Optional description in this language
    
    // Navigation property
    public StoryRegionDefinition StoryRegionDefinition { get; set; } = null!;
}

