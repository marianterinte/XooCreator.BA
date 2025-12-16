namespace XooCreator.BA.Data;

public class StoryEpicDefinitionRegion
{
    public int Id { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK to StoryEpicDefinition
    public string RegionId { get; set; } = string.Empty; // e.g., "region-1", "region-2"
    public string Label { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsLocked { get; set; } = false;
    public bool IsStartupRegion { get; set; } = false; // Marchează regiunea ca regiune de început pentru epic
    public double? X { get; set; } // Coordonată X pentru vizualizare
    public double? Y { get; set; } // Coordonată Y pentru vizualizare
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryEpicDefinition Epic { get; set; } = null!;
    public ICollection<StoryEpicDefinitionStoryNode> Stories { get; set; } = new List<StoryEpicDefinitionStoryNode>();
}

