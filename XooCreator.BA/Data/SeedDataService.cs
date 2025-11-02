using System.Text.Json;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data.SeedData.DTOs;

namespace XooCreator.BA.Data;

public class SeedDataService
{
    private List<(StoryTile tile, string storyId)> _independentStoryTiles = new();
    private readonly string _seedDataPath;

    public SeedDataService(string? seedDataPath = null)
    {
        _seedDataPath = seedDataPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "LaboratoryOfImagination", "i18n", "ro-ro");
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
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "BookOfHeroes", "hero-messages.json");
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
                        AudioUrl = regionMessage.AudioUrl,
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
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "BookOfHeroes", "hero-messages.json");
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
                        AudioUrl = clickMessage.AudioUrl,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        return heroClickMessages;
    }

    public async Task<List<StoryDefinition>> LoadIndependentStoriesAsync()
    {
        var storiesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "Stories", "independent", "i18n", "ro-ro");
        var stories = new List<StoryDefinition>();

        Console.WriteLine($"[SEEDING] Loading independent stories from: {storiesPath}");

        if (!Directory.Exists(storiesPath))
        {
            Console.WriteLine($"[SEEDING] Directory does not exist: {storiesPath}");
            return stories;
        }

        var jsonFiles = Directory.GetFiles(storiesPath, "*.json");
        Console.WriteLine($"[SEEDING] Found {jsonFiles.Length} JSON files");
        
        foreach (var filePath in jsonFiles)
        {
            try
            {
                Console.WriteLine($"[SEEDING] Processing file: {Path.GetFileName(filePath)}");
                var json = await File.ReadAllTextAsync(filePath);
                var storyData = JsonSerializer.Deserialize<StoryDefinitionSeedData>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (storyData != null)
                {
                    var storyId = GetStoryDefinitionIdByStoryId(storyData.StoryId);
                    Console.WriteLine($"[SEEDING] Creating story: {storyData.StoryId} with ID: {storyId}");
                    
                    var storyDefinition = new StoryDefinition
                    {
                        Id = storyId, // Use consistent ID for each story
                        StoryId = storyData.StoryId,
                        Title = storyData.Title,
                        CoverImageUrl = storyData.CoverImageUrl,
                        StoryTopic = storyData.Category,
                        Summary = storyData.Summary ?? string.Empty,
                        StoryType = StoryType.Indie,
                        Status = StoryStatus.Published,
                        SortOrder = storyData.SortOrder,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.Parse("33333333-3333-3333-3333-333333333333"), // Alchimalia-Admin
                        UpdatedBy = Guid.Parse("33333333-3333-3333-3333-333333333333") // Alchimalia-Admin
                    };

                    // Process tiles if they exist
                    if (storyData.Tiles != null && storyData.Tiles.Any())
                    {
                        Console.WriteLine($"[SEEDING] Processing {storyData.Tiles.Count} tiles for story: {storyData.StoryId}");
                        foreach (var tileSeed in storyData.Tiles)
                        {
                            var tile = new StoryTile
                            {
                                Id = Guid.NewGuid(), // Generate unique ID for tile
                                TileId = tileSeed.TileId,
                                Type = tileSeed.Type,
                                SortOrder = tileSeed.SortOrder,
                                Caption = tileSeed.Caption,
                                Text = tileSeed.Text,
                                ImageUrl = tileSeed.ImageUrl,
                                AudioUrl = null, // Independent stories don't have audio
                                Question = tileSeed.Question,  // Set question for quiz tiles
                                StoryDefinitionId = storyId // Set the foreign key
                            };

                            // NOTE: Do NOT process Answers here because this method is used for migrations
                            // which don't allow navigation properties. Independent stories should be seeded
                            // via StoriesRepository.SeedIndependentStoriesAsync() which uses SaveChanges()
                            // and can handle navigation properties through MapFromSeedData()

                            // Store tile WITHOUT Answers to avoid navigation property issues
                            _independentStoryTiles.Add((tile, storyData.StoryId));
                        }
                        Console.WriteLine($"[SEEDING] Prepared {storyData.Tiles.Count} tiles for story: {storyData.StoryId}");
                    }
                    else
                    {
                        Console.WriteLine($"[SEEDING] No tiles found for story: {storyData.StoryId}");
                    }

                    stories.Add(storyDefinition);
                    Console.WriteLine($"[SEEDING] Added story: {storyData.StoryId} -> {storyId}");
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with other files
                Console.WriteLine($"[SEEDING ERROR] Error loading story from {filePath}: {ex.Message}");
            }
        }

        Console.WriteLine($"[SEEDING] Total stories loaded: {stories.Count}");
        return stories;
    }

    public List<StoryTile> GetIndependentStoryTiles()
    {
        return _independentStoryTiles.Select(t => t.tile).ToList();
    }

    public async Task<List<StoryDefinitionTranslation>> LoadIndependentStoryTranslationsAsync(List<StoryDefinition> existingStories)
    {
        var translations = new List<StoryDefinitionTranslation>();
        var languages = LanguageCodeExtensions.All().Select(lang => lang.ToFolder()).ToArray();

        Console.WriteLine($"[SEEDING] Loading translations for {existingStories.Count} existing stories");

        // Create a mapping of storyId to actual StoryDefinition ID
        var storyIdToDefinitionId = existingStories.ToDictionary(s => s.StoryId, s => s.Id);
        Console.WriteLine($"[SEEDING] Story ID mapping created:");
        foreach (var kvp in storyIdToDefinitionId)
        {
            Console.WriteLine($"[SEEDING]   {kvp.Key} -> {kvp.Value}");
        }

        foreach (var language in languages)
        {
            var storiesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "Stories", "independent", "i18n", language);
            Console.WriteLine($"[SEEDING] Processing language: {language} from {storiesPath}");
            
            if (!Directory.Exists(storiesPath))
            {
                Console.WriteLine($"[SEEDING] Directory does not exist for language {language}: {storiesPath}");
                continue;
            }

            var jsonFiles = Directory.GetFiles(storiesPath, "*.json");
            Console.WriteLine($"[SEEDING] Found {jsonFiles.Length} JSON files for language {language}");
            
            foreach (var filePath in jsonFiles)
            {
                try
                {
                    Console.WriteLine($"[SEEDING] Processing translation file: {Path.GetFileName(filePath)}");
                    var json = await File.ReadAllTextAsync(filePath);
                    var storyData = JsonSerializer.Deserialize<StoryDefinitionSeedData>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    if (storyData != null && storyIdToDefinitionId.TryGetValue(storyData.StoryId, out var storyDefinitionId))
                    {
                        var translationId = Guid.NewGuid();
                        Console.WriteLine($"[SEEDING] Creating translation: {storyData.StoryId} ({language}) -> {storyDefinitionId} with translation ID: {translationId}");
                        
                        translations.Add(new StoryDefinitionTranslation
                        {
                            Id = translationId,
                            StoryDefinitionId = storyDefinitionId,
                            LanguageCode = language,
                            Title = storyData.Title
                        });
                        
                        Console.WriteLine($"[SEEDING] Added translation: {storyData.StoryId} ({language}) -> {storyDefinitionId}");
                    }
                    else if (storyData != null)
                    {
                        Console.WriteLine($"[SEEDING WARNING] Story {storyData.StoryId} not found in existing stories for language {language}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SEEDING ERROR] Error loading story translation from {filePath}: {ex.Message}");
                }
            }
        }

        Console.WriteLine($"[SEEDING] Total translations loaded: {translations.Count}");
        return translations;
    }

    private Guid GetStoryDefinitionIdByStoryId(string storyId)
    {
        // This is a simplified approach - in a real scenario, you might want to maintain a mapping
        // For now, we'll generate consistent IDs based on storyId
        return storyId switch
        {
            "learn-to-read-s1" => Guid.Parse("44444444-4444-4444-4444-444444444444"),
            _ => Guid.NewGuid()
        };
    }

    private static Guid GetFixedStoryHeroId(string heroId)
    {
        return heroId switch
        {
            "puf-puf" => Guid.Parse("00000000-0000-0000-0000-000000000100"),
            "linkaro" => Guid.Parse("11111111-1111-1111-1111-111111111100"),
            "grubot" => Guid.Parse("22222222-2222-2222-2222-222222222200"),
            _ => Guid.NewGuid()
        };
    }
}
