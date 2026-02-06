using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

public interface IStoryLikesService
{
    Task<ToggleStoryLikeResponse> ToggleLikeAsync(Guid userId, string storyId);
    Task<StoryLikeStatusResponse> GetLikeStatusAsync(Guid userId, string storyId);
}

public record ToggleStoryLikeResponse
{
    public bool IsLiked { get; init; }
    public int LikesCount { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record StoryLikeStatusResponse
{
    public bool IsLiked { get; init; }
    public int LikesCount { get; init; }
}

public class StoryLikesService : IStoryLikesService
{
    private readonly IStoryLikesRepository _repository;

    public StoryLikesService(IStoryLikesRepository repository)
    {
        _repository = repository;
    }

    public async Task<ToggleStoryLikeResponse> ToggleLikeAsync(Guid userId, string storyId)
    {
        // Check removed: Users can now like stories without reading them first


        var isLiked = await _repository.ToggleLikeAsync(userId, storyId);
        var likesCount = await _repository.GetStoryLikesCountAsync(storyId);

        return new ToggleStoryLikeResponse
        {
            Success = true,
            IsLiked = isLiked,
            LikesCount = likesCount
        };
    }

    public async Task<StoryLikeStatusResponse> GetLikeStatusAsync(Guid userId, string storyId)
    {
        var isLiked = await _repository.IsLikedAsync(userId, storyId);
        var likesCount = await _repository.GetStoryLikesCountAsync(storyId);

        return new StoryLikeStatusResponse
        {
            IsLiked = isLiked,
            LikesCount = likesCount
        };
    }
}

