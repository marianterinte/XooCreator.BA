namespace XooCreator.BA.Data;

/// <summary>
/// Represents the reference of an EpicHero within a StoryEpicCraft (draft epic)
/// Similar to StoryEpicCraftRegion - used for drafts, not published epics
/// </summary>
public class StoryEpicCraftHeroReference
{
    public int Id { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK to StoryEpicCraft
    public string HeroId { get; set; } = string.Empty; // FK to EpicHero
    public string? StoryId { get; set; } // Story care deblochează acest hero (opțional)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryEpicCraft Epic { get; set; } = null!;
    public EpicHeroCraft Hero { get; set; } = null!;
}

