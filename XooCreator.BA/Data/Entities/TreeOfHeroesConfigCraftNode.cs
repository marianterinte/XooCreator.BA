namespace XooCreator.BA.Data;

public class TreeOfHeroesConfigCraftNode
{
    public Guid Id { get; set; }
    public Guid ConfigCraftId { get; set; }
    public string HeroDefinitionId { get; set; } = string.Empty;
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public int CourageCost { get; set; }
    public int CuriosityCost { get; set; }
    public int ThinkingCost { get; set; }
    public int CreativityCost { get; set; }
    public int SafetyCost { get; set; }
    public bool IsStartup { get; set; }
    public string PrerequisitesJson { get; set; } = "[]"; // JSON array of hero definition IDs

    public TreeOfHeroesConfigCraft Config { get; set; } = null!;
    public HeroDefinitionDefinition HeroDefinition { get; set; } = null!;
}
