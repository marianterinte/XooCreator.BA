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
            if (await _context.HeroDefinitionDefinitions.AnyAsync())
            {
                _logger.LogInformation("Hero definitions already present, skipping seed.");
                return;
            }

            _logger.LogWarning("HeroDefinition seed from JSON is disabled. Run migration scripts to populate definitions.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed hero definitions");
            throw;
        }
    }
}
