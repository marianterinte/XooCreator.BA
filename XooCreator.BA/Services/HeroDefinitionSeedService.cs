using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Services;

public interface IHeroDefinitionSeedService
{
    Task SeedHeroDefinitionsAsync();
}

public class HeroDefinitionSeedService : IHeroDefinitionSeedService
{
    private readonly XooDbContext _context;
    private readonly ILogger<HeroDefinitionSeedService> _logger;

    public HeroDefinitionSeedService(XooDbContext context, ILogger<HeroDefinitionSeedService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedHeroDefinitionsAsync()
    {
        try
        {
            // Check if data already exists
            if (await _context.HeroDefinitions.AnyAsync())
            {
                _logger.LogInformation("Hero definitions already seeded, skipping...");
                return;
            }

            _logger.LogInformation("Seeding hero definitions...");

            // Load hero definitions from JSON file
            var heroData = await LoadHeroDefinitionsFromJsonAsync();
            
            if (heroData?.Nodes == null || !heroData.Nodes.Any())
            {
                _logger.LogWarning("No hero definitions found in JSON file");
                return;
            }

            // Convert to entities with calculated positions
            var heroDefinitions = heroData.Nodes.Select((node, index) => 
            {
                var (positionX, positionY) = CalculateNodePosition(node, index, heroData.Nodes.Count);
                return new HeroDefinition
                {
                    Id = node.Id,
                    Name = node.Name,
                    Description = node.Description,
                    Type = node.Type,
                    CourageCost = node.Costs?.Courage ?? 0,
                    CuriosityCost = node.Costs?.Curiosity ?? 0,
                    ThinkingCost = node.Costs?.Thinking ?? 0,
                    CreativityCost = node.Costs?.Creativity ?? 0,
                    SafetyCost = node.Costs?.Safety ?? 0,
                    PrerequisitesJson = JsonSerializer.Serialize(node.Prerequisites ?? new List<string>()),
                    RewardsJson = JsonSerializer.Serialize(node.Rewards ?? new List<string>()),
                    IsUnlocked = node.IsUnlocked,
                    PositionX = positionX,
                    PositionY = positionY,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }).ToList();

            // Add to database
            _context.HeroDefinitions.AddRange(heroDefinitions);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {Count} hero definitions", heroDefinitions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed hero definitions");
            throw;
        }
    }

    private async Task<HeroTreeData?> LoadHeroDefinitionsFromJsonAsync()
    {
        try
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "hero-tree.json");
            
            if (!File.Exists(jsonPath))
            {
                _logger.LogError("Hero tree JSON file not found at path: {Path}", jsonPath);
                return null;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            return JsonSerializer.Deserialize<HeroTreeData>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load hero definitions from JSON file");
            return null;
        }
    }

    // DTOs for JSON deserialization
    private class HeroTreeData
    {
        public List<string> BaseHeroIds { get; set; } = new();
        public List<HeroNodeData> Nodes { get; set; } = new();
    }

    private class HeroNodeData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public HeroCosts? Costs { get; set; }
        public List<string>? Prerequisites { get; set; }
        public List<string>? Rewards { get; set; }
        public bool IsUnlocked { get; set; }
    }

    private class HeroCosts
    {
        public int Courage { get; set; }
        public int Curiosity { get; set; }
        public int Thinking { get; set; }
        public int Creativity { get; set; }
        public int Safety { get; set;         }
    }

    private (double positionX, double positionY) CalculateNodePosition(HeroNodeData node, int index, int totalNodes)
    {
        // Define base heroes positions - VERTICAL LAYOUT
        var baseHeroPositions = new Dictionary<string, (double x, double y)>
        {
            { "seed", (0, 0) }, // Center
            { "hero_brave_puppy", (-80, -200) }, // Left side, higher
            { "hero_curious_cat", (80, -200) }, // Right side, higher  
            { "hero_wise_owl", (0, -250) }, // Top center
            { "hero_playful_horse", (-80, -300) }, // Left side, lower
            { "hero_cautious_hedgehog", (80, -300) } // Right side, lower
        };

        // If it's a base hero, use predefined position
        if (baseHeroPositions.TryGetValue(node.Id, out var basePos))
        {
            return (basePos.x, basePos.y);
        }

        // For other nodes, calculate VERTICAL positioning
        var prerequisites = node.Prerequisites ?? new List<string>();
        var isLegendary = node.Id == "hero_alchimalian_dragon";

        // VERTICAL LAYOUT: position nodes in vertical tiers
        if (isLegendary)
        {
            // Legendary at the very top
            return (0, -450);
        }
        else if (prerequisites.Count == 2)
        {
            // Hybrid nodes - position between their prerequisites but further up
            var prereq1 = prerequisites[0];
            var prereq2 = prerequisites[1];
            
            if (baseHeroPositions.TryGetValue(prereq1, out var pos1) && 
                baseHeroPositions.TryGetValue(prereq2, out var pos2))
            {
                // Average X position of prerequisites
                var avgX = (pos1.x + pos2.x) / 2.0;
                // Position higher (more negative Y) than both prerequisites
                var avgY = Math.Min(pos1.y, pos2.y) - 120; // 120px above the higher prerequisite
                
                return (avgX, avgY);
            }
        }

        // Fallback: position in vertical tiers based on index
        var tier = (index - 6) / 3; // Each tier has max 3 nodes
        var positionInTier = (index - 6) % 3;
        
        var x = positionInTier switch
        {
            0 => -100, // Left
            1 => 0,    // Center
            2 => 100,  // Right
            _ => 0
        };
        
        var y = -350 - (tier * 80); // Each tier is 80px apart, going up

        return (x, y);
    }
}
