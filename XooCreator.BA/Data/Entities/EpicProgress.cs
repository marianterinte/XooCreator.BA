namespace XooCreator.BA.Data;

/// <summary>
/// Represents a user's progress for a region in a Story Epic
/// </summary>
public class EpicProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RegionId { get; set; } = string.Empty; // Region ID from StoryEpicRegion
    public bool IsUnlocked { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK to StoryEpic
    public DateTime UnlockedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public DbStoryEpic Epic { get; set; } = null!;
    public AlchimaliaUser User { get; set; } = null!;
}

