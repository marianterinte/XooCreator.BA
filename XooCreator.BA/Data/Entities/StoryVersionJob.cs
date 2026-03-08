namespace XooCreator.BA.Data.Entities;

public class StoryVersionJob
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public int BaseVersion { get; set; }  // Versiunea publicată de la care se creează draft-ul
    public bool LightChanges { get; set; }
    /// <summary>Copy cover + tile images when LightChanges is false. Null = legacy, treat as true.</summary>
    public bool? CopyImages { get; set; }
    /// <summary>Copy audio (tile + dialog) when LightChanges is false. Null = legacy, treat as true.</summary>
    public bool? CopyAudio { get; set; }
    /// <summary>Copy video when LightChanges is false. Null = legacy, treat as true.</summary>
    public bool? CopyVideo { get; set; }
    /// <summary>'all' or 'selected'. Null = legacy, treat as 'all'.</summary>
    public string? LanguageMode { get; set; }
    /// <summary>JSON array of language codes when LanguageMode is 'selected'. Null or empty = none.</summary>
    public string? SelectedLanguagesJson { get; set; }
    public string Status { get; set; } = StoryVersionJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
}

public static class StoryVersionJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Superseded = "Superseded";
}

