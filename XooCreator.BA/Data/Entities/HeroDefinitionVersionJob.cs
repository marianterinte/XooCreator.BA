namespace XooCreator.BA.Data.Entities;

public class HeroDefinitionVersionJob
{
    public Guid Id { get; set; }
    public string HeroId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public int BaseVersion { get; set; }
    public string Status { get; set; } = HeroDefinitionVersionJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}

public static class HeroDefinitionVersionJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Superseded = "Superseded";
}
