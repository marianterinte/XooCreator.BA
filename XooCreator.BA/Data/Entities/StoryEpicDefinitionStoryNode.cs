using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data;

public class StoryEpicDefinitionStoryNode
{
    public int Id { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK to StoryEpicDefinition
    public string StoryId { get; set; } = string.Empty; // FK to StoryDefinition (only published stories)
    public string RegionId { get; set; } = string.Empty; // FK to StoryEpicDefinitionRegion
    public string? RewardImageUrl { get; set; }
    public int SortOrder { get; set; }
    public double? X { get; set; } // Coordonată X pentru vizualizare
    public double? Y { get; set; } // Coordonată Y pentru vizualizare
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryEpicDefinition Epic { get; set; } = null!;
    public StoryEpicDefinitionRegion Region { get; set; } = null!;
    public StoryDefinition StoryDefinition { get; set; } = null!; // Only published stories in definition
}

