namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Configurable definition of a Supporter Pack plan (Bronze, Silver, Gold, Platinum).
/// Used by grant/order flow for credits, print quota, and by public page for display.
/// </summary>
public class SupporterPackPlan
{
    public string PlanId { get; set; } = string.Empty;
    public decimal PriceRon { get; set; }
    public int GenerativeCredits { get; set; }
    public int PrintQuota { get; set; }
    public int ExclusiveStoryAccessCount { get; set; }
    public int ExclusiveEpicAccessCount { get; set; }
    public int FullStoryGenerationCount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime UpdatedAtUtc { get; set; }
}
