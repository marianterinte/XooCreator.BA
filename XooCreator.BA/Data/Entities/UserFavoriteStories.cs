namespace XooCreator.BA.Data;

/// <summary>
/// Represents a story marked as favorite by a user
/// </summary>
public class UserFavoriteStories
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StoryDefinitionId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public AlchimaliaUser User { get; set; } = null!;
    public StoryDefinition StoryDefinition { get; set; } = null!;
}

