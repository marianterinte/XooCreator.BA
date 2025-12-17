using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for EpicHeroDefinition (Name, Description, GreetingText per language)
/// Similar to EpicHeroTranslation but for definition (published)
/// </summary>
public class EpicHeroDefinitionTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string EpicHeroDefinitionId { get; set; } // FK to EpicHeroDefinition.Id (string, not Guid)
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case, e.g., "ro-ro", "en-us"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Translated hero name
    
    public string? Description { get; set; } // Optional description in this language
    
    [MaxLength(1000)]
    public string? GreetingText { get; set; } // Translated greeting message
    
    [MaxLength(1000)]
    public string? GreetingAudioUrl { get; set; } // Audio URL for greeting in this language
    
    // Navigation property
    public EpicHeroDefinition EpicHeroDefinition { get; set; } = null!;
}

