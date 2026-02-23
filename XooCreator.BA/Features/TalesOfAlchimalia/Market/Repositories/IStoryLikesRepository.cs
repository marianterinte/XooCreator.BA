namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public interface IStoryLikesRepository
{
    Task<bool> ToggleLikeAsync(Guid userId, string storyId, CancellationToken ct = default);
    Task<bool> IsLikedAsync(Guid userId, string storyId);
    Task<int> GetStoryLikesCountAsync(string storyId);
    /// <summary>Returns likes count per story ID for the given list (single query, GROUP BY).</summary>
    Task<Dictionary<string, int>> GetStoryLikesCountsAsync(IReadOnlyList<string> storyIds);
    Task<bool> RemoveLikeAsync(Guid userId, string storyId, CancellationToken ct = default);
}

