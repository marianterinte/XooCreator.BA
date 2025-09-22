using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TreeOfHeroes;

public interface ITreeOfHeroesRepository
{
    Task<UserTokensDto> GetUserTokensAsync(Guid userId);
    Task<List<HeroDto>> GetHeroProgressAsync(Guid userId);
    Task<List<HeroTreeNodeDto>> GetHeroTreeProgressAsync(Guid userId);
    Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request);
    Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0);
    Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0);
}

public class TreeOfHeroesRepository : ITreeOfHeroesRepository
{
    private readonly XooDbContext _context;

    public TreeOfHeroesRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<UserTokensDto> GetUserTokensAsync(Guid userId)
    {
        var userTokens = await _context.UserTokens
            .FirstOrDefaultAsync(ut => ut.UserId == userId);

        if (userTokens == null)
        {
            // Create default tokens if user doesn't have any
            userTokens = new Data.UserTokens
            {
                UserId = userId,
                Courage = 0,
                Curiosity = 0,
                Thinking = 0,
                Creativity = 0,
                Safety = 0
            };
            _context.UserTokens.Add(userTokens);
            await _context.SaveChangesAsync();
        }

        return new UserTokensDto
        {
            Courage = userTokens.Courage,
            Curiosity = userTokens.Curiosity,
            Thinking = userTokens.Thinking,
            Creativity = userTokens.Creativity,
            Safety = userTokens.Safety
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
                TokensCostSafety = htp.TokensCostSafety,
                UnlockedAt = htp.UnlockedAt
            })
            .ToListAsync();
    }

    public async Task<bool> UnlockHeroTreeNodeAsync(Guid userId, UnlockHeroTreeNodeRequest request)
    {
        var existingNode = await _context.HeroTreeProgress
            .FirstOrDefaultAsync(htp => htp.UserId == userId && htp.NodeId == request.NodeId);

        if (existingNode != null)
        {
            return false; // Already unlocked
        }

        var heroTreeNode = new Data.HeroTreeProgress
        {
            UserId = userId,
            NodeId = request.NodeId,
            TokensCostCourage = request.TokensCostCourage,
            TokensCostCuriosity = request.TokensCostCuriosity,
            TokensCostThinking = request.TokensCostThinking,
            TokensCostCreativity = request.TokensCostCreativity,
            TokensCostSafety = request.TokensCostSafety,
            UnlockedAt = DateTime.UtcNow
        };

        _context.HeroTreeProgress.Add(heroTreeNode);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SpendTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0)
    {
        var userTokens = await _context.UserTokens
            .FirstOrDefaultAsync(ut => ut.UserId == userId);

        if (userTokens == null)
        {
            return false;
        }

        // Check if user has enough tokens
        if (userTokens.Courage < courage ||
            userTokens.Curiosity < curiosity ||
            userTokens.Thinking < thinking ||
            userTokens.Creativity < creativity ||
            userTokens.Safety < safety)
        {
            return false;
        }

        // Spend tokens
        userTokens.Courage -= courage;
        userTokens.Curiosity -= curiosity;
        userTokens.Thinking -= thinking;
        userTokens.Creativity -= creativity;
        userTokens.Safety -= safety;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AwardTokensAsync(Guid userId, int courage = 0, int curiosity = 0, int thinking = 0, int creativity = 0, int safety = 0)
    {
        var userTokens = await _context.UserTokens
            .FirstOrDefaultAsync(ut => ut.UserId == userId);

        if (userTokens == null)
        {
            // Create new user tokens if they don't exist
            userTokens = new Data.UserTokens
            {
                UserId = userId,
                Courage = courage,
                Curiosity = curiosity,
                Thinking = thinking,
                Creativity = creativity,
                Safety = safety
            };
            _context.UserTokens.Add(userTokens);
        }
        else
        {
            // Add tokens to existing record
            userTokens.Courage += courage;
            userTokens.Curiosity += curiosity;
            userTokens.Thinking += thinking;
            userTokens.Creativity += creativity;
            userTokens.Safety += safety;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
