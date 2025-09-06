namespace XooCreator.BA.Data;

public class TreeRegion
{
    public string Id { get; set; } = string.Empty; // root, trunk, branch-1, etc.
    public string Label { get; set; } = string.Empty; // Rădăcină, Trunchi, etc.
    public string? ImageUrl { get; set; }
    public string? PufpufMessage { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TreeStoryNode> Stories { get; set; } = new List<TreeStoryNode>();
    // Note: Removed UnlockRules navigation properties to simplify FK relationships
}
