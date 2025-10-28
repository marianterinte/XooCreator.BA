namespace XooCreator.BA.Data.SeedData.DTOs;

public class StoryHeroesSeedData
{
    public List<StoryHeroSeedData> StoryHeroes { get; set; } = new();
}

public class StoryHeroSeedData
{
    public string Id { get; set; } = string.Empty;
    public string HeroId { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public UnlockConditions UnlockConditions { get; set; } = new();
}

public class UnlockConditions
{
    public string Type { get; set; } = string.Empty;
    public List<string> RequiredStories { get; set; } = new();
    public int MinProgress { get; set; }
}
