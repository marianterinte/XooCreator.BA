using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data;

public class StoryEpicCraftStoryNode
{
    public int Id { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK to StoryEpicCraft
    public string StoryId { get; set; } = string.Empty; // FK to StoryCraft sau StoryDefinition
    public string RegionId { get; set; } = string.Empty; // FK to StoryEpicCraftRegion
    public string? RewardImageUrl { get; set; }
    public int SortOrder { get; set; }
    public double? X { get; set; } // Coordonată X pentru vizualizare
    public double? Y { get; set; } // Coordonată Y pentru vizualizare
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryEpicCraft Epic { get; set; } = null!;
    public StoryEpicCraftRegion Region { get; set; } = null!;
    public StoryCraft? StoryCraft { get; set; } // Pentru draft stories
    public StoryDefinition? StoryDefinition { get; set; } // Pentru published stories
}

