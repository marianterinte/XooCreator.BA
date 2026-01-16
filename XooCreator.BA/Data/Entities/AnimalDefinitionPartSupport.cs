using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Supported body parts for AnimalDefinition.
/// </summary>
public class AnimalDefinitionPartSupport
{
    public Guid AnimalDefinitionId { get; set; }

    [MaxLength(50)]
    public required string BodyPartKey { get; set; } = string.Empty;

    public AnimalDefinition AnimalDefinition { get; set; } = null!;
    public BodyPart BodyPart { get; set; } = null!;
}
