using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Token balance for creators. Stores different types of tokens earned from stories.
/// Admin can override values manually.
/// </summary>
public class CreatorTokenBalance
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    [MaxLength(50)]
    public required string TokenType { get; set; } // e.g., "Alchimalia", "Personality", "Alchemy", etc.
    
    [MaxLength(200)]
    public required string TokenValue { get; set; } // e.g., "Alchimalia Token", "courage", "Karott", etc.
    
    public int Quantity { get; set; } = 0; // Current balance
    
    /// <summary>
    /// If true, this value was manually set by an admin (override)
    /// </summary>
    public bool IsAdminOverride { get; set; } = false;
    
    /// <summary>
    /// User ID of the admin who set the override (if IsAdminOverride is true)
    /// </summary>
    public Guid? OverrideByUserId { get; set; }
    
    /// <summary>
    /// Timestamp when override was set
    /// </summary>
    public DateTime? OverrideAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
    public AlchimaliaUser? OverrideByUser { get; set; }
}
