namespace XooCreator.BA.Data;

/// <summary>
/// Represents a tree progress node/region in the Tree of Light
/// </summary>
public class TreeProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RegionId { get; set; } = string.Empty; // e.g., "root", "farm", "sahara"
    public bool IsUnlocked { get; set; }
    public DateTime UnlockedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public UserAlchimalia User { get; set; } = null!;
}
