namespace XooCreator.BA.Data.Entities;

public class StoryForkAssetJob
{
    public Guid Id { get; set; }
    public string SourceStoryId { get; set; } = string.Empty;
    public string SourceType { get; set; } = StoryForkAssetJobSourceTypes.Draft;
    public Guid? SourceOwnerUserId { get; set; }
    public string? SourceOwnerEmail { get; set; }
    public string TargetStoryId { get; set; } = string.Empty;
    public Guid TargetOwnerUserId { get; set; }
    public string TargetOwnerEmail { get; set; } = string.Empty;
    public string RequestedByEmail { get; set; } = string.Empty;
    public string Status { get; set; } = StoryForkAssetJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public int AttemptedAssets { get; set; }
    public int CopiedAssets { get; set; }
    public string? ErrorMessage { get; set; }
    public string? WarningSummary { get; set; }
}

public static class StoryForkAssetJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}

public static class StoryForkAssetJobSourceTypes
{
    public const string Draft = "draft";
    public const string Published = "published";
}

