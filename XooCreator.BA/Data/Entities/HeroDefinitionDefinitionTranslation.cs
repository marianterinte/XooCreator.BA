using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for HeroDefinitionDefinition.
/// </summary>
public class HeroDefinitionDefinitionTranslation
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public required string HeroDefinitionDefinitionId { get; set; } = string.Empty;

    [MaxLength(10)]
    public required string LanguageCode { get; set; } = string.Empty;

    [MaxLength(255)]
    public required string Name { get; set; } = string.Empty;

    public required string Description { get; set; } = string.Empty;
    public required string Story { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }

    public HeroDefinitionDefinition HeroDefinitionDefinition { get; set; } = null!;
}
