using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents the reference of a StoryRegion within a StoryEpic
/// This is a junction entity that links published regions to epics
/// </summary>
public class StoryEpicRegionReference
{
    public int Id { get; set; }
    
    [MaxLength(100)]
    public required string EpicId { get; set; } = string.Empty; // FK to StoryEpicDefinition
    
    [MaxLength(100)]
    public required string RegionId { get; set; } = string.Empty; // FK to StoryRegion (must be published)
    
    public double? X { get; set; } // Coordonată X în tree
    public double? Y { get; set; } // Coordonată Y în tree
    public int SortOrder { get; set; }
    public bool IsLocked { get; set; } = false;
    
    // Navigation properties
    public StoryEpicDefinition Epic { get; set; } = null!;
    public StoryRegion Region { get; set; } = null!;
}

