namespace XooCreator.BA.Data;

public class TreeRegion
{
    public string Id { get; set; } = string.Empty; // root, trunk, branch-1, etc.
    public string Label { get; set; } = string.Empty; // Rădăcină, Trunchi, etc.
    public string? ImageUrl { get; set; }
    public string? PufpufMessage { get; set; }
    public int SortOrder { get; set; }
    public bool IsLocked { get; set; } = false;
    public string TreeConfigurationId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public TreeConfiguration TreeConfiguration { get; set; } = null!;
    public ICollection<TreeStoryNode> Stories { get; set; } = new List<TreeStoryNode>();
    // Note: Removed UnlockRules navigation properties to simplify FK relationships
}
