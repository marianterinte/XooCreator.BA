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

            // Convert to entities
            var heroDefinitions = heroData.Nodes.Select(node => new HeroDefinition
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
        public int Safety { get; set; }
    }
}
