namespace XooCreator.BA.Data;

/// <summary>
/// Represents a hero that can be unlocked through story completion
/// </summary>
public class StoryHero
{
    public Guid Id { get; set; }
    public string HeroId { get; set; } = string.Empty; // Reference to HeroDefinition.Id
    public string ImageUrl { get; set; } = string.Empty; // Image URL for the hero
    public string UnlockConditionJson { get; set; } = string.Empty; // JSON with unlock conditions
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public List<StoryHeroUnlock> StoryUnlocks { get; set; } = new();
}

/// <summary>
/// Represents the relationship between a story and a story hero unlock
/// </summary>
public class StoryHeroUnlock
{
    public Guid Id { get; set; }
    public Guid StoryHeroId { get; set; }
    public string StoryId { get; set; } = string.Empty; // Reference to StoryDefinition.StoryId
    public int UnlockOrder { get; set; } = 1; // Order of unlock within the story
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public StoryHero StoryHero { get; set; } = null!;
}
