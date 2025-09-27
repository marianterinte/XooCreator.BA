using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeModelRepository
{
    Task<List<TreeRegion>> GetAllRegionsAsync();
    Task<List<TreeStoryNode>> GetAllStoryNodesAsync();
    Task<List<TreeUnlockRule>> GetAllUnlockRulesAsync();
    Task SeedTreeModelAsync();
}

public class TreeModelRepository : ITreeModelRepository
{
    private readonly XooDbContext _context;
    private const string LegacySeedFilePath = "Data/SeedData/tree-model-seed.json";

    public TreeModelRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<TreeRegion>> GetAllRegionsAsync()
    {
        return await _context.TreeRegions
            .OrderBy(r => r.SortOrder)
            .ToListAsync();
    }

    public async Task<List<TreeStoryNode>> GetAllStoryNodesAsync()
    {
        return await _context.TreeStoryNodes
            .Include(sn => sn.Region)
            .Include(sn => sn.StoryDefinition)
            .OrderBy(sn => sn.RegionId)
            .ThenBy(sn => sn.SortOrder)
            .ToListAsync();
    }

    public async Task<List<TreeUnlockRule>> GetAllUnlockRulesAsync()
    {
        return await _context.TreeUnlockRules
            .OrderBy(ur => ur.SortOrder)
            .ToListAsync();
    }

    public async Task SeedTreeModelAsync()
    {
        // Already seeded? (regions act as presence flag)
        if (await _context.TreeRegions.AnyAsync()) return;

        var seed = await LoadSeedAsync();

        // Map and persist
        var regions = seed.Regions
            .OrderBy(r => r.SortOrder)
            .Select(r => new TreeRegion
            {
                Id = r.Id,
                Label = r.Label,
                ImageUrl = r.ImageUrl,
                PufpufMessage = r.PufpufMessage,
                SortOrder = r.SortOrder
            }).ToList();

        await _context.TreeRegions.AddRangeAsync(regions);
        await _context.SaveChangesAsync();

        var storyNodes = seed.StoryNodes
            .OrderBy(sn => sn.RegionId)
            .ThenBy(sn => sn.SortOrder)
            .Select(sn => new TreeStoryNode
            {
                StoryId = sn.StoryId,
                RegionId = sn.RegionId,
                RewardImageUrl = sn.RewardImageUrl,
                SortOrder = sn.SortOrder
            }).ToList();

        await _context.TreeStoryNodes.AddRangeAsync(storyNodes);
        await _context.SaveChangesAsync();

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
                SortOrder = ur.SortOrder
            }).ToList();

        await _context.TreeUnlockRules.AddRangeAsync(unlockRules);
        await _context.SaveChangesAsync();
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private static async Task<TreeModelSeedRoot> LoadSeedAsync()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var candidates = new[]
        {
            Path.Combine(baseDir, "Data", "SeedData", "ro-ro", "tree-model-seed.json"),
            Path.Combine(baseDir, LegacySeedFilePath)
        };

        string? pathToUse = candidates.FirstOrDefault(File.Exists);
        if (string.IsNullOrEmpty(pathToUse))
        {
            throw new FileNotFoundException($"Tree model seed file not found. Checked: {string.Join(", ", candidates)}")
            {
                HResult = 404
            };
        }

        var json = await File.ReadAllTextAsync(pathToUse);
        var data = JsonSerializer.Deserialize<TreeModelSeedRoot>(json, _jsonOptions);
        if (data == null)
        {
            throw new InvalidOperationException("Failed to deserialize tree model seed data.");
        }
        ValidateSeed(data);
        return data;
    }

    private static void ValidateSeed(TreeModelSeedRoot data)
    {
        // Basic validation to prevent silent bad data
        var regionIds = new HashSet<string>(data.Regions.Select(r => r.Id));
        foreach (var node in data.StoryNodes)
        {
            if (!regionIds.Contains(node.RegionId))
            {
                throw new InvalidOperationException($"StoryNode '{node.StoryId}' references unknown region '{node.RegionId}'.")
                {
                    HResult = 400
                };
            }
        }

        foreach (var rule in data.UnlockRules)
        {
            if (string.IsNullOrWhiteSpace(rule.Type) || string.IsNullOrWhiteSpace(rule.FromId) || string.IsNullOrWhiteSpace(rule.ToRegionId))
            {
                throw new InvalidOperationException("UnlockRule has missing mandatory fields (type/fromId/toRegionId).")
                {
                    HResult = 400
                };
            }
            if (!regionIds.Contains(rule.ToRegionId))
            {
                throw new InvalidOperationException($"UnlockRule targets unknown region '{rule.ToRegionId}'.")
                {
                    HResult = 400
                };
            }
        }
    }

    // Seed DTOs
    private class TreeModelSeedRoot
    {
        public List<TreeRegionSeed> Regions { get; set; } = new();
        public List<TreeStoryNodeSeed> StoryNodes { get; set; } = new();
        public List<TreeUnlockRuleSeed> UnlockRules { get; set; } = new();
    }

    private class TreeRegionSeed
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? PufpufMessage { get; set; }
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
