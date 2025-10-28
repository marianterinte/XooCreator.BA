namespace XooCreator.BA.Data.SeedData.DTOs;

public class BodyPartSeedData
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public bool IsBaseLocked { get; set; }
}

public class RegionSeedData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class AnimalSeedData
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Src { get; set; } = string.Empty;
    public string RegionId { get; set; } = string.Empty;
    public bool IsHybrid { get; set; }
}

public class AnimalPartSupportSeedData
{
    public string AnimalId { get; set; } = string.Empty;
    public string PartKey { get; set; } = string.Empty;
}
