using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between EpicHeroCraft and StoryRegionDefinition (hero ↔ regions).
/// </summary>
public class EpicHeroCraftRegion
{
    public string EpicHeroCraftId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string RegionId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public EpicHeroCraft EpicHeroCraft { get; set; } = null!;
    public StoryRegionDefinition? Region { get; set; }
}
