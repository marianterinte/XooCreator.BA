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
public class StoryMarketplaceInfo
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public int PriceInCredits { get; set; }
    public string Region { get; set; } = string.Empty;
    public string AgeRating { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public List<string> Characters { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public int EstimatedReadingTime { get; set; } // in minutes
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public StoryDefinition Story { get; set; } = null!;
}
