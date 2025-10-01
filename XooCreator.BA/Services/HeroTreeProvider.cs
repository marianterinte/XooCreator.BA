using System.Text.Json;
using XooCreator.BA.Data;

namespace XooCreator.BA.Services;

public interface IHeroTreeProvider
{
    Task<HeroTreeData?> GetHeroTree(LanguageCode languageCode);
    Task<HeroTreeStructure?> GetHeroTreeStructure();
}

public class HeroTreeProvider : IHeroTreeProvider
{
    private readonly ILogger<HeroTreeProvider> _logger;

    public HeroTreeProvider(ILogger<HeroTreeProvider> logger)
    {
        _logger = logger;
    }

    public async Task<HeroTreeData?> GetHeroTree(LanguageCode languageCode)
    {
        try
        {
            var structure = await GetHeroTreeStructure();
            if (structure == null) return null;

            var translations = await LoadTranslations(languageCode);
            if (translations == null) return null;

            var nodes = structure.Nodes.Select(n =>
            {
                translations.TryGetValue(n.NameKey, out var name);
                translations.TryGetValue(n.DescriptionKey, out var description);
                translations.TryGetValue(n.StoryKey, out var story);

                return new HeroNodeData
                {
                    Id = n.Id,
                    Name = name ?? n.NameKey,
                    Description = description ?? n.DescriptionKey,
                    Story = story ?? n.StoryKey,
                    Type = n.Type,
                    Costs = n.Costs,
                    Prerequisites = n.Prerequisites,
                    Rewards = n.Rewards,
                    IsUnlocked = n.IsUnlocked,
                    PositionX = n.PositionX,
                    PositionY = n.PositionY,
                    Image = n.Image
                };
            }).ToList();

            return new HeroTreeData
            {
                RootHeroId = structure.RootHeroId,
                BaseHeroIds = structure.BaseHeroIds,
                Nodes = nodes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get hero tree for language {Language}", languageCode);
            return null;
        }
    }

    public async Task<HeroTreeStructure?> GetHeroTreeStructure()
    {
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "SharedConfigs", "hero-tree.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogError("Hero tree structure file not found at path: {Path}", jsonPath);
                return null;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            return JsonSerializer.Deserialize<HeroTreeStructure>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load hero tree structure from JSON file");
            return null;
        }
    }

    private async Task<Dictionary<string, string>?> LoadTranslations(LanguageCode languageCode)
    {
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "Translations", languageCode.ToFolder(), "hero-tree.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("Hero tree translation file not found for language {Language} at path: {Path}", languageCode, jsonPath);
                return new Dictionary<string, string>();
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load hero tree translations for language {Language}", languageCode);
            return null;
        }
    }
}

// DTOs for JSON deserialization
public class HeroTreeStructure
{
    public string RootHeroId { get; set; } = string.Empty;
    public List<string> BaseHeroIds { get; set; } = new();
    public List<HeroNodeStructure> Nodes { get; set; } = new();
}

public class HeroNodeStructure
{
    public string Id { get; set; } = string.Empty;
    public string NameKey { get; set; } = string.Empty;
    public string DescriptionKey { get; set; } = string.Empty;
    public string StoryKey { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public HeroCosts? Costs { get; set; }
    public List<string>? Prerequisites { get; set; }
    public List<string>? Rewards { get; set; }
    public bool IsUnlocked { get; set; }
    public double PositionX { get; set; }
    public double PositionY { get; set; }
}

public class HeroTreeData
{
    public string RootHeroId { get; set; } = string.Empty;
    public List<string> BaseHeroIds { get; set; } = new();
    public List<HeroNodeData> Nodes { get; set; } = new();
}

public class HeroNodeData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public HeroCosts? Costs { get; set; }
    public List<string>? Prerequisites { get; set; }
    public List<string>? Rewards { get; set; }
    public bool IsUnlocked { get; set; }
    public double PositionX { get; set; }
    public double PositionY { get; set; }
}

public class HeroCosts
{
    public int Courage { get; set; }
    public int Curiosity { get; set; }
    public int Thinking { get; set; }
    public int Creativity { get; set; }
    public int Safety { get; set; }
}
