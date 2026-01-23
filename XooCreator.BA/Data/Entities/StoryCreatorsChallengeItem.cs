using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Challenge item (story title requirement)
/// </summary>
public class StoryCreatorsChallengeItem
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string ChallengeId { get; set; }
    
    [MaxLength(100)]
    public required string ItemId { get; set; } // e.g., "item-1"
    
    public int SortOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public StoryCreatorsChallenge Challenge { get; set; } = null!;
    public ICollection<StoryCreatorsChallengeItemTranslation> Translations { get; set; } = new List<StoryCreatorsChallengeItemTranslation>();
    public ICollection<StoryCreatorsChallengeItemReward> Rewards { get; set; } = new List<StoryCreatorsChallengeItemReward>();
}
