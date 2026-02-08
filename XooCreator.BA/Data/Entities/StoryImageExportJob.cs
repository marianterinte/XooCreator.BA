namespace XooCreator.BA.Data.Entities;

public class StoryImageExportJob
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public string Locale { get; set; } = "ro-ro";
    /// <summary>Image generation provider: "Google" (Nano Banana) or "OpenAI". Default Google for now.</summary>
    public string Provider { get; set; } = "Google";
    public string Status { get; set; } = StoryImageExportJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ZipBlobPath { get; set; }
    public string? ZipFileName { get; set; }
    public long? ZipSizeBytes { get; set; }
    public int? ImageCount { get; set; }
    public string? SelectedTileIdsJson { get; set; }
    public string? ApiKeyOverride { get; set; }
}

public static class StoryImageExportJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
