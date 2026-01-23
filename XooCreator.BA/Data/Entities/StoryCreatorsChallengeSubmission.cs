using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Submission of a story to a challenge
/// </summary>
public class StoryCreatorsChallengeSubmission
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string ChallengeId { get; set; }
    
    [MaxLength(200)]
    public required string StoryId { get; set; } // StoryId (string, not FK)
    
    public Guid UserId { get; set; } // Creator who submitted
    
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    public int LikesCount { get; set; } = 0; // Computed from StoryLikes table
    
    public bool IsWinner { get; set; } = false; // Set by admin or automatically
    
    // Navigation properties
    public StoryCreatorsChallenge Challenge { get; set; } = null!;
    // public AlchimaliaUser User { get; set; } = null!; // Keeping typical pattern, often Users are not directly navigated if in different schema, but guide says yes.
}
