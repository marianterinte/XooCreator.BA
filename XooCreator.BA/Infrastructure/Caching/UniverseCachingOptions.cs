namespace XooCreator.BA.Infrastructure.Caching;

/// <summary>
/// Configuration for Universe / Story region & hero caching.
/// Bound from the \"Caching\" section in appsettings.*.json.
/// </summary>
public sealed class UniverseCachingOptions
{
    public const string SectionName = "Caching";

    public bool Enabled { get; set; } = true;
    public int UniverseRegionsMinutes { get; set; } = 720;
    public int UniverseRegionHeroesMinutes { get; set; } = 720;
    public int StoryRegionMinutes { get; set; } = 15;
    public int StoryHeroMinutes { get; set; } = 15;

    public static string GetUniverseRegionsKey() => "alchimalia_universe_regions_v1";

    public static string GetUniverseRegionHeroesKey(string regionId) =>
        $"alchimalia_universe_region_{regionId}_heroes_v1";

    public static string GetStoryRegionKey(string regionId) =>
        $"story_region_{regionId}_v1";

    public static string GetStoryHeroKey(string heroId) =>
        $"story_hero_{heroId}_v1";
}

