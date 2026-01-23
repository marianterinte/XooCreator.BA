using System.ComponentModel.DataAnnotations;
// Try to resolve TokenFamily if needed, though strictly it's stored as string.
// If TokenFamily is needed, I'll need to know its namespace. 
// Based on analysis, TokenType is stored as string.

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Reward for a challenge item (tokens)
/// </summary>
public class StoryCreatorsChallengeItemReward
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string ItemId { get; set; }
    
    [MaxLength(50)]
    public required string TokenType { get; set; } = "Personality"; // TokenFamily enum value as string
    
    [MaxLength(200)]
    public required string TokenValue { get; set; } = string.Empty; // e.g., "courage", "beers", "wine"
    
    public int Quantity { get; set; } = 1;
    
    public int SortOrder { get; set; } = 0;
    
    // Navigation property
    public StoryCreatorsChallengeItem Item { get; set; } = null!;
}
