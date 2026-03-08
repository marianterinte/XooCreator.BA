namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Records a one-time grant of a Supporter Pack plan to a user (admin-granted or after order acceptance).
/// No expiry; grants stack. Used for print quota sum, LOI credits, exclusive content access.
/// </summary>
public class UserPackGrant
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    /// <summary>Plan identifier: Bronze, Silver, Gold, Platinum.</summary>
    public string PlanId { get; set; } = string.Empty;
    public DateTime GrantedAtUtc { get; set; }
    /// <summary>Admin user who granted (or system).</summary>
    public Guid GrantedByUserId { get; set; }
    /// <summary>Email used for payment/audit (e.g. bank transfer reference).</summary>
    public string? EmailUsed { get; set; }
    /// <summary>Optional link to Order when grant is created from Accept order flow.</summary>
    public Guid? OrderId { get; set; }
}
