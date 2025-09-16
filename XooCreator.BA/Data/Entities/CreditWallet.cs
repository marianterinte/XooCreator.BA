namespace XooCreator.BA.Data;

public class CreditWallet
{
    public Guid UserId { get; set; }
    public UserAlchimalia User { get; set; } = null!;
    public int Balance { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
