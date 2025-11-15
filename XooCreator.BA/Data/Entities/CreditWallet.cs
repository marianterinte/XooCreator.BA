namespace XooCreator.BA.Data;

public class CreditWallet
{
    public Guid UserId { get; set; }
    public AlchimaliaUser User { get; set; } = null!;
    // Generative credits (legacy Balance kept for backwards compatibility)
    public double Balance { get; set; }
    // New: Discovery credits balance
    public double DiscoveryBalance { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
