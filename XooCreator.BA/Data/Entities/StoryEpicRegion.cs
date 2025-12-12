namespace XooCreator.BA.Data;

public class StoryEpicRegion
{
    public int Id { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK to StoryEpic
    public string RegionId { get; set; } = string.Empty; // e.g., "region-1", "region-2"
    public string Label { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsLocked { get; set; } = false;
    public double? X { get; set; } // Coordonată X pentru vizualizare
    public double? Y { get; set; } // Coordonată Y pentru vizualizare
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DbStoryEpic Epic { get; set; } = null!;
    public ICollection<StoryEpicStoryNode> Stories { get; set; } = new List<StoryEpicStoryNode>();
}

