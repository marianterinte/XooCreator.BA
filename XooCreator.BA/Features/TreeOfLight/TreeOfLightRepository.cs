using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Features.Stories;

namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeOfLightRepository
{
    Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId);
    Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId);

    Task<bool> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, Stories.StoryContentDto? story);
    Task<bool> UnlockRegionAsync(Guid userId, string regionId);
    Task ResetUserProgressAsync(Guid userId);
}

public class TreeOfLightRepository : ITreeOfLightRepository
{
    private readonly XooDbContext _context;

    public TreeOfLightRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId)
    {
        return await _context.TreeProgress
            .Where(tp => tp.UserId == userId)
            .Select(tp => new TreeProgressDto
            {
                RegionId = tp.RegionId,
                IsUnlocked = tp.IsUnlocked,
                UnlockedAt = tp.IsUnlocked ? tp.UnlockedAt : null
            })
            .ToListAsync();
    }

    public async Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId)
    {
        var storyProgresses = await _context.StoryProgress
            .Where(sp => sp.UserId == userId)
            .ToListAsync();

        return storyProgresses.Select(sp => new StoryProgressDto
        {
            StoryId = sp.StoryId,
            SelectedAnswer = sp.SelectedAnswer,
            Tokens = !string.IsNullOrEmpty(sp.TokensJson)
                ? JsonSerializer.Deserialize<List<TokenReward>>(sp.TokensJson) ?? new List<TokenReward>()
                : new List<TokenReward>(),
            CompletedAt = sp.CompletedAt
        }).ToList();
    }




    public async Task<bool> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, Stories.StoryContentDto? story)
    {
        try
        {
            // Check if story is already completed
            var existingStory = await _context.StoryProgress
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.StoryId == request.StoryId);

            if (existingStory != null)
            {
                return false; // Already completed
            }

            var storyProgress = new StoryProgress
            {
                UserId = userId,
                StoryId = request.StoryId,
                SelectedAnswer = request.SelectedAnswer,
                TokensJson = GetTokens(story, request.SelectedAnswer)
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

    public async Task<bool> UnlockRegionAsync(Guid userId, string regionId)
    {
        try
        {
            var existingRegion = await _context.TreeProgress
                .FirstOrDefaultAsync(tp => tp.UserId == userId && tp.RegionId == regionId);

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
                    UnlockedAt = DateTime.UtcNow
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

            // Reset user tokens to default values (5 tokens of each type)
            var userTokens = await _context.UserTokens.FirstOrDefaultAsync(ut => ut.UserId == userId);
            if (userTokens != null)
            {
                userTokens.Courage = 5;
                userTokens.Curiosity = 5;
                userTokens.Thinking = 5;
                userTokens.Creativity = 5;
                userTokens.Safety = 5;
                userTokens.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create default tokens if they don't exist (5 tokens of each type)
                _context.UserTokens.Add(new UserTokens
                {
                    UserId = userId,
                    Courage = 5,
                    Curiosity = 5,
                    Thinking = 5,
                    Creativity = 5,
                    Safety = 5,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

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
