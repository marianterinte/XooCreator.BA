using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents a story hero that can be unlocked through story completion (Tree of Heroes system)
/// Note: This is different from EpicHero which is for Heroes Management in Story Editor
/// </summary>
public class StoryHero
{
    public Guid Id { get; set; } // Primary key (Guid, not string)
    
    [MaxLength(100)]
    public required string HeroId { get; set; } = string.Empty; // e.g., "puf-puf", "linkaro", "grubot"
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; } // Hero image URL
    
    [MaxLength(2000)]
    public string? UnlockConditionJson { get; set; } // JSON with unlock conditions
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<StoryHeroUnlock> StoryUnlocks { get; set; } = new List<StoryHeroUnlock>();
}
