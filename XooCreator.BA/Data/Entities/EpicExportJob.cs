namespace XooCreator.BA.Data.Entities;

public class EpicExportJob
{
    public Guid Id { get; set; }
    public string EpicId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public string Locale { get; set; } = "ro-ro";
    public bool IsDraft { get; set; }
    public string Status { get; set; } = EpicExportJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    // Export options
    public bool IncludeStories { get; set; } = true;
    public bool IncludeHeroes { get; set; } = true;
    public bool IncludeRegions { get; set; } = true;
    public bool IncludeImages { get; set; } = true;
    public bool IncludeAudio { get; set; } = true;
    public bool IncludeVideo { get; set; } = true;
    public bool IncludeProgress { get; set; } = false;
    public string? LanguageFilter { get; set; }

    // Progress tracking
    public string? CurrentPhase { get; set; }
    public int StoriesExported { get; set; }
    public int TotalStories { get; set; }
    public int HeroesExported { get; set; }
    public int TotalHeroes { get; set; }
    public int RegionsExported { get; set; }
    public int TotalRegions { get; set; }
    public int AssetsExported { get; set; }
    public int TotalAssets { get; set; }

    // Results
    public string? ErrorMessage { get; set; }
    public string? ZipBlobPath { get; set; } // Path to the generated ZIP in blob storage
    public string? ZipFileName { get; set; }
    public long? ZipSizeBytes { get; set; }
}

public static class EpicExportJobStatus
{
    public const string Queued = "Queued";
    public const string Preparing = "Preparing";
    public const string PackagingStories = "PackagingStories";
    public const string PackagingHeroes = "PackagingHeroes";
    public const string PackagingRegions = "PackagingRegions";
    public const string Finalizing = "Finalizing";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}