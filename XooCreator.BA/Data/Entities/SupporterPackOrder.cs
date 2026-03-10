namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Supporter Pack order: user creates with PendingPayment; after payment proof admin Accepts and grant is created.
/// </summary>
public class SupporterPackOrder
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    /// <summary>Bronze, Silver, Gold, Platinum.</summary>
    public string PlanId { get; set; } = string.Empty;
    /// <summary>Amount in RON.</summary>
    public decimal Amount { get; set; }
    public SupporterPackOrderStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    /// <summary>Idempotency key used to deduplicate create-order requests for a user.</summary>
    public string? IdempotencyKey { get; set; }
    /// <summary>Optional reference for payment (e.g. bank transfer note).</summary>
    public string? OrderReference { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public Guid? ProcessedByUserId { get; set; }
}

public enum SupporterPackOrderStatus
{
    PendingPayment = 0,
    PaymentConfirmed = 1,
    Fulfilled = 2,
    Rejected = 3,
    Refunded = 4
}
