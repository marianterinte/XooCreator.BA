using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Features.GuestSync.DTOs;
using XooCreator.BA.Features.HeroStoryRewards.DTOs;
using XooCreator.BA.Features.HeroStoryRewards.Services;
using XooCreator.BA.Features.Stories.Mappers;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;
using TokenReward = XooCreator.BA.Features.TreeOfLight.DTOs.TokenReward;
using TokenFamily = XooCreator.BA.Features.TreeOfLight.DTOs.TokenFamily;

namespace XooCreator.BA.Features.GuestSync.Services;

public interface IGuestSyncService
{
    Task<GuestSyncResponse> SyncAsync(Guid userId, GuestSyncRequest request, CancellationToken ct = default);
}

public class GuestSyncService : IGuestSyncService
{
    private readonly XooDbContext _context;
    private readonly ITreeOfHeroesRepository _treeOfHeroesRepository;
    private readonly IFavoritesRepository _favoritesRepository;
    private readonly IStoryLikesRepository _storyLikesRepository;
    private readonly IEpicFavoritesRepository _epicFavoritesRepository;
    private readonly ILogger<GuestSyncService> _logger;

    public GuestSyncService(
        XooDbContext context,
        ITreeOfHeroesRepository treeOfHeroesRepository,
        IFavoritesRepository favoritesRepository,
        IStoryLikesRepository storyLikesRepository,
        IEpicFavoritesRepository epicFavoritesRepository,
        ILogger<GuestSyncService> logger)
    {
        _context = context;
        _treeOfHeroesRepository = treeOfHeroesRepository;
        _favoritesRepository = favoritesRepository;
        _storyLikesRepository = storyLikesRepository;
        _epicFavoritesRepository = epicFavoritesRepository;
        _logger = logger;
    }

    public async Task<GuestSyncResponse> SyncAsync(Guid userId, GuestSyncRequest request, CancellationToken ct = default)
    {
        var warnings = new List<string>();

        // 1. Award tokens (add, don't overwrite)
        if (request.TokenBalances.Count > 0)
        {
            var rewards = new List<TokenReward>();
            foreach (var t in request.TokenBalances)
            {
                if (t.Quantity <= 0) continue;
                var family = StoryDefinitionMapper.MapFamily(t.Type?.Trim() ?? "");
                var value = (t.Value ?? "").Trim();
                if (string.IsNullOrEmpty(value)) continue;
                rewards.Add(new TokenReward { Type = family, Value = value, Quantity = t.Quantity });
            }
            if (rewards.Count > 0)
            {
                try
                {
                    await _treeOfHeroesRepository.AwardTokensAsync(userId, rewards);
                    _logger.LogInformation("GuestSync: Awarded {Count} token types for userId={UserId}", rewards.Count, userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "GuestSync: Failed to award tokens for userId={UserId}", userId);
                    warnings.Add("Tokens could not be saved.");
                }
            }
        }

        // 2. Completed stories -> UserStoryReadHistory (upsert, CompletedAt set)
        foreach (var storyId in request.CompletedStoryIds.Distinct())
        {
            if (string.IsNullOrWhiteSpace(storyId)) continue;
            try
            {
                var storyDef = await _context.StoryDefinitions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.StoryId == storyId && s.IsActive, ct);
                if (storyDef == null)
                {
                    warnings.Add($"Story {storyId} not found or inactive, skipped.");
                    continue;
                }
                var totalTiles = storyDef.Tiles?.Count ?? 1;
                var existing = await _context.UserStoryReadHistory
                    .FirstOrDefaultAsync(h => h.UserId == userId && EF.Functions.ILike(h.StoryId, storyId), ct);
                if (existing != null)
                {
                    if (!existing.CompletedAt.HasValue)
                    {
                        existing.TotalTilesRead = totalTiles;
                        existing.TotalTiles = totalTiles;
                        existing.LastReadAt = DateTime.UtcNow;
                        existing.CompletedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    _context.UserStoryReadHistory.Add(new UserStoryReadHistory
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        StoryId = storyId,
                        TotalTilesRead = totalTiles,
                        TotalTiles = totalTiles,
                        LastReadAt = DateTime.UtcNow,
                        CompletedAt = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GuestSync: Failed to add completed story {StoryId} for userId={UserId}", storyId, userId);
                warnings.Add($"Completed story {storyId} could not be saved.");
            }
        }

        if (request.CompletedStoryIds.Count > 0)
        {
            await _context.SaveChangesAsync(ct);
        }

        // 3. Likes -> StoryLikes (ignore duplicates)
        foreach (var storyId in request.LikedStoryIds.Distinct())
        {
            if (string.IsNullOrWhiteSpace(storyId)) continue;
            try
            {
                var alreadyLiked = await _storyLikesRepository.IsLikedAsync(userId, storyId);
                if (!alreadyLiked)
                {
                    await _storyLikesRepository.ToggleLikeAsync(userId, storyId, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GuestSync: Failed to add like for story {StoryId} userId={UserId}", storyId, userId);
                warnings.Add($"Like for story {storyId} could not be saved.");
            }
        }

        // 4. Favorite stories -> UserFavoriteStories (ignore duplicates)
        foreach (var storyId in request.FavoriteStoryIds.Distinct())
        {
            if (string.IsNullOrWhiteSpace(storyId)) continue;
            try
            {
                await _favoritesRepository.AddFavoriteAsync(userId, storyId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GuestSync: Failed to add favorite story {StoryId} userId={UserId}", storyId, userId);
                warnings.Add($"Favorite story {storyId} could not be saved.");
            }
        }

        // 5. Favorite epics -> UserFavoriteEpics (ignore duplicates)
        foreach (var epicId in request.FavoriteEpicIds.Distinct())
        {
            if (string.IsNullOrWhiteSpace(epicId)) continue;
            try
            {
                await _epicFavoritesRepository.AddFavoriteAsync(userId, epicId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GuestSync: Failed to add favorite epic {EpicId} userId={UserId}", epicId, userId);
                warnings.Add($"Favorite epic {epicId} could not be saved.");
            }
        }

        _logger.LogInformation("GuestSync: Completed for userId={UserId} stories={Stories} tokens={Tokens} likes={Likes} favStories={FavStories} favEpics={FavEpics}",
            userId, request.CompletedStoryIds.Count, request.TokenBalances.Count, request.LikedStoryIds.Count,
            request.FavoriteStoryIds.Count, request.FavoriteEpicIds.Count);

        return new GuestSyncResponse { Success = true, Warnings = warnings };
    }
}
