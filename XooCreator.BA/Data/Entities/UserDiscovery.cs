namespace XooCreator.BA.Data;

public class UserBestiary
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserAlchimalia User { get; set; } = null!;
    public Guid BestiaryItemId { get; set; }
    public BestiaryItem BestiaryItem { get; set; } = null!;
    public string BestiaryType { get; set; } = null!; // "discovery", "treeoflight", "generative"
    public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
}


