namespace XooCreator.BA.Data;

public class TreeOfHeroesConfigCraft
{
    public Guid Id { get; set; }
    public Guid? PublishedDefinitionId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public Guid? CreatedByUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TreeOfHeroesConfigCraftNode> Nodes { get; set; } = new List<TreeOfHeroesConfigCraftNode>();
    public ICollection<TreeOfHeroesConfigCraftEdge> Edges { get; set; } = new List<TreeOfHeroesConfigCraftEdge>();
}
