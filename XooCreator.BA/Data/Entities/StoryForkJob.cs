namespace XooCreator.BA.Data.Entities;

public class StoryForkJob
{
    public Guid Id { get; set; }
    public string SourceStoryId { get; set; } = string.Empty;
    public string SourceType { get; set; } = StoryForkAssetJobSourceTypes.Draft;
    public bool CopyAssets { get; set; }
    public Guid RequestedByUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public Guid TargetOwnerUserId { get; set; }
    public string TargetOwnerEmail { get; set; } = string.Empty;
    public string TargetStoryId { get; set; } = string.Empty;
    public string Status { get; set; } = StoryForkJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public string? WarningSummary { get; set; }
    public Guid? AssetJobId { get; set; }
    public string? AssetJobStatus { get; set; }
    public int SourceTranslations { get; set; }
    public int SourceTiles { get; set; }
}

public static class StoryForkJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}

