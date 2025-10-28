namespace XooCreator.BA.Data.SeedData.DTOs;

public class HeroMessagesSeedData
{
    public List<HeroMessageSeedData> HeroMessages { get; set; } = new();
}

public class HeroMessageSeedData
{
    public string HeroId { get; set; } = string.Empty;
    public List<RegionMessageSeedData> RegionMessages { get; set; } = new();
    public List<ClickMessageSeedData> ClickMessages { get; set; } = new();
}

public class RegionMessageSeedData
{
    public string RegionId { get; set; } = string.Empty;
    public string MessageKey { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
}

public class ClickMessageSeedData
{
    public string MessageKey { get; set; } = string.Empty;
    public string? AudioUrl { get; set; }
}
