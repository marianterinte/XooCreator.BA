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
    public string? TokensJson { get; set; } // JSON serialized list of TokenReward objects
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string TreeConfigurationId { get; set; } = string.Empty;
    
    // Navigation
    public TreeConfiguration TreeConfiguration { get; set; } = null!;
    public AlchimaliaUser User { get; set; } = null!;
}
