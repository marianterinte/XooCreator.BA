using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents the unlock condition for a StoryHero (links stories to heroes)
/// This is for the Tree of Heroes system, not for Heroes Management in Story Editor
/// </summary>
public class StoryHeroUnlock
{
    public Guid Id { get; set; }
    
    public Guid StoryHeroId { get; set; } // FK to StoryHero.Id (Guid)
    
    [MaxLength(100)]
    public required string StoryId { get; set; } // Story that unlocks this hero
    
    public int UnlockOrder { get; set; } // Order in which stories must be completed
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public StoryHero StoryHero { get; set; } = null!;
}
