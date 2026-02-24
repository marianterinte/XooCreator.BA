namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public interface IEpicLikesRepository
{
    Task<bool> ToggleLikeAsync(Guid userId, string epicId, CancellationToken ct = default);
    Task<bool> IsLikedAsync(Guid userId, string epicId);
    Task<int> GetEpicLikesCountAsync(string epicId);
    Task<Dictionary<string, int>> GetEpicLikesCountsAsync(IReadOnlyList<string> epicIds);
}
