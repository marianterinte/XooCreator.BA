namespace XooCreator.BA.Data;

/// <summary>
/// Tracks individual quiz answers submitted by users during story reading.
/// One record per quiz question answered.
/// </summary>
public class StoryQuizAnswer
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty; // e.g., "learn-math-s1"
    public string TileId { get; set; } = string.Empty;  // e.g., "q1", "q2"
    public string SelectedAnswerId { get; set; } = string.Empty; // e.g., "a", "b", "c"
    public bool IsCorrect { get; set; } // Calculated: true if selected answer has isCorrect=true
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    
    // Session tracking (for retries)
    public Guid? SessionId { get; set; } // Groups answers from same story reading session
    
    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
}

