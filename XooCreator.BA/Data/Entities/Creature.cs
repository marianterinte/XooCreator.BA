namespace XooCreator.BA.Data;

public class Creature
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public AlchimaliaUser User { get; set; } = null!;
    public Guid? TreeId { get; set; }
    public Tree? Tree { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Archetype { get; set; } = string.Empty;
    public string TraitsJson { get; set; } = "{}"; // jsonb
    public string Rarity { get; set; } = "common";
    public string? ImageUrl { get; set; }
    public string? ThumbUrl { get; set; }
    public string PromptUsedJson { get; set; } = "{}";
    public string? Story { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
