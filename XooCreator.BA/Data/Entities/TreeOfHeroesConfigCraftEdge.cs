namespace XooCreator.BA.Data;

public class TreeOfHeroesConfigCraftEdge
{
    public Guid Id { get; set; }
    public Guid ConfigCraftId { get; set; }
    public string FromHeroId { get; set; } = string.Empty;
    public string ToHeroId { get; set; } = string.Empty;

    public TreeOfHeroesConfigCraft Config { get; set; } = null!;
    public HeroDefinitionDefinition FromHero { get; set; } = null!;
    public HeroDefinitionDefinition ToHero { get; set; } = null!;
}
