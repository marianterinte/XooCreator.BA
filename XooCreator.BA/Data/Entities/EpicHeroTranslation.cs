using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for EpicHero (Name, GreetingText per language)
/// Similar to StoryCraftTranslation pattern
/// </summary>
public class EpicHeroTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string EpicHeroId { get; set; } // FK to EpicHero.Id (string, not Guid)
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case, e.g., "ro-ro", "en-us"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Translated hero name
    
    [MaxLength(1000)]
    public string? GreetingText { get; set; } // Translated greeting message
    
    // Navigation property
    public EpicHero EpicHero { get; set; } = null!;
}

