namespace XooCreator.BA.Data;

/// <summary>
/// Represents a user's progress through reading a specific story tile
/// </summary>
public class UserStoryReadProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty; // e.g., "root-s1"
    public string TileId { get; set; } = string.Empty; // e.g., "p3"
    public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public UserAlchimalia User { get; set; } = null!;
}
