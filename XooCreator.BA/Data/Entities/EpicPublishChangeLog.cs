namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Tracks granular modifications performed inside the epic editor so publish can replay only diffs.
/// </summary>
public class EpicPublishChangeLog
{
    public Guid Id { get; set; }
    public string EpicId { get; set; } = string.Empty;
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
/// Links draft assets to their published counterparts to support rename/copy tracking for epics.
/// </summary>
public class EpicAssetLink
{
    public Guid Id { get; set; }
    public string EpicId { get; set; } = string.Empty;
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

