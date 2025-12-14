namespace XooCreator.BA.Data;

/// <summary>
/// Represents an epic marked as favorite by a user
/// </summary>
public class UserFavoriteEpics
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK către DbStoryEpic.Id
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public AlchimaliaUser User { get; set; } = null!;
    public DbStoryEpic Epic { get; set; } = null!;
}

