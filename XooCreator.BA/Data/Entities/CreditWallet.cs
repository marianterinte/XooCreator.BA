namespace XooCreator.BA.Data;

public class CreditWallet
{
    public Guid UserId { get; set; }
    public AlchimaliaUser User { get; set; } = null!;
    // Generative credits (legacy Balance kept for backwards compatibility)
    public int Balance { get; set; }
    // New: Discovery credits balance
    public int DiscoveryBalance { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
