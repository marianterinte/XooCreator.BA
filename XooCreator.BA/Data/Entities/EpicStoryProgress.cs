namespace XooCreator.BA.Data;

/// <summary>
/// Represents a completed story in a Story Epic
/// </summary>
public class EpicStoryProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty; // Story ID from StoryEpicStoryNode
    public string? SelectedAnswer { get; set; } // The choice made by user
    public string? TokensJson { get; set; } // JSON serialized list of TokenReward objects
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string EpicId { get; set; } = string.Empty; // Can reference either StoryEpics (old) or StoryEpicDefinition (new architecture)
    
    // Navigation (ignored in EF Core - EpicId can reference either StoryEpics or StoryEpicDefinition)
    // public DbStoryEpic Epic { get; set; } = null!;
    public AlchimaliaUser User { get; set; } = null!;
}

