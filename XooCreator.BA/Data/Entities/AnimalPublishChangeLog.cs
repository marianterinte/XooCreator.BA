namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Tracks granular modifications performed inside the animal editor so publish can replay only diffs.
/// </summary>
public class AnimalPublishChangeLog
{
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
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
