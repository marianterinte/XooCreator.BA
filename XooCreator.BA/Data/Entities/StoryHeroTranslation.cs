using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for StoryHero (Name, Description, GreetingText per language)
/// Similar to StoryRegionTranslation pattern
/// </summary>
public class StoryHeroTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string StoryHeroId { get; set; } // FK to StoryHero.Id (string, not Guid)
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case, e.g., "ro-ro", "en-us"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Translated hero name
    
    public string? Description { get; set; } // Optional description in this language
    
    public string? GreetingText { get; set; } // Translated greeting text in this language
    
    // Navigation property
    public StoryHero StoryHero { get; set; } = null!;
}
