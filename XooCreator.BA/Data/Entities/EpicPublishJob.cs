namespace XooCreator.BA.Data.Entities;

public class EpicPublishJob
{
    public Guid Id { get; set; }
    public string EpicId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public string LangTag { get; set; } = "ro-ro";
    public int DraftVersion { get; set; }
    public bool ForceFull { get; set; }
    public string Status { get; set; } = EpicPublishJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}

public static class EpicPublishJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Superseded = "Superseded";
}

