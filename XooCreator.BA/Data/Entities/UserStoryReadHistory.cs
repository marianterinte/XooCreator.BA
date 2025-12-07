namespace XooCreator.BA.Data;

/// <summary>
/// Represents a permanent history record of a user's reading progress for a story.
/// This table preserves reading history even after progress is reset.
/// Stores aggregate progress (total tiles read, percentage) rather than individual tile records.
/// </summary>
public class UserStoryReadHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty; // e.g., "root-s1"
    public int TotalTilesRead { get; set; } // Number of tiles read
    public int TotalTiles { get; set; } // Total number of tiles in the story
    public DateTime LastReadAt { get; set; } = DateTime.UtcNow; // When the story was last read
    public DateTime? CompletedAt { get; set; } // When the story was completed (set when progress is reset after completion)
    
    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
}

