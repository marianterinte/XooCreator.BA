namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Admin-configurable reward token definitions. Used by Story Creator comboboxes when adding tokens to quiz/dialog answers.
/// </summary>
public class RewardTokenDefinition
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty; // e.g., Personality, Alchemy, Discovery
    public string Value { get; set; } = string.Empty; // e.g., courage, curiosity
    public string DisplayNameKey { get; set; } = string.Empty; // i18n key, e.g., token_courage
    public string? Icon { get; set; } // emoji, e.g., 🦁
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
