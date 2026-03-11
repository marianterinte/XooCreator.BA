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
    /// <summary>For generative type: blob path of the generated image. Null for discovery/tree heroes.</summary>
    public string? ImageBlobPath { get; set; }
    /// <summary>For generative type: full combination as JSON e.g. {"head":"Bunny","body":"Hippo","arms":"Giraffe",...}. Null for discovery.</summary>
    public string? PartsJson { get; set; }
    public ICollection<BestiaryItemTranslation> Translations { get; set; } = new List<BestiaryItemTranslation>();
}


