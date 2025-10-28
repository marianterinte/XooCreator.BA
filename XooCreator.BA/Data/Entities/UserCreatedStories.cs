namespace XooCreator.BA.Data;

/// <summary>
/// Represents a story created by a user
/// </summary>
public class UserCreatedStories
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StoryDefinitionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; } = false;
    public string? CreationNotes { get; set; }
    
    // Navigation properties
    public AlchimaliaUser User { get; set; } = null!;
    public StoryDefinition StoryDefinition { get; set; } = null!;
}
