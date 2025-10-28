namespace XooCreator.BA.Data;

/// <summary>
/// Represents a story owned by a user (purchased from marketplace)
/// </summary>
public class UserOwnedStories
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StoryDefinitionId { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public decimal PurchasePrice { get; set; }
    public string? PurchaseReference { get; set; } // Reference to the transaction
    
    // Navigation properties
    public AlchimaliaUser User { get; set; } = null!;
    public StoryDefinition StoryDefinition { get; set; } = null!;
}
