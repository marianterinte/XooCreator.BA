namespace XooCreator.BA.Data;

public class CreditWallet
{
    public Guid UserId { get; set; }
    public AlchimaliaUser User { get; set; } = null!;
    // Legacy balance (purchased credits)
    public double Balance { get; set; }
    // Discovery credits (e.g. Tree of Light, reset)
    public double DiscoveryBalance { get; set; }
    /// <summary>Supporter Pack generative LOI credits (consumed per generation).</summary>
    public double GenerativeBalance { get; set; }
    /// <summary>Supporter Pack full story generation credits (consumed per private story generation).</summary>
    public double FullStoryGenerationBalance { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
