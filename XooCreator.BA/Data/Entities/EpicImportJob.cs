namespace XooCreator.BA.Data.Entities;

public class EpicImportJob
{
    public Guid Id { get; set; }
    public string EpicId { get; set; } = string.Empty; // New epic ID (after import)
    public string OriginalEpicId { get; set; } = string.Empty; // Original epic ID from the export
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    public string Locale { get; set; } = "ro-ro";
    public string ZipBlobPath { get; set; } = string.Empty;
    public string ZipFileName { get; set; } = string.Empty;
    public long ZipSizeBytes { get; set; }
    public string Status { get; set; } = EpicImportJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    // Import options
    public string ConflictStrategy { get; set; } = "skip"; // skip, replace, new_ids
    public bool IncludeStories { get; set; } = true;
    public bool IncludeHeroes { get; set; } = true;
    public bool IncludeRegions { get; set; } = true;
    public bool IncludeImages { get; set; } = true;
    public bool IncludeAudio { get; set; } = true;
    public bool IncludeVideo { get; set; } = true;
    public bool GenerateNewIds { get; set; } = false;
    public string? IdPrefix { get; set; }

    // Progress tracking (stored as JSON)
    public string? PhasesJson { get; set; } // Serialized ImportPhases
    public string? IdMappingsJson { get; set; } // Serialized ID mappings

    // Results
    public string? ErrorMessage { get; set; }
    public string? WarningsJson { get; set; } // Serialized warnings array
}

public static class EpicImportJobStatus
{
    public const string Queued = "Queued";
    public const string Analyzing = "Analyzing";
    public const string ImportingRegions = "ImportingRegions";
    public const string ImportingHeroes = "ImportingHeroes";
    public const string ImportingStories = "ImportingStories";
    public const string EstablishingRelationships = "EstablishingRelationships";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}

// Helper classes for JSON serialization
public class EpicImportPhases
{
    public PhaseStatus Validation { get; set; } = new();
    public ImportPhaseProgress Regions { get; set; } = new();
    public ImportPhaseProgress Heroes { get; set; } = new();
    public ImportPhaseProgress Stories { get; set; } = new();
    public ImportPhaseProgress Assets { get; set; } = new();
    public ImportPhaseProgress Relationships { get; set; } = new();
}

public class PhaseStatus
{
    public string Status { get; set; } = "pending";
    public List<string> Errors { get; set; } = new();
}

public class ImportPhaseProgress
{
    public string Status { get; set; } = "pending";
    public int Imported { get; set; }
    public int Total { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class EpicIdMappings
{
    public Dictionary<string, string> Regions { get; set; } = new();
    public Dictionary<string, string> Heroes { get; set; } = new();
    public Dictionary<string, string> Stories { get; set; } = new();
}