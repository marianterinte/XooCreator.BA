using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeModelRepository
{
    Task<List<TreeRegion>> GetAllRegionsAsync(string configId);
    Task<List<TreeStoryNode>> GetAllStoryNodesAsync(string configId);
    Task<List<TreeUnlockRule>> GetAllUnlockRulesAsync(string configId);
    Task<List<TreeConfiguration>> GetAllConfigurationsAsync();
    Task SeedTreeModelAsync();
}

public class TreeModelRepository : ITreeModelRepository
{
    private readonly XooDbContext _context;

    public TreeModelRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<TreeConfiguration>> GetAllConfigurationsAsync()
    {
        return await _context.TreeConfigurations.ToListAsync();
    }

    public async Task<List<TreeRegion>> GetAllRegionsAsync(string configId)
    {
        return await _context.TreeRegions
            .Where(r => r.TreeConfigurationId == configId)
            .OrderBy(r => r.SortOrder)
            .ToListAsync();
    }

    public async Task<List<TreeStoryNode>> GetAllStoryNodesAsync(string configId)
    {
        return await _context.TreeStoryNodes
            .Where(sn => sn.TreeConfigurationId == configId)
            .Include(sn => sn.Region)
            .Include(sn => sn.StoryDefinition)
                .ThenInclude(sd => sd.Translations)
            .OrderBy(sn => sn.RegionId)
            .ThenBy(sn => sn.SortOrder)
            .ToListAsync();
    }

    public async Task<List<TreeUnlockRule>> GetAllUnlockRulesAsync(string configId)
    {
        return await _context.TreeUnlockRules
            .Where(ur => ur.TreeConfigurationId == configId)
            .OrderBy(ur => ur.SortOrder)
            .ToListAsync();
    }

    public async Task SeedTreeModelAsync()
    {
        try
        {
            if (await _context.TreeConfigurations.AnyAsync()) return;

            var seeds = await LoadSeedAsync();

            foreach (var seed in seeds)
            {
                var config = new TreeConfiguration
                {
                    Id = seed.Configuration.Id,
                    Name = seed.Configuration.Name,
                    IsDefault = seed.Configuration.IsDefault
                };
                _context.TreeConfigurations.Add(config);

                var regions = seed.Regions
                    .OrderBy(r => r.SortOrder)
                    .Select(r => new TreeRegion
                    {
                        Id = r.Id,
                        Label = string.Empty,
                        ImageUrl = r.ImageUrl,
                        PufpufMessage = string.Empty,
                        SortOrder = r.SortOrder,
                        TreeConfigurationId = config.Id
                    }).ToList();
                _context.TreeRegions.AddRange(regions);

                // Validate that all referenced stories exist before creating TreeStoryNodes
                var referencedStoryIds = seed.StoryNodes.Select(sn => sn.StoryId).Distinct().ToList();
                var existingStoryIds = await _context.StoryDefinitions
                    .Where(sd => referencedStoryIds.Contains(sd.StoryId))
                    .Select(sd => sd.StoryId)
                    .ToListAsync();

                var missingStoryIds = referencedStoryIds.Except(existingStoryIds).ToList();
                if (missingStoryIds.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"TreeModel references stories that don't exist in StoryDefinitions: {string.Join(", ", missingStoryIds)}. " +
                        "Make sure StoriesService.InitializeStoriesAsync() is called BEFORE TreeModelService.InitializeTreeModelAsync().");
                }

                var storyNodes = seed.StoryNodes
                    .OrderBy(sn => sn.RegionId)
                    .ThenBy(sn => sn.SortOrder)
                    .Select(sn => new TreeStoryNode
                    {
                        StoryId = sn.StoryId,
                        RegionId = sn.RegionId,
                        RewardImageUrl = sn.RewardImageUrl,
                        SortOrder = sn.SortOrder,
                        TreeConfigurationId = config.Id
                    }).ToList();
                _context.TreeStoryNodes.AddRange(storyNodes);

                var unlockRules = seed.UnlockRules
                    .OrderBy(ur => ur.SortOrder)
                    .Select(ur => new TreeUnlockRule
                    {
                        Type = ur.Type,
                        FromId = ur.FromId,
                        ToRegionId = ur.ToRegionId,
                        RequiredStoriesCsv = ur.RequiredStoriesCsv,
                        MinCount = ur.MinCount,
                        StoryId = ur.StoryId,
                        SortOrder = ur.SortOrder,
                        TreeConfigurationId = config.Id
                    }).ToList();
                _context.TreeUnlockRules.AddRange(unlockRules);
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error seeding tree model data: " + ex.Message, ex);
        }
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private static async Task<List<TreeModelSeedRoot>> LoadSeedAsync()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var configsDir = Path.Combine(baseDir, "Data", "SeedData", "SharedConfigs", "TreeOfLight");

        if (!Directory.Exists(configsDir))
        {
            throw new FileNotFoundException($"Tree model seed directory not found: {configsDir}");
        }

        var seeds = new List<TreeModelSeedRoot>();
        var files = Directory.GetFiles(configsDir, "*.json");

        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file);
            var data = JsonSerializer.Deserialize<TreeModelSeedRoot>(json, _jsonOptions);
            if (data == null)
            {
                throw new InvalidOperationException($"Failed to deserialize tree model seed data from {file}.");
            }
            ValidateSeed(data);
            seeds.Add(data);
        }

        return seeds;
    }

    private static void ValidateSeed(TreeModelSeedRoot data)
    {
        var regionIds = new HashSet<string>(data.Regions.Select(r => r.Id));
        foreach (var node in data.StoryNodes)
        {
            if (!regionIds.Contains(node.RegionId))
            {
                throw new InvalidOperationException($"StoryNode '{node.StoryId}' references unknown region '{node.RegionId}'.");
            }
        }

        foreach (var rule in data.UnlockRules)
        {
            if (string.IsNullOrWhiteSpace(rule.Type) || string.IsNullOrWhiteSpace(rule.FromId) || string.IsNullOrWhiteSpace(rule.ToRegionId))
            {
                throw new InvalidOperationException("UnlockRule has missing mandatory fields (type/fromId/toRegionId).");
            }
            if (!regionIds.Contains(rule.ToRegionId))
            {
                throw new InvalidOperationException($"UnlockRule targets unknown region '{rule.ToRegionId}'.");
            }
        }
    }

    private class TreeModelSeedRoot
    {
        public TreeConfigurationSeed Configuration { get; set; } = new();
        public List<TreeRegionSeed> Regions { get; set; } = new();
        public List<TreeStoryNodeSeed> StoryNodes { get; set; } = new();
        public List<TreeUnlockRuleSeed> UnlockRules { get; set; } = new();
    }

    private class TreeConfigurationSeed
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

    private class TreeRegionSeed
    {
        public string Id { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; }
    }

    private class TreeStoryNodeSeed
    {
        public string StoryId { get; set; } = string.Empty;
        public string RegionId { get; set; } = string.Empty;
        public string? RewardImageUrl { get; set; }
        public int SortOrder { get; set; }
    }

    private class TreeUnlockRuleSeed
    {
        public string Type { get; set; } = string.Empty; // story, all, any
        public string FromId { get; set; } = string.Empty; // region or story id
        public string ToRegionId { get; set; } = string.Empty; // target region
        public string? RequiredStoriesCsv { get; set; } // for all/any
        public int? MinCount { get; set; } // for any
        public string? StoryId { get; set; } // for story
        public int SortOrder { get; set; }
    }
}