using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for StoryHero (Name, Description, GreetingText, GreetingAudioUrl per language)
/// Similar to StoryRegionTranslation pattern
/// </summary>
public class StoryHeroTranslation
{
    public Guid Id { get; set; }
    
    public Guid StoryHeroId { get; set; } // FK to StoryHero.Id (Guid)
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "en-us"; // normalized lower-case, e.g., "en-us", "ro-ro", "hu-hu"
    
    [MaxLength(255)]
    public required string Name { get; set; } = string.Empty; // Translated hero name
    
    public string? Description { get; set; } // Optional description in this language
    
    public string? GreetingText { get; set; } // Translated greeting text in this language
    
    [MaxLength(500)]
    public string? GreetingAudioUrl { get; set; } // URL pentru audio greeting (per limbă)
    
    // Navigation property
    public StoryHero StoryHero { get; set; } = null!;
}
