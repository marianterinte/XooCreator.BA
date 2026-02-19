namespace XooCreator.BA.Data.Entities;

public class StoryAudioImportJob
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    /// <summary>Email used for draft blob path (owner or requester).</summary>
    public string OwnerEmail { get; set; } = string.Empty;
    public string Locale { get; set; } = "ro-ro";
    /// <summary>Blob path where the uploaded ZIP is stored (draft container). Null when using batch file import flow.</summary>
    public string? ZipBlobPath { get; set; }
    /// <summary>Staging prefix for per-file direct uploads (batch import flow).</summary>
    public string? StagingPrefix { get; set; }
    /// <summary>JSON payload that stores staged files and optional manual mapping overrides.</summary>
    public string? BatchMappingJson { get; set; }
    public string Status { get; set; } = StoryAudioImportJobStatus.Queued;
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Success { get; set; }
    public int ImportedCount { get; set; }
    public int TotalPages { get; set; }
    /// <summary>JSON array of error strings.</summary>
    public string? ErrorsJson { get; set; }
    /// <summary>JSON array of warning strings.</summary>
    public string? WarningsJson { get; set; }
}

public static class StoryAudioImportJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
