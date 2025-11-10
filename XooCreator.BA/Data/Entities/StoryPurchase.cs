namespace XooCreator.BA.Data;

/// <summary>
/// Represents a story purchase by a user in the marketplace
/// </summary>
public class StoryPurchase
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public int CreditsSpent { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public AlchimaliaUser User { get; set; } = null!;
    public StoryDefinition Story { get; set; } = null!;
}

/// <summary>
/// Represents story marketplace metadata and pricing
/// </summary>
// Removed StoryMarketplaceInfo entity
