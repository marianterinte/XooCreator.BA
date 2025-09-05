namespace XooCreator.BA.Data;

/// <summary>
/// Represents a completed story in the Tree of Light
/// </summary>
public class StoryProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty; // e.g., "root-s1", "farm-s1"
    public string? SelectedAnswer { get; set; } // The choice made by user
    public string? RewardReceived { get; set; } // The reward name/id received
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User User { get; set; } = null!;
}
