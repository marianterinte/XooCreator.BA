namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Tracks granular modifications performed inside the region editor so publish can replay only diffs.
/// </summary>
public class RegionPublishChangeLog
{
    public Guid Id { get; set; }
    public string RegionId { get; set; } = string.Empty;
    public int DraftVersion { get; set; }
    public string LanguageCode { get; set; } = "ro-ro";
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? Hash { get; set; }
    public string? PayloadJson { get; set; }
    public string? AssetDraftPath { get; set; }
    public string? AssetPublishedPath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
}

/// <summary>
/// Tracks granular modifications performed inside the hero editor so publish can replay only diffs.
/// </summary>
public class HeroPublishChangeLog
{
    public Guid Id { get; set; }
    public string HeroId { get; set; } = string.Empty;
    public int DraftVersion { get; set; }
    public string LanguageCode { get; set; } = "ro-ro";
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? Hash { get; set; }
    public string? PayloadJson { get; set; }
    public string? AssetDraftPath { get; set; }
    public string? AssetPublishedPath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
}

/// <summary>
/// Links draft assets to their published counterparts to support rename/copy tracking for regions.
/// </summary>
public class RegionAssetLink
{
    public Guid Id { get; set; }
    public string RegionId { get; set; } = string.Empty;
    public int DraftVersion { get; set; }
    public string? LanguageCode { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string DraftPath { get; set; } = string.Empty;
    public string? PublishedPath { get; set; }
    public string? ContentHash { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Links draft assets to their published counterparts to support rename/copy tracking for heroes.
/// </summary>
public class HeroAssetLink
{
    public Guid Id { get; set; }
    public string HeroId { get; set; } = string.Empty;
    public int DraftVersion { get; set; }
    public string? LanguageCode { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string DraftPath { get; set; } = string.Empty;
    public string? PublishedPath { get; set; }
    public string? ContentHash { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

