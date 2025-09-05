using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeOfLightRepository
{
    Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId);
    Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId);
    Task<UserTokensDto> GetUserTokensAsync(Guid userId);
    Task<List<HeroDto>> GetHeroProgressAsync(Guid userId);
    Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId);
    
    Task<bool> CompleteStoryAsync(Guid userId, CompleteStoryRequest request);
    Task<bool> UnlockRegionAsync(Guid userId, string regionId);
    Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0);
    Task<bool> UnlockHeroAsync(Guid userId, string heroId, string heroType, string? sourceStoryId = null);
    Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request);
    Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0);
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
        return await _context.StoryProgress
            .Where(sp => sp.UserId == userId)
            .Select(sp => new StoryProgressDto
            {
                StoryId = sp.StoryId,
                SelectedAnswer = sp.SelectedAnswer,
                RewardReceived = sp.RewardReceived,
                CompletedAt = sp.CompletedAt
            })
            .ToListAsync();
    }

    public async Task<UserTokensDto> GetUserTokensAsync(Guid userId)
    {
        var tokens = await _context.UserTokens
            .FirstOrDefaultAsync(ut => ut.UserId == userId);

        if (tokens == null)
        {
            // Create default tokens entry
            tokens = new UserTokens
            {
                UserId = userId,
                Courage = 0,
                Curiosity = 0,
                Thinking = 0,
                Creativity = 0
            };
            _context.UserTokens.Add(tokens);
            await _context.SaveChangesAsync();
        }

        return new UserTokensDto
        {
            Courage = tokens.Courage,
            Curiosity = tokens.Curiosity,
            Thinking = tokens.Thinking,
            Creativity = tokens.Creativity
        };
    }

    public async Task<List<HeroDto>> GetHeroProgressAsync(Guid userId)
    {
        return await _context.HeroProgress
            .Where(hp => hp.UserId == userId)
            .Select(hp => new HeroDto
            {
                HeroId = hp.HeroId,
                HeroType = hp.HeroType,
                SourceStoryId = hp.SourceStoryId,
                UnlockedAt = hp.UnlockedAt
            })
            .ToListAsync();
    }

    public async Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId)
    {
        return await _context.HeroTreeProgress
            .Where(htp => htp.UserId == userId)
            .Select(htp => new HeroTreeNodeDto
            {
                NodeId = htp.NodeId,
                TokensCostCourage = htp.TokensCostCourage,
                TokensCostCuriosity = htp.TokensCostCuriosity,
                TokensCostThinking = htp.TokensCostThinking,
                TokensCostCreativity = htp.TokensCostCreativity,
                UnlockedAt = htp.UnlockedAt
            })
            .ToListAsync();
    }

    public async Task<bool> CompleteStoryAsync(Guid userId, CompleteStoryRequest request)
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
                RewardReceived = request.RewardReceived
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

    public async Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0)
    {
        try
        {
            var userTokens = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.UserId == userId);

            if (userTokens == null)
            {
                userTokens = new UserTokens
                {
                    UserId = userId,
                    Courage = courage,
                    Curiosity = curiosity,
                    Thinking = thinking,
                    Creativity = creativity
                };
                _context.UserTokens.Add(userTokens);
            }
            else
            {
                userTokens.Courage += courage;
                userTokens.Curiosity += curiosity;
                userTokens.Thinking += thinking;
                userTokens.Creativity += creativity;
                userTokens.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UnlockHeroAsync(Guid userId, string heroId, string heroType, string? sourceStoryId = null)
    {
        try
        {
            var existingHero = await _context.HeroProgress
                .FirstOrDefaultAsync(hp => hp.UserId == userId && hp.HeroId == heroId && hp.HeroType == heroType);

            if (existingHero != null)
            {
                return false; // Already unlocked
            }

            var heroProgress = new HeroProgress
            {
                UserId = userId,
                HeroId = heroId,
                HeroType = heroType,
                SourceStoryId = sourceStoryId ?? string.Empty
            };

            _context.HeroProgress.Add(heroProgress);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request)
    {
        try
        {
            var existingNode = await _context.HeroTreeProgress
                .FirstOrDefaultAsync(htp => htp.UserId == userId && htp.NodeId == request.NodeId);

            if (existingNode != null)
            {
                return false; // Already unlocked
            }

            var heroTreeProgress = new HeroTreeProgress
            {
                UserId = userId,
                NodeId = request.NodeId,
                TokensCostCourage = request.TokensCostCourage,
                TokensCostCuriosity = request.TokensCostCuriosity,
                TokensCostThinking = request.TokensCostThinking,
                TokensCostCreativity = request.TokensCostCreativity
            };

            _context.HeroTreeProgress.Add(heroTreeProgress);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0)
    {
        try
        {
            var userTokens = await _context.UserTokens
                .FirstOrDefaultAsync(ut => ut.UserId == userId);

            if (userTokens == null ||
                userTokens.Courage < courage ||
                userTokens.Curiosity < curiosity ||
                userTokens.Thinking < thinking ||
                userTokens.Creativity < creativity)
            {
                return false; // Insufficient tokens
            }

            userTokens.Courage -= courage;
            userTokens.Curiosity -= curiosity;
            userTokens.Thinking -= thinking;
            userTokens.Creativity -= creativity;
            userTokens.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
