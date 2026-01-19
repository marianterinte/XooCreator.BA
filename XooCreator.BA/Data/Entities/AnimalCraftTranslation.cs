using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Translation for AnimalCraft.
/// </summary>
public class AnimalCraftTranslation
{
    public Guid Id { get; set; }

    public Guid AnimalCraftId { get; set; }

    [MaxLength(10)]
    public required string LanguageCode { get; set; } = string.Empty;

    [MaxLength(255)]
    public required string Label { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? AudioUrl { get; set; }

    public AnimalCraft AnimalCraft { get; set; } = null!;
}
