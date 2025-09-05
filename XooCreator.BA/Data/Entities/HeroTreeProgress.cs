namespace XooCreator.BA.Data;

/// <summary>
/// Represents hero tree nodes that have been unlocked/purchased
/// </summary>
public class HeroTreeProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string NodeId { get; set; } = string.Empty; // e.g., "courage_path", "keen_eye"
    public int TokensCostCourage { get; set; }
    public int TokensCostCuriosity { get; set; }
    public int TokensCostThinking { get; set; }
    public int TokensCostCreativity { get; set; }
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User User { get; set; } = null!;
}
