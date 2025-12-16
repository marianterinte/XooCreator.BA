using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents the reference of an EpicHero within a StoryEpic
/// This is a junction entity that links published heroes to epics
/// </summary>
public class StoryEpicHeroReference
{
    public int Id { get; set; }
    
    [MaxLength(100)]
    public required string EpicId { get; set; } = string.Empty; // FK to StoryEpicDefinition
    
    [MaxLength(100)]
    public required string HeroId { get; set; } = string.Empty; // FK to EpicHero (must be published)
    
    [MaxLength(200)]
    public string? StoryId { get; set; } // Story care deblochează acest hero (opțional)
    
    // Navigation properties
    public StoryEpicDefinition Epic { get; set; } = null!;
    public EpicHero Hero { get; set; } = null!;
}

