using System.Text.Json;

namespace XooCreator.BA.Data;

public class SeedDataService
{
    private readonly string _seedDataPath;

    public SeedDataService(string? seedDataPath = null)
    {
        _seedDataPath = seedDataPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData\\LaboratoryOfImagination");
    }

    public async Task<List<BodyPart>> LoadBodyPartsAsync()
    {
        var filePath = Path.Combine(_seedDataPath, "bodyparts.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"BodyParts seed data file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);
        var bodyParts = JsonSerializer.Deserialize<List<BodyPartSeedData>>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return bodyParts?.Select(bp => new BodyPart
        {
            Key = bp.Key,
            Name = bp.Name,
            Image = bp.Image,
            IsBaseLocked = bp.IsBaseLocked
        }).ToList() ?? new List<BodyPart>();
    }

    public async Task<List<Region>> LoadRegionsAsync()
    {
        var filePath = Path.Combine(_seedDataPath, "regions.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Regions seed data file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);
        var regions = JsonSerializer.Deserialize<List<RegionSeedData>>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return regions?.Select(r => new Region
        {
            Id = Guid.Parse(r.Id),
            Name = r.Name
        }).ToList() ?? new List<Region>();
    }

    public async Task<List<Animal>> LoadAnimalsAsync()
    {
        var filePath = Path.Combine(_seedDataPath, "animals.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Animals seed data file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);
        var animals = JsonSerializer.Deserialize<List<AnimalSeedData>>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return animals?.Select(a => new Animal
        {
            Id = Guid.Parse(a.Id),
            Label = a.Label,
            Src = a.Src,
            RegionId = Guid.Parse(a.RegionId),
            IsHybrid = a.IsHybrid
        }).ToList() ?? new List<Animal>();
    }

    public async Task<List<AnimalPartSupport>> LoadAnimalPartSupportsAsync()
    {
        var filePath = Path.Combine(_seedDataPath, "animal-part-supports.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"AnimalPartSupports seed data file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);
        var supports = JsonSerializer.Deserialize<List<AnimalPartSupportSeedData>>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return supports?.Select(aps => new AnimalPartSupport
        {
            AnimalId = Guid.Parse(aps.AnimalId),
            PartKey = aps.PartKey
        }).ToList() ?? new List<AnimalPartSupport>();
    }
}

// DTOs for seed data deserialization
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
