namespace XooCreator.BA.Data;

/// <summary>
/// Represents an unlocked hero in the user's bestiary
/// </summary>
public class HeroProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string HeroId { get; set; } = string.Empty; // e.g., "LEO", "FOX", "OWL"
    public string HeroType { get; set; } = string.Empty; // e.g., "STORY_REWARD", "HERO_TREE_UNLOCK"
    public string SourceStoryId { get; set; } = string.Empty; // If from story completion
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User User { get; set; } = null!;
}
