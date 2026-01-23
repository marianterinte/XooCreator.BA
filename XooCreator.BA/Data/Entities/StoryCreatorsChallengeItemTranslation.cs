using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Translation for ChallengeItem (Title per language)
/// </summary>
public class StoryCreatorsChallengeItemTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string ItemId { get; set; }
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro";
    
    [MaxLength(500)]
    public required string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; } // Item description
    
    // Navigation property
    public StoryCreatorsChallengeItem Item { get; set; } = null!;
}
