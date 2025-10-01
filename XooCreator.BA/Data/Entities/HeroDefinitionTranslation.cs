namespace XooCreator.BA.Data;

public class HeroDefinitionTranslation
{
    public Guid Id { get; set; }
    public string HeroDefinitionId { get; set; } = string.Empty;
    public HeroDefinition HeroDefinition { get; set; } = null!;
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
}
