namespace XooCreator.BA.Data;

public class DiscoveryItem
{
    public Guid Id { get; set; }
    public string ArmsKey { get; set; } = string.Empty; // e.g., "Bunny" or "—"
    public string BodyKey { get; set; } = string.Empty;
    public string HeadKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ImageV1 { get; set; }
    public string? ImageV2 { get; set; }
    public string? ImageV3 { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


