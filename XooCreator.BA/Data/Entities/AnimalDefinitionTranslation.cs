using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for AnimalDefinition.
/// </summary>
public class AnimalDefinitionTranslation
{
    public Guid Id { get; set; }

    public Guid AnimalDefinitionId { get; set; }

    [MaxLength(10)]
    public required string LanguageCode { get; set; } = string.Empty;

    [MaxLength(255)]
    public required string Label { get; set; } = string.Empty;

    public AnimalDefinition AnimalDefinition { get; set; } = null!;
}
