namespace XooCreator.BA.Data;

public class BestiaryItem
{
    public Guid Id { get; set; }
    public string ArmsKey { get; set; } = string.Empty; // e.g., "Bunny" or "—"
    public string BodyKey { get; set; } = string.Empty;
    public string HeadKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<BestiaryItemTranslation> Translations { get; set; } = new List<BestiaryItemTranslation>();
}


