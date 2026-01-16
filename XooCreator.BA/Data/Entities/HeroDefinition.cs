using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

public class HeroDefinition
{
    [Key]
    public string Id { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public int CourageCost { get; set; }
    public int CuriosityCost { get; set; }
    public int ThinkingCost { get; set; }
    public int CreativityCost { get; set; }
    public int SafetyCost { get; set; }

    public string PrerequisitesJson { get; set; } = string.Empty;
    public string RewardsJson { get; set; } = string.Empty;

    public bool IsUnlocked { get; set; }

    public double PositionX { get; set; }
    public double PositionY { get; set; }

    public string Image { get; set; } = string.Empty;

    // Workflow fields
    public string Status { get; set; } = AlchimaliaUniverseStatus.Draft.ToDb();
    public Guid? CreatedByUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public int Version { get; set; } = 1;
    public string? ParentVersionId { get; set; } // FK către HeroDefinitions (pentru versioning)

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<HeroDefinitionTranslation> Translations { get; set; } = new List<HeroDefinitionTranslation>();
}
