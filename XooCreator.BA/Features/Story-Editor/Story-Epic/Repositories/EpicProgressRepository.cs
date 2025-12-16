using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public class EpicProgressRepository : IEpicProgressRepository
{
    private readonly XooDbContext _context;

    public EpicProgressRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<EpicProgressDto>> GetEpicProgressAsync(Guid userId, string epicId)
    {
        return await _context.EpicProgress
            .Where(ep => ep.UserId == userId && ep.EpicId == epicId)
            .Select(ep => new EpicProgressDto
            {
                RegionId = ep.RegionId,
                IsUnlocked = ep.IsUnlocked,
                UnlockedAt = ep.IsUnlocked ? ep.UnlockedAt : null
            })
            .ToListAsync();
    }

    public async Task<List<EpicStoryProgressDto>> GetEpicStoryProgressAsync(Guid userId, string epicId)
    {
        var storyProgresses = await _context.EpicStoryProgress
            .Where(esp => esp.UserId == userId && esp.EpicId == epicId)
            .ToListAsync();

        return storyProgresses.Select(esp => new EpicStoryProgressDto
        {
            StoryId = esp.StoryId,
            SelectedAnswer = esp.SelectedAnswer,
            CompletedAt = esp.CompletedAt
        }).ToList();
    }

    public async Task<bool> CompleteStoryAsync(Guid userId, string epicId, string storyId, string? selectedAnswer = null)
    {
        try
        {
            var existingStory = await _context.EpicStoryProgress
                .FirstOrDefaultAsync(esp => esp.UserId == userId && esp.StoryId == storyId && esp.EpicId == epicId);

            if (existingStory != null)
            {
                return false; // Already completed
            }

            var storyProgress = new EpicStoryProgress
            {
                UserId = userId,
                StoryId = storyId,
                SelectedAnswer = selectedAnswer,
                TokensJson = null,
                EpicId = epicId
            };

            _context.EpicStoryProgress.Add(storyProgress);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UnlockRegionAsync(Guid userId, string epicId, string regionId)
    {
        try
        {
            var existingRegion = await _context.EpicProgress
                .FirstOrDefaultAsync(ep => ep.UserId == userId && ep.RegionId == regionId && ep.EpicId == epicId);

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
                var epicProgress = new EpicProgress
                {
                    UserId = userId,
                    RegionId = regionId,
                    IsUnlocked = true,
                    UnlockedAt = DateTime.UtcNow,
                    EpicId = epicId
                };
                _context.EpicProgress.Add(epicProgress);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ResetProgressAsync(Guid userId, string epicId)
    {
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            // Delete all epic progress for this user and epic
            await _context.EpicProgress
                .Where(ep => ep.UserId == userId && ep.EpicId == epicId)
                .ExecuteDeleteAsync();
            
            // Delete all epic story progress for this user and epic
            await _context.EpicStoryProgress
                .Where(esp => esp.UserId == userId && esp.EpicId == epicId)
                .ExecuteDeleteAsync();
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}

