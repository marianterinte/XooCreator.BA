namespace XooCreator.BA.Data.SeedData.DTOs;

/// <summary>
/// DTO for loading a single story hero from JSON file
/// Heroes are now independent - stories decide which heroes unlock via unlockedStoryHeroes array
/// </summary>
public class StoryHeroSeedData
{
    public string HeroId { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

// Legacy DTOs kept for backward compatibility but no longer used in new seeding
// Keeping for reference in case needed for migration
public class StoryHeroesSeedData
{
    public List<StoryHeroSeedData> StoryHeroes { get; set; } = new();
}

public class UnlockConditions
{
    public string Type { get; set; } = string.Empty;
    public List<string> RequiredStories { get; set; } = new();
    public int MinProgress { get; set; }
}
