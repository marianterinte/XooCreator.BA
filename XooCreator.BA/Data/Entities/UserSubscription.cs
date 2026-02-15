namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Records a user's Stripe Supporter Pack purchase.
/// Used to grant unlimited print quota until ExpiresAtUtc.
/// </summary>
public class UserSubscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string? StripeSessionId { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public DateTime PaidAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
