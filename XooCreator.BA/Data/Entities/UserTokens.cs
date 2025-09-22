namespace XooCreator.BA.Data;

/// <summary>
/// Represents token balances for different traits (Courage, Curiosity, etc.)
/// </summary>
public class UserTokens
{
    public Guid UserId { get; set; }
    public int Courage { get; set; }
    public int Curiosity { get; set; }
    public int Thinking { get; set; }
    public int Creativity { get; set; }
    public int Safety { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public UserAlchimalia User { get; set; } = null!;
}
