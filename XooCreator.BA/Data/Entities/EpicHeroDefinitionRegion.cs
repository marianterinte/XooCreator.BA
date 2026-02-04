using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between EpicHeroDefinition and StoryRegionDefinition (hero ↔ regions).
/// </summary>
public class EpicHeroDefinitionRegion
{
    public string EpicHeroDefinitionId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string RegionId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public EpicHeroDefinition EpicHeroDefinition { get; set; } = null!;
    public StoryRegionDefinition? Region { get; set; }
}
