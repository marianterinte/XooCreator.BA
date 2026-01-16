using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Draft/working copy for Tree of Heroes (editor side).
/// </summary>
public class HeroDefinitionCraft
{
    [MaxLength(100)]
    public required string Id { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? PublishedDefinitionId { get; set; }

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
    public required string Status { get; set; } = "draft";

    public Guid? CreatedByUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<HeroDefinitionCraftTranslation> Translations { get; set; } = new List<HeroDefinitionCraftTranslation>();
}
