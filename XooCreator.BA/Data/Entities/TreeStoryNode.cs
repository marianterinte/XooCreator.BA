namespace XooCreator.BA.Data;

public class TreeStoryNode
{
    public int Id { get; set; }
    public string StoryId { get; set; } = string.Empty; // FK to StoryDefinition
    public string RegionId { get; set; } = string.Empty; // FK to TreeRegion
    public string? RewardImageUrl { get; set; }
    public int SortOrder { get; set; }
    public string TreeConfigurationId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public TreeConfiguration TreeConfiguration { get; set; } = null!;
    public TreeRegion Region { get; set; } = null!;
    public StoryDefinition? StoryDefinition { get; set; }
}
