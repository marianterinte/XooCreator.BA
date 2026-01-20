using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Supported body parts for AnimalCraft.
/// </summary>
public class AnimalCraftPartSupport
{
    public Guid AnimalCraftId { get; set; }

    [MaxLength(50)]
    public required string BodyPartKey { get; set; } = string.Empty;

    public AnimalCraft AnimalCraft { get; set; } = null!;
    public BodyPart BodyPart { get; set; } = null!;
}
