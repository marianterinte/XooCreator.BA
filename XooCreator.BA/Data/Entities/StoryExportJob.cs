namespace XooCreator.BA.Data.Entities;

public class StoryExportJob
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public string Locale { get; set; } = "ro-ro";
    public bool IsDraft { get; set; }
    public string Status { get; set; } = StoryExportJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ZipBlobPath { get; set; } // Path to the generated ZIP in blob storage
    public string? ZipFileName { get; set; }
    public long? ZipSizeBytes { get; set; }
    public int? MediaCount { get; set; }
    public int? LanguageCount { get; set; }
}

public static class StoryExportJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
