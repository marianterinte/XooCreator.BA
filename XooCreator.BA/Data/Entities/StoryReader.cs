using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

/// <summary>
/// Tracks unique readers (downloads) per story.
/// </summary>
public class StoryReader
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
    public StoryAcquisitionSource AcquisitionSource { get; set; } = StoryAcquisitionSource.FreeClaim;

    public AlchimaliaUser User { get; set; } = null!;
}

