namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public interface IStoryLikesRepository
{
    Task<bool> ToggleLikeAsync(Guid userId, string storyId);
    Task<bool> IsLikedAsync(Guid userId, string storyId);
    Task<int> GetStoryLikesCountAsync(string storyId);
    Task<bool> RemoveLikeAsync(Guid userId, string storyId);
    Task<bool> HasUserReadStoryAsync(Guid userId, string storyId);
}

