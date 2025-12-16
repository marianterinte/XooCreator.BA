namespace XooCreator.BA.Data;

/// <summary>
/// Represents a user review for an epic
/// </summary>
public class EpicReview
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK to StoryEpicDefinition
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; } // Optional review text, max 2000 chars
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true; // For soft delete
    
    // Navigation properties
    public AlchimaliaUser User { get; set; } = null!;
    public StoryEpicDefinition Epic { get; set; } = null!;
}

