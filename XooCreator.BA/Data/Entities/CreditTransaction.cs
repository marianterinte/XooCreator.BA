namespace XooCreator.BA.Data;

public enum CreditTransactionType
{
    Purchase,
    Spend,
    Grant
}

public class CreditTransaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public int Amount { get; set; }
    public CreditTransactionType Type { get; set; }
    public string? Reference { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
