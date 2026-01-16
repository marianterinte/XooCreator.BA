using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Draft/working copy for LOI animals.
/// </summary>
public class AnimalCraft
{
    public Guid Id { get; set; }
    public Guid? PublishedDefinitionId { get; set; }

    [MaxLength(255)]
    public required string Label { get; set; } = string.Empty;

    [MaxLength(500)]
    public required string Src { get; set; } = string.Empty;

    public bool IsHybrid { get; set; }
    public Guid RegionId { get; set; }

    [MaxLength(20)]
    public required string Status { get; set; } = "draft";

    public Guid? CreatedByUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Region Region { get; set; } = null!;
    public ICollection<AnimalCraftTranslation> Translations { get; set; } = new List<AnimalCraftTranslation>();
    public ICollection<AnimalCraftPartSupport> SupportedParts { get; set; } = new List<AnimalCraftPartSupport>();
}
