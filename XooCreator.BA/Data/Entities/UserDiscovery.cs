namespace XooCreator.BA.Data;

public class UserDiscovery
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserAlchimalia User { get; set; } = null!;
    public Guid DiscoveryItemId { get; set; }
    public DiscoveryItem DiscoveryItem { get; set; } = null!;
    public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
}


