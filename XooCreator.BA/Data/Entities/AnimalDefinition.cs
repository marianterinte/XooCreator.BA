using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Published LOI animal definition (runtime source).
/// </summary>
public class AnimalDefinition
{
    public Guid Id { get; set; }

    [MaxLength(255)]
    public required string Label { get; set; } = string.Empty;

    [MaxLength(500)]
    public required string Src { get; set; } = string.Empty;

    public bool IsHybrid { get; set; }
    public Guid RegionId { get; set; }

    [MaxLength(20)]
    public required string Status { get; set; } = "published";

    public Guid? PublishedByUserId { get; set; }
    public DateTime? PublishedAtUtc { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Region Region { get; set; } = null!;
    public ICollection<AnimalDefinitionTranslation> Translations { get; set; } = new List<AnimalDefinitionTranslation>();
    public ICollection<AnimalDefinitionPartSupport> SupportedParts { get; set; } = new List<AnimalDefinitionPartSupport>();
}
