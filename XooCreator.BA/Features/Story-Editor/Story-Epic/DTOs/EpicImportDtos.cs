using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

// Request DTOs
public record EpicImportRequest
{
    public string ConflictStrategy { get; init; } = "skip"; // skip, replace, new_ids
    public bool IncludeStories { get; init; } = true;
    public bool IncludeHeroes { get; init; } = true;
    public bool IncludeRegions { get; init; } = true;
    public bool IncludeImages { get; init; } = true;
    public bool IncludeAudio { get; init; } = true;
    public bool IncludeVideo { get; init; } = true;
    public bool GenerateNewIds { get; init; } = false;
    public string? IdPrefix { get; init; }
}

// Response DTOs
public record EpicImportResponse
{
    public Guid JobId { get; init; }
    public string Status { get; init; } = EpicImportJobStatus.Queued;
}

public record EpicImportJobStatusResponse
{
    public Guid JobId { get; init; }
    public string EpicId { get; init; } = string.Empty; // New epic ID
    public string OriginalEpicId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public EpicImportPhasesDto Phases { get; init; } = new();
    public EpicIdMappingsDto? IdMappings { get; init; }
    public List<string> Warnings { get; init; } = new();
    public List<string> Errors { get; init; } = new();
    public DateTime QueuedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
}

public record EpicImportPhasesDto
{
    public PhaseStatusDto Validation { get; init; } = new();
    public ImportPhaseProgressDto Regions { get; init; } = new();
    public ImportPhaseProgressDto Heroes { get; init; } = new();
    public ImportPhaseProgressDto Stories { get; init; } = new();
    public ImportPhaseProgressDto Assets { get; init; } = new();
    public ImportPhaseProgressDto Relationships { get; init; } = new();
}

public record PhaseStatusDto
{
    public string Status { get; init; } = "pending";
    public List<string> Errors { get; init; } = new();
}

public record ImportPhaseProgressDto
{
    public string Status { get; init; } = "pending";
    public int Imported { get; init; }
    public int Total { get; init; }
    public List<string> Errors { get; init; } = new();
}

public record EpicIdMappingsDto
{
    public Dictionary<string, string> Regions { get; init; } = new();
    public Dictionary<string, string> Heroes { get; init; } = new();
    public Dictionary<string, string> Stories { get; init; } = new();
}

// Preview DTOs
public record EpicImportPreviewRequest
{
    // No additional fields needed, ZIP file is sent as form data
}

public record EpicImportPreviewResponse
{
    public EpicInfoDto EpicInfo { get; init; } = new();
    public EpicContentsDto Contents { get; init; } = new();
    public EpicConflictsDto Conflicts { get; init; } = new();
    public List<string> ValidationWarnings { get; init; } = new();
}

public record EpicInfoDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public record EpicContentsDto
{
    public int RegionsCount { get; init; }
    public int HeroesCount { get; init; }
    public int StoriesCount { get; init; }
    public int AssetsCount { get; init; }
    public long TotalSizeBytes { get; init; }
    public List<string> Languages { get; init; } = new();
}

public record EpicConflictsDto
{
    public bool EpicIdExists { get; init; }
    public List<string> ExistingRegions { get; init; } = new();
    public List<string> ExistingHeroes { get; init; } = new();
    public List<string> ExistingStories { get; init; } = new();
}

// Validation Result DTOs
public record EpicImportValidationResult
{
    public bool IsValid { get; init; }
    public List<string> Errors { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
}

public record ConflictCheckResult
{
    public bool HasConflicts => EpicIdExists || ExistingRegions.Any() || ExistingHeroes.Any() || ExistingStories.Any();
    public bool EpicIdExists { get; set; }
    public List<string> ExistingRegions { get; set; } = new();
    public List<string> ExistingHeroes { get; set; } = new();
    public List<string> ExistingStories { get; set; } = new();
}