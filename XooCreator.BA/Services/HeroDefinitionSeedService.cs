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
    private readonly IHeroTreeProvider _heroTreeProvider;

    public HeroDefinitionSeedService(XooDbContext context, ILogger<HeroDefinitionSeedService> logger, IHeroTreeProvider heroTreeProvider)
    {
        _context = context;
        _logger = logger;
        _heroTreeProvider = heroTreeProvider;
    }

    public async Task SeedHeroDefinitionsAsync()
    {
        try
        {
            if (await _context.HeroDefinitions.AnyAsync())
            {
                _logger.LogInformation("Hero definitions already seeded, skipping...");
                return;
            }

            _logger.LogInformation("Seeding hero definitions...");

            var heroTreeStructure = await _heroTreeProvider.GetHeroTreeStructure();
            if (heroTreeStructure?.Nodes == null || !heroTreeStructure.Nodes.Any())
            {
                _logger.LogWarning("No hero definitions found in hero tree structure file");
                return;
            }

            var heroDefinitions = heroTreeStructure.Nodes.Select(node => new HeroDefinition
            {
                Id = node.Id,
                Type = node.Type,
                CourageCost = node.Costs?.Courage ?? 0,
                CuriosityCost = node.Costs?.Curiosity ?? 0,
                ThinkingCost = node.Costs?.Thinking ?? 0,
                CreativityCost = node.Costs?.Creativity ?? 0,
                SafetyCost = node.Costs?.Safety ?? 0,
                PrerequisitesJson = JsonSerializer.Serialize(node.Prerequisites ?? new List<string>()),
                RewardsJson = JsonSerializer.Serialize(node.Rewards ?? new List<string>()),
                IsUnlocked = node.IsUnlocked,
                PositionX = node.PositionX,
                PositionY = node.PositionY,
                Image = node.Image,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            _context.HeroDefinitions.AddRange(heroDefinitions);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {Count} hero definitions", heroDefinitions.Count);

            // Seed translations
            foreach (var languageCode in LanguageCodeExtensions.All())
            {
                var heroTreeData = await _heroTreeProvider.GetHeroTree(languageCode);
                if (heroTreeData?.Nodes == null) continue;

                var translations = heroTreeData.Nodes.Select(node => new HeroDefinitionTranslation
                {
                    HeroDefinitionId = node.Id,
                    LanguageCode = languageCode.ToTag(),
                    Name = node.Name,
                    Description = node.Description,
                    Story = node.Story
                }).ToList();

                _context.HeroDefinitionTranslations.AddRange(translations);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully seeded hero definition translations");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed hero definitions");
            throw;
        }
    }
}
