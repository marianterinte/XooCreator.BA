namespace XooCreator.BA.Data;

/// <summary>
/// Represents a message that a hero can say when interacting with a specific region
/// </summary>
public class HeroMessage
{
    public Guid Id { get; set; }
    public string HeroId { get; set; } = string.Empty; // Reference to StoryHero.HeroId
    public string RegionId { get; set; } = string.Empty; // Reference to region ID
    public string MessageKey { get; set; } = string.Empty; // Translation key for the message
    public string? AudioUrl { get; set; } // Optional audio file URL for the message
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents a message that a hero can say when clicked directly
/// </summary>
public class HeroClickMessage
{
    public Guid Id { get; set; }
    public string HeroId { get; set; } = string.Empty; // Reference to StoryHero.HeroId
    public string MessageKey { get; set; } = string.Empty; // Translation key for the message
    public string? AudioUrl { get; set; } // Optional audio file URL for the message
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
