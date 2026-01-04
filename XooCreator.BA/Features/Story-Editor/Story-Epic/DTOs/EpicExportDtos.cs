using System.Text.Json.Serialization;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

// Request DTOs
public record EpicExportRequest
{
    public bool IncludeStories { get; init; } = true;
    public bool IncludeHeroes { get; init; } = true;
    public bool IncludeRegions { get; init; } = true;
    public bool IncludeImages { get; init; } = true;
    public bool IncludeAudio { get; init; } = true;
    public bool IncludeVideo { get; init; } = true;
    public bool IncludeProgress { get; init; } = false;
    public string? LanguageFilter { get; init; }
}

// Response DTOs
public record EpicExportResponse
{
    public Guid JobId { get; init; }
    public string Status { get; init; } = EpicExportJobStatus.Queued;
}

public record EpicExportJobStatusResponse
{
    public Guid JobId { get; init; }
    public string EpicId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public EpicExportProgressDto Progress { get; init; } = new();
    public DateTime QueuedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ZipDownloadUrl { get; init; }
    public string? ZipFileName { get; init; }
    public long? ZipSizeBytes { get; init; }
}

public record EpicExportProgressDto
{
    public string? CurrentPhase { get; init; }
    public int StoriesExported { get; init; }
    public int TotalStories { get; init; }
    public int HeroesExported { get; init; }
    public int TotalHeroes { get; init; }
    public int RegionsExported { get; init; }
    public int TotalRegions { get; init; }
    public int AssetsExported { get; init; }
    public int TotalAssets { get; init; }
}

// Export Manifest DTOs
public record EpicExportManifestDto
{
    public string Version { get; init; } = "1.0";
    public DateTime ExportedAtUtc { get; init; }
    public EpicExportDataDto EpicData { get; init; } = new();
    public Dictionary<string, RegionExportDto> Regions { get; init; } = new();
    public Dictionary<string, HeroExportDto> Heroes { get; init; } = new();
    public Dictionary<string, StoryExportDto> Stories { get; init; } = new();
    public EpicStructureExportDto EpicStructure { get; init; } = new();
    public Dictionary<string, string> AssetMap { get; init; } = new();
}

public record EpicExportDataDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? CoverImageUrl { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? PublishedAtUtc { get; init; }
    public List<EpicTranslationExportDto> Translations { get; init; } = new();
}

public record EpicTranslationExportDto
{
    public string LanguageCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public record RegionExportDto
{
    public string Id { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public int SortOrder { get; init; }
    public bool IsLocked { get; init; }
    public bool? IsStartupRegion { get; init; }
    public double? X { get; init; }
    public double? Y { get; init; }
    public List<RegionTranslationExportDto> Translations { get; init; } = new();
}

public record RegionTranslationExportDto
{
    public string LanguageCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public record HeroExportDto
{
    public string Id { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public string? GreetingAudioUrl { get; init; }
    public string Status { get; init; } = string.Empty;
    public List<HeroTranslationExportDto> Translations { get; init; } = new();
}

public record HeroTranslationExportDto
{
    public string LanguageCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? GreetingText { get; init; }
    public string? GreetingAudioUrl { get; init; }
}

public record StoryExportDto
{
    public string StoryId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string CoverImageUrl { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string? StoryTopic { get; init; }
    public List<string>? TopicIds { get; init; }
    public string? AuthorName { get; init; }
    public string? ClassicAuthorId { get; init; }
    public List<string>? AgeGroupIds { get; init; }
    public int? PriceInCredits { get; init; }
    public string LanguageCode { get; init; } = string.Empty;
    public int StoryType { get; init; }
    public int SortOrder { get; init; }
    public List<string> UnlockedStoryHeroes { get; init; } = new();
    public bool? IsEvaluative { get; init; }
    public List<StoryTileExportDto> Tiles { get; init; } = new();
    public List<StoryTranslationExportDto> Translations { get; init; } = new();
}

public record StoryTranslationExportDto
{
    public string LanguageCode { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
}

public record StoryTileExportDto
{
    public string TileId { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public string Caption { get; init; } = string.Empty;
    public string? Text { get; init; }
    public string? ImageUrl { get; init; }
    public string? AudioUrl { get; init; }
    public string? VideoUrl { get; init; }
    public string? Question { get; init; }
    public List<StoryAnswerExportDto>? Answers { get; init; }
}

public record StoryAnswerExportDto
{
    public string AnswerId { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
    public bool? IsCorrect { get; init; }
    public List<AnswerTokenExportDto> Tokens { get; init; } = new();
    public int SortOrder { get; init; }
}

public record AnswerTokenExportDto
{
    public string Type { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public int Quantity { get; init; }
}

public record EpicStructureExportDto
{
    public List<EpicRegionReferenceDto> Regions { get; init; } = new();
    public List<EpicStoryNodeDto> StoryNodes { get; init; } = new();
    public List<EpicUnlockRuleDto> UnlockRules { get; init; } = new();
    public List<EpicHeroReferenceDto> HeroReferences { get; init; } = new();
}

public record EpicRegionReferenceDto
{
    public string Id { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public int SortOrder { get; init; }
    public bool IsLocked { get; init; }
    public bool? IsStartupRegion { get; init; }
    public double? X { get; init; }
    public double? Y { get; init; }
}

public record EpicStoryNodeDto
{
    public string StoryId { get; init; } = string.Empty;
    public string RegionId { get; init; } = string.Empty;
    public string? RewardImageUrl { get; init; }
    public int SortOrder { get; init; }
    public double? X { get; init; }
    public double? Y { get; init; }
}

public record EpicUnlockRuleDto
{
    public string Type { get; init; } = string.Empty;
    public string FromId { get; init; } = string.Empty;
    public string ToRegionId { get; init; } = string.Empty;
    public string? ToStoryId { get; init; }
    public List<string> RequiredStories { get; init; } = new();
    public int? MinCount { get; init; }
    public string? StoryId { get; init; }
    public int SortOrder { get; init; }
}

public record EpicHeroReferenceDto
{
    public string HeroId { get; init; } = string.Empty;
    public string? StoryId { get; init; } // Story that unlocks hero
    public string? HeroName { get; init; } // For UI display
    public string? HeroImageUrl { get; init; } // For UI display
}