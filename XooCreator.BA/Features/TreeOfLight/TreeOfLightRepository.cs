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
            // Check if story is already completed
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
            // Delete all user progress records
            await _context.TreeProgress.Where(tp => tp.UserId == userId).ExecuteDeleteAsync();
            await _context.StoryProgress.Where(sp => sp.UserId == userId).ExecuteDeleteAsync();
            await _context.HeroProgress.Where(hp => hp.UserId == userId).ExecuteDeleteAsync();
            await _context.HeroTreeProgress.Where(htp => htp.UserId == userId).ExecuteDeleteAsync();

            // Remove all existing token balances for this user
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
}
