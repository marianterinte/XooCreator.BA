namespace XooCreator.BA.Data.Entities;

public class AnimalVersionJob
{
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public int BaseVersion { get; set; }
    public string Status { get; set; } = AnimalVersionJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}

public static class AnimalVersionJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Superseded = "Superseded";
}
