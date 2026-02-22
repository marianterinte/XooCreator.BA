namespace XooCreator.BA.Data.Entities;

public class StoryImportJob
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public string OriginalStoryId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public string Locale { get; set; } = "ro-ro";
    public string? ZipBlobPath { get; set; }
    /// <summary>When set, worker loads manifest and assets from staging instead of ZIP. e.g. imports/{userId}/temp/{uploadId}.</summary>
    public string? StagingPrefix { get; set; }
    /// <summary>Path to manifest.json in blob when using staging. e.g. imports/{userId}/temp/{uploadId}/manifest.json.</summary>
    public string? ManifestBlobPath { get; set; }
    public string ZipFileName { get; set; } = string.Empty;
    public long ZipSizeBytes { get; set; }
    public string Status { get; set; } = StoryImportJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public int ImportedAssets { get; set; }
    public int TotalAssets { get; set; }
    public int ImportedLanguagesCount { get; set; }
    public string? ErrorMessage { get; set; }
    public string? WarningSummary { get; set; }
    public bool IncludeImages { get; set; } = true;
    public bool IncludeAudio { get; set; } = true;
    public bool IncludeVideo { get; set; } = true;
}

public static class StoryImportJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}

