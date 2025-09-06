using Microsoft.EntityFrameworkCore;
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
        // Check if already seeded
        var existingRegionsCount = await _context.TreeRegions.CountAsync();
        if (existingRegionsCount > 0) return;

        // Seed Tree Regions (based on frontend world-model.ts)
        var regions = new[]
        {
            new TreeRegion 
            { 
                Id = "root", 
                Label = "Rădăcină", 
                ImageUrl = "images/biomes/farm.jpg",
                PufpufMessage = "Aici e Rădăcina, locul unde totul începe! Curajul tău va înflori aici ca o sămânță magică. 🌱",
                SortOrder = 1 
            },
            new TreeRegion 
            { 
                Id = "trunk", 
                Label = "Trunchi", 
                ImageUrl = "images/biomes/forest.jpg",
                PufpufMessage = "Trunchiul e inima copacului! Termină toate cele trei povești aici pentru a debloca prima ramură. Fiecare poveste îți va aduce mai aproape de următorul nivel! 💪",
                SortOrder = 2 
            },
            new TreeRegion 
            { 
                Id = "branch-1", 
                Label = "Ramura 1", 
                ImageUrl = "images/biomes/desert.jpg",
                PufpufMessage = "Ramura Întâi - Tărâmul Curajului! Felicitări că ai completat toate poveștile din Trunchi! Aici aventurierii curajoși își testează hotărârea. ⚔️",
                SortOrder = 3 
            },
            new TreeRegion 
            { 
                Id = "branch-2", 
                Label = "Ramura 2", 
                ImageUrl = "images/biomes/jungle.jpg",
                PufpufMessage = "Ramura a Doua - Grădina Înțelepciunii! Aici se nasc ideile strălucite și răspunsurile. 🧠",
                SortOrder = 4 
            },
            new TreeRegion 
            { 
                Id = "branch-3", 
                Label = "Ramura 3", 
                ImageUrl = "images/biomes/montain.jpg",
                PufpufMessage = "Ramura a Treia - Sanctuarul Creativității! Aici imaginația ta va înflori ca o floare rară. 🎨",
                SortOrder = 5 
            }
        };

        _context.TreeRegions.AddRange(regions);
        await _context.SaveChangesAsync();

        // Seed Tree Story Nodes (based on frontend world-model.ts)
        var storyNodes = new[]
        {
            new TreeStoryNode
            {
                StoryId = "root-s1",
                RegionId = "root",
                RewardImageUrl = "images/homepage/heroes/hero0.jpg",
                SortOrder = 1
            },
            new TreeStoryNode
            {
                StoryId = "trunk-s1",
                RegionId = "trunk",
                RewardImageUrl = "images/homepage/heroes/hero1.jpg",
                SortOrder = 1
            },
            new TreeStoryNode
            {
                StoryId = "trunk-s2",
                RegionId = "trunk",
                RewardImageUrl = "images/homepage/heroes/hero2.jpg",
                SortOrder = 2
            },
            new TreeStoryNode
            {
                StoryId = "trunk-s3",
                RegionId = "trunk",
                RewardImageUrl = "images/homepage/heroes/hero0.jpg",
                SortOrder = 3
            }
        };

        _context.TreeStoryNodes.AddRange(storyNodes);
        await _context.SaveChangesAsync();

        // Seed Tree Unlock Rules (based on frontend world-model.ts)
        var unlockRules = new[]
        {
            // Unlock trunk after root-s1 story completion
            new TreeUnlockRule
            {
                Type = "story",
                FromId = "root-s1", // story ID
                ToRegionId = "trunk",
                StoryId = "root-s1",
                SortOrder = 1
            },
            // Unlock branch-1 after ALL three trunk stories are completed
            new TreeUnlockRule
            {
                Type = "all",
                FromId = "trunk", // region ID
                ToRegionId = "branch-1",
                RequiredStoriesCsv = "trunk-s1,trunk-s2,trunk-s3",
                SortOrder = 2
            }
            // Future: branch-2, branch-3 unlock rules can be added here
        };

        _context.TreeUnlockRules.AddRange(unlockRules);
        await _context.SaveChangesAsync();
    }
}
