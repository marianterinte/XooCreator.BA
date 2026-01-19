namespace XooCreator.BA.Data;

public class TreeOfHeroesConfigDefinition
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Status { get; set; } = "published";
    public Guid? PublishedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAtUtc { get; set; }

    public ICollection<TreeOfHeroesConfigDefinitionNode> Nodes { get; set; } = new List<TreeOfHeroesConfigDefinitionNode>();
    public ICollection<TreeOfHeroesConfigDefinitionEdge> Edges { get; set; } = new List<TreeOfHeroesConfigDefinitionEdge>();
}
