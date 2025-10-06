namespace XooCreator.BA.Data;

/// <summary>
/// Aggregated balance per (UserId, TokenType, TokenValue). Extensible for any token family.
/// </summary>
public class UserTokenBalance
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty; // e.g., "TreeOfHeroes", "AnimalEvolution"
    public string Value { get; set; } = string.Empty; // e.g., "courage", "Karott"
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
}


