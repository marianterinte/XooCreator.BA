using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

/// <summary>
/// Tracks unique readers (downloads) per epic.
/// </summary>
public class EpicReader
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK to StoryEpicDefinition
    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
    public EpicAcquisitionSource AcquisitionSource { get; set; } = EpicAcquisitionSource.Free;

    public AlchimaliaUser User { get; set; } = null!;
    public StoryEpicDefinition Epic { get; set; } = null!;
}

