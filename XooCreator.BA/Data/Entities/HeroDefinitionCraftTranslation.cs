using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for HeroDefinitionCraft.
/// </summary>
public class HeroDefinitionCraftTranslation
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public required string HeroDefinitionCraftId { get; set; } = string.Empty;

    [MaxLength(10)]
    public required string LanguageCode { get; set; } = string.Empty;

    [MaxLength(255)]
    public required string Name { get; set; } = string.Empty;

    public required string Description { get; set; } = string.Empty;
    public required string Story { get; set; } = string.Empty;

    public HeroDefinitionCraft HeroDefinitionCraft { get; set; } = null!;
}
