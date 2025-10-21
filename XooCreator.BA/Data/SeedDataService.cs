using System.Text.Json;

namespace XooCreator.BA.Data;

public class SeedDataService
{
    private readonly string _seedDataPath;

    public SeedDataService(string? seedDataPath = null)
    {
        // Default to RO path; callers can instantiate additional services for other locales
        _seedDataPath = seedDataPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "ro-ro", "LaboratoryOfImagination");
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

    public async Task<List<StoryHero>> LoadStoryHeroesAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "SharedConfigs", "story-heroes.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"StoryHeroes seed data file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);
        var storyHeroesData = JsonSerializer.Deserialize<StoryHeroesSeedData>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return storyHeroesData?.StoryHeroes?.Select((sh, index) => new StoryHero
        {
            Id = GetFixedStoryHeroId(sh.HeroId), // Use fixed IDs for seeding
            HeroId = sh.HeroId,
            ImageUrl = sh.ImageUrl,
            UnlockConditionJson = JsonSerializer.Serialize(sh.UnlockConditions),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList() ?? new List<StoryHero>();
    }

    public async Task<List<StoryHeroUnlock>> LoadStoryHeroUnlocksAsync()
    {
        var storyHeroes = await LoadStoryHeroesAsync();
        var unlocks = new List<StoryHeroUnlock>();

        foreach (var storyHero in storyHeroes)
        {
            var unlockConditions = JsonSerializer.Deserialize<UnlockConditions>(storyHero.UnlockConditionJson);
            if (unlockConditions?.RequiredStories != null)
            {
                foreach (var storyId in unlockConditions.RequiredStories)
                {
                    unlocks.Add(new StoryHeroUnlock
                    {
                        Id = Guid.NewGuid(),
                        StoryHeroId = storyHero.Id,
                        StoryId = storyId,
                        UnlockOrder = 1,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        return unlocks;
    }

    public async Task<List<HeroMessage>> LoadHeroMessagesAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "SharedConfigs", "hero-messages.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"HeroMessages seed data file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);
        var heroMessagesData = JsonSerializer.Deserialize<HeroMessagesSeedData>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var heroMessages = new List<HeroMessage>();
        
        if (heroMessagesData?.HeroMessages != null)
        {
            foreach (var heroMessageData in heroMessagesData.HeroMessages)
            {
                foreach (var regionMessage in heroMessageData.RegionMessages)
                {
                    heroMessages.Add(new HeroMessage
                    {
                        Id = Guid.NewGuid(),
                        HeroId = heroMessageData.HeroId,
                        RegionId = regionMessage.RegionId,
                        MessageKey = regionMessage.MessageKey,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        return heroMessages;
    }

    public async Task<List<HeroClickMessage>> LoadHeroClickMessagesAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "SharedConfigs", "hero-messages.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"HeroMessages seed data file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);
        var heroMessagesData = JsonSerializer.Deserialize<HeroMessagesSeedData>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var heroClickMessages = new List<HeroClickMessage>();
        
        if (heroMessagesData?.HeroMessages != null)
        {
            foreach (var heroMessageData in heroMessagesData.HeroMessages)
            {
                foreach (var clickMessage in heroMessageData.ClickMessages)
                {
                    heroClickMessages.Add(new HeroClickMessage
                    {
                        Id = Guid.NewGuid(),
                        HeroId = heroMessageData.HeroId,
                        MessageKey = clickMessage.MessageKey,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        return heroClickMessages;
    }

    private static Guid GetFixedStoryHeroId(string heroId)
    {
        // Use fixed GUIDs for seeding to ensure consistency
        return heroId switch
        {
            "pufpuf" => Guid.Parse("00000000-0000-0000-0000-000000000100"),
            "linkaro" => Guid.Parse("11111111-1111-1111-1111-111111111100"),
            "grubot" => Guid.Parse("22222222-2222-2222-2222-222222222200"),
            _ => Guid.NewGuid()
        };
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
}

public class ClickMessageSeedData
{
    public string MessageKey { get; set; } = string.Empty;
}
