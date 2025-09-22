using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Features.Stories;

namespace XooCreator.BA.Features.TreeOfLight;

public interface ITreeOfLightRepository
{
    Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId);
    Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId);
    Task<UserTokensDto> GetUserTokensAsync(Guid userId);
    Task<List<HeroDto>> GetHeroProgressAsync(Guid userId);
    Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId);

    Task<bool> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, Stories.StoryContentDto? story);
    Task<bool> UnlockRegionAsync(Guid userId, string regionId);
    Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0);
    Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request);
    Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0);
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

    public async Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0)
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

            // Reset user tokens to default values
            var userTokens = await _context.UserTokens.FirstOrDefaultAsync(ut => ut.UserId == userId);
            if (userTokens != null)
            {
                userTokens.Courage = 0;
                userTokens.Curiosity = 0;
                userTokens.Thinking = 0;
                userTokens.Creativity = 0;
                userTokens.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create default tokens if they don't exist
                _context.UserTokens.Add(new UserTokens
                {
                    UserId = userId,
                    Courage = 0,
                    Curiosity = 0,
                    Thinking = 0,
                    Creativity = 0,
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
