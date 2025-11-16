namespace XooCreator.BA.Data;

/// <summary>
/// Represents a user review for a story
/// </summary>
public class StoryReview
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; } // Optional review text
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true; // For soft delete
    
    // Navigation properties
    public AlchimaliaUser User { get; set; } = null!;
    public StoryDefinition Story { get; set; } = null!;
}

