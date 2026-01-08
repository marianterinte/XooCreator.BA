namespace XooCreator.BA.Data;

/// <summary>
/// Tracks story likes per user.
/// Only users who have read the story can like it.
/// </summary>
public class StoryLike
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;

    public AlchimaliaUser User { get; set; } = null!;
}

