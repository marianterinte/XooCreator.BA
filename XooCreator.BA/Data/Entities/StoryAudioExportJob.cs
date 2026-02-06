namespace XooCreator.BA.Data.Entities;

public class StoryAudioExportJob
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public string Locale { get; set; } = "ro-ro";
    public string Status { get; set; } = StoryAudioExportJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ZipBlobPath { get; set; }
    public string? ZipFileName { get; set; }
    public long? ZipSizeBytes { get; set; }
    public int? AudioCount { get; set; }
    public string? SelectedTileIdsJson { get; set; } // JSON array of selected tile GUIDs, null = all pages
    public string? ApiKeyOverride { get; set; } // Optional API key from UI, null = use default from config
}

public static class StoryAudioExportJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
