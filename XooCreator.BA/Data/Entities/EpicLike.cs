namespace XooCreator.BA.Data;

/// <summary>
/// Tracks epic likes per user.
/// </summary>
public class EpicLike
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EpicId { get; set; } = string.Empty;
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;

    public AlchimaliaUser User { get; set; } = null!;
}
