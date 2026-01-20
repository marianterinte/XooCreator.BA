using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Published Tree of Heroes definition (runtime source).
/// </summary>
public class HeroDefinitionDefinition
{
    [MaxLength(100)]
    public required string Id { get; set; } = string.Empty;

    public int Version { get; set; } = 1;
    public int BaseVersion { get; set; } = 0;
    public int LastPublishedVersion { get; set; } = 0;

    public int CourageCost { get; set; }
    public int CuriosityCost { get; set; }
    public int ThinkingCost { get; set; }
    public int CreativityCost { get; set; }
    public int SafetyCost { get; set; }

    public string PrerequisitesJson { get; set; } = "[]";
    public string RewardsJson { get; set; } = "[]";
    public bool IsUnlocked { get; set; }

    public double PositionX { get; set; }
    public double PositionY { get; set; }

    public string Image { get; set; } = string.Empty;

    [MaxLength(20)]
    public required string Status { get; set; } = "published";

    public Guid? PublishedByUserId { get; set; }
    public DateTime? PublishedAtUtc { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<HeroDefinitionDefinitionTranslation> Translations { get; set; } = new List<HeroDefinitionDefinitionTranslation>();
}
