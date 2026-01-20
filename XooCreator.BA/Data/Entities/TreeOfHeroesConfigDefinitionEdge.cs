namespace XooCreator.BA.Data;

public class TreeOfHeroesConfigDefinitionEdge
{
    public Guid Id { get; set; }
    public Guid ConfigDefinitionId { get; set; }
    public string FromHeroId { get; set; } = string.Empty;
    public string ToHeroId { get; set; } = string.Empty;

    public TreeOfHeroesConfigDefinition Config { get; set; } = null!;
    public HeroDefinitionDefinition FromHero { get; set; } = null!;
    public HeroDefinitionDefinition ToHero { get; set; } = null!;
}
