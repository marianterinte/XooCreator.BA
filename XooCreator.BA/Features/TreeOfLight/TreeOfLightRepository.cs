using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Features.Stories;

namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeOfLightRepository
{
    Task<List<TreeConfiguration>> GetAllConfigurationsAsync();
    Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId, string configId);
    Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId, string configId);

    Task<bool> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, Stories.StoryContentDto? story, string configId);
    Task<bool> UnlockRegionAsync(Guid userId, string regionId, string configId);
    Task ResetUserProgressAsync(Guid userId);
    
    Task<List<StoryHero>> GetStoryHeroesAsync();
    Task<bool> IsHeroUnlockedAsync(Guid userId, string heroId);
    Task<bool> UnlockHeroAsync(Guid userId, string heroId, string heroType);
    
    Task<List<HeroMessage>> GetHeroMessagesAsync();
    Task<List<HeroClickMessage>> GetHeroClickMessagesAsync();
    Task<HeroMessage?> GetHeroMessageAsync(string heroId, string regionId);
    Task<HeroClickMessage?> GetHeroClickMessageAsync(string heroId);
}

public class TreeOfLightRepository : ITreeOfLightRepository
{
    private readonly XooDbContext _context;

    public TreeOfLightRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<TreeConfiguration>> GetAllConfigurationsAsync()
    {
        return await _context.TreeConfigurations.ToListAsync();
    }

    public async Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId, string configId)
    {
        return await _context.TreeProgress
            .Where(tp => tp.UserId == userId && tp.TreeConfigurationId == configId)
            .Select(tp => new TreeProgressDto
            {
                RegionId = tp.RegionId,
                IsUnlocked = tp.IsUnlocked,
                UnlockedAt = tp.IsUnlocked ? tp.UnlockedAt : null
            })
            .ToListAsync();
    }

    public async Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId, string configId)
    {
        var storyProgresses = await _context.StoryProgress
            .Where(sp => sp.UserId == userId && sp.TreeConfigurationId == configId)
            .ToListAsync();

        return storyProgresses.Select(sp => new StoryProgressDto
        {
            StoryId = sp.StoryId,
            SelectedAnswer = sp.SelectedAnswer,
            Tokens = new List<TokenReward>(),
            CompletedAt = sp.CompletedAt
        }).ToList();
    }




    public async Task<bool> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, Stories.StoryContentDto? story, string configId)
    {
        try
        {
            var existingStory = await _context.StoryProgress
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.StoryId == request.StoryId && sp.TreeConfigurationId == configId);

            if (existingStory != null)
            {
                return false; // Already completed
            }

            var storyProgress = new StoryProgress
            {
                UserId = userId,
                StoryId = request.StoryId,
                SelectedAnswer = request.SelectedAnswer,
                TokensJson = null,
                TreeConfigurationId = configId
            };

            _context.StoryProgress.Add(storyProgress);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GetTokens(StoryContentDto? story, string? selectedAnswer)
    {

        if (story == null || string.IsNullOrEmpty(selectedAnswer))
            return null;

        if (story.Tiles.Where(t => t.Type == "quiz")
                    .SelectMany(t => t.Answers)
                    .Where(a => a.Id == selectedAnswer)
                    .SelectMany(a => a.Tokens)
                    .Any())
        {
            return JsonSerializer.Serialize(
                            story.Tiles
                                .Where(t => t.Type == "quiz")
                                .SelectMany(t => t.Answers)
                                .Where(a => a.Id == selectedAnswer)
                                .SelectMany(a => a.Tokens));
        }
        return null;
    }

    public async Task<bool> UnlockRegionAsync(Guid userId, string regionId, string configId)
    {
        try
        {
            var existingRegion = await _context.TreeProgress
                .FirstOrDefaultAsync(tp => tp.UserId == userId && tp.RegionId == regionId && tp.TreeConfigurationId == configId);

            if (existingRegion?.IsUnlocked == true)
            {
                return false; // Already unlocked
            }

            if (existingRegion != null)
            {
                existingRegion.IsUnlocked = true;
                existingRegion.UnlockedAt = DateTime.UtcNow;
                existingRegion.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var treeProgress = new TreeProgress
                {
                    UserId = userId,
                    RegionId = regionId,
                    IsUnlocked = true,
                    UnlockedAt = DateTime.UtcNow,
                    TreeConfigurationId = configId
                };
                _context.TreeProgress.Add(treeProgress);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }





    public async Task ResetUserProgressAsync(Guid userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.TreeProgress.Where(tp => tp.UserId == userId).ExecuteDeleteAsync();
            await _context.StoryProgress.Where(sp => sp.UserId == userId).ExecuteDeleteAsync();
            await _context.HeroProgress.Where(hp => hp.UserId == userId).ExecuteDeleteAsync();
            await _context.HeroTreeProgress.Where(htp => htp.UserId == userId).ExecuteDeleteAsync();

            await _context.UserTokenBalances
                .Where(b => b.UserId == userId)
                .ExecuteDeleteAsync();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<StoryHero>> GetStoryHeroesAsync()
    {
        return await _context.StoryHeroes
            .Where(sh => sh.IsActive)
            .ToListAsync();
    }

    public async Task<bool> IsHeroUnlockedAsync(Guid userId, string heroId)
    {
        return await _context.HeroProgress
            .AnyAsync(hp => hp.UserId == userId && hp.HeroId == heroId);
    }

    public async Task<bool> UnlockHeroAsync(Guid userId, string heroId, string heroType)
    {
        var existingProgress = await _context.HeroProgress
            .FirstOrDefaultAsync(hp => hp.UserId == userId && hp.HeroId == heroId);
        
        if (existingProgress != null)
        {
            return false; // Already unlocked
        }

        var heroProgress = new HeroProgress
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HeroId = heroId,
            HeroType = heroType,
            SourceStoryId = string.Empty, // Will be set by caller if needed
            UnlockedAt = DateTime.UtcNow
        };

        _context.HeroProgress.Add(heroProgress);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<List<HeroMessage>> GetHeroMessagesAsync()
    {
        return await _context.HeroMessages
            .Where(hm => hm.IsActive)
            .ToListAsync();
    }

    public async Task<List<HeroClickMessage>> GetHeroClickMessagesAsync()
    {
        return await _context.HeroClickMessages
            .Where(hcm => hcm.IsActive)
            .ToListAsync();
    }

    public async Task<HeroMessage?> GetHeroMessageAsync(string heroId, string regionId)
    {
        return await _context.HeroMessages
            .FirstOrDefaultAsync(hm => hm.HeroId == heroId && hm.RegionId == regionId && hm.IsActive);
    }

    public async Task<HeroClickMessage?> GetHeroClickMessageAsync(string heroId)
    {
        return await _context.HeroClickMessages
            .FirstOrDefaultAsync(hcm => hcm.HeroId == heroId && hcm.IsActive);
    }
}
