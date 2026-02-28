using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Stories.DTOs;
using XooCreator.BA.Features.TreeOfLight.DTOs;

namespace XooCreator.BA.Features.TreeOfLight.Repositories;

public interface ITreeOfLightRepository
{
    Task<List<TreeConfiguration>> GetAllConfigurationsAsync();
    Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId, string configId);
    Task<List<StoryProgressDto>> GetStoryProgressAsync(Guid userId, string configId);

    Task<bool> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, StoryContentDto? story, string configId, CancellationToken ct = default);
    Task<bool> UnlockRegionAsync(Guid userId, string regionId, string configId, CancellationToken ct = default);
    Task ResetUserProgressAsync(Guid userId, CancellationToken ct = default);
    
    Task<List<StoryHero>> GetStoryHeroesAsync();
    Task<bool> IsHeroUnlockedAsync(Guid userId, string heroId);
    Task<bool> UnlockHeroAsync(Guid userId, string heroId, string heroType, CancellationToken ct = default);
    
    Task<List<HeroMessage>> GetHeroMessagesAsync();
    Task<List<HeroClickMessage>> GetHeroClickMessagesAsync();
    Task<HeroMessage?> GetHeroMessageAsync(string heroId, string regionId);
    Task<HeroClickMessage?> GetHeroClickMessageAsync(string heroId);
}

public class TreeOfLightRepository : ITreeOfLightRepository
{
    private const string StoryHeroesCacheKey = "story_heroes_published";
    private readonly XooDbContext _context;
    private readonly ILogger<TreeOfLightRepository> _logger;
    private readonly IMemoryCache _cache;

    public TreeOfLightRepository(XooDbContext context, ILogger<TreeOfLightRepository> logger, IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<TreeConfiguration>> GetAllConfigurationsAsync()
    {
        return await _context.TreeConfigurations.AsNoTracking().ToListAsync();
    }

    public async Task<List<TreeProgressDto>> GetTreeProgressAsync(Guid userId, string configId)
    {
        return await _context.TreeProgress
            .AsNoTracking()
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
            .AsNoTracking()
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




    public async Task<bool> CompleteStoryAsync(Guid userId, CompleteStoryRequest request, StoryContentDto? story, string configId, CancellationToken ct = default)
    {
        try
        {
            var existingStory = await _context.StoryProgress
                .AsNoTracking()
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
            await _context.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error completing story for user {UserId}, story {StoryId}, config {ConfigId}", userId, request.StoryId, configId);
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

    public async Task<bool> UnlockRegionAsync(Guid userId, string regionId, string configId, CancellationToken ct = default)
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

            await _context.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error unlocking region for user {UserId}, region {RegionId}, config {ConfigId}", userId, regionId, configId);
            return false;
        }
    }





    public async Task ResetUserProgressAsync(Guid userId, CancellationToken ct = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            await _context.TreeProgress.Where(tp => tp.UserId == userId).ExecuteDeleteAsync(ct);
            await _context.StoryProgress.Where(sp => sp.UserId == userId).ExecuteDeleteAsync(ct);
            await _context.HeroProgress.Where(hp => hp.UserId == userId).ExecuteDeleteAsync(ct);
            await _context.HeroTreeProgress.Where(htp => htp.UserId == userId).ExecuteDeleteAsync(ct);

            await _context.UserTokenBalances
                .Where(b => b.UserId == userId)
                .ExecuteDeleteAsync(ct);

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<List<StoryHero>> GetStoryHeroesAsync()
    {
        if (_cache.TryGetValue(StoryHeroesCacheKey, out List<StoryHero>? cached) && cached != null)
            return cached;

        var heroes = await _context.StoryHeroes
            .AsNoTracking()
            .Where(sh => sh.Status == AlchimaliaUniverseStatus.Published.ToDb())
            .ToListAsync();

        _cache.Set(StoryHeroesCacheKey, heroes, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
            Size = 5
        });

        return heroes;
    }

    public async Task<bool> IsHeroUnlockedAsync(Guid userId, string heroId)
    {
        return await _context.HeroProgress
            .AsNoTracking()
            .AnyAsync(hp => hp.UserId == userId && hp.HeroId == heroId);
    }

    public async Task<bool> UnlockHeroAsync(Guid userId, string heroId, string heroType, CancellationToken ct = default)
    {
        var existingProgress = await _context.HeroProgress
            .AsNoTracking()
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
        await _context.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<List<HeroMessage>> GetHeroMessagesAsync()
    {
        return await _context.HeroMessages
            .AsNoTracking()
            .Where(hm => hm.IsActive)
            .ToListAsync();
    }

    public async Task<List<HeroClickMessage>> GetHeroClickMessagesAsync()
    {
        return await _context.HeroClickMessages
            .AsNoTracking()
            .Where(hcm => hcm.IsActive)
            .ToListAsync();
    }

    public async Task<HeroMessage?> GetHeroMessageAsync(string heroId, string regionId)
    {
        return await _context.HeroMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(hm => hm.HeroId == heroId && hm.RegionId == regionId && hm.IsActive);
    }

    public async Task<HeroClickMessage?> GetHeroClickMessageAsync(string heroId)
    {
        return await _context.HeroClickMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(hcm => hcm.HeroId == heroId && hcm.IsActive);
    }
}
