namespace XooCreator.BA.Data;

/// <summary>
/// Stores evaluation results for a user's completion of an evaluative story.
/// One record per story completion attempt. All attempts are stored (no limit).
/// Best score tracking will be added later in parent dashboard.
/// </summary>
public class StoryEvaluationResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid SessionId { get; set; } // Links to StoryQuizAnswer records
    
    // Score calculation
    public int TotalQuizzes { get; set; }        // Total number of quiz tiles in story
    public int CorrectAnswers { get; set; }      // Number of correct answers
    public int ScorePercentage { get; set; }     // 0-100: (CorrectAnswers / TotalQuizzes) * 100
    
    // Completion tracking
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
}

