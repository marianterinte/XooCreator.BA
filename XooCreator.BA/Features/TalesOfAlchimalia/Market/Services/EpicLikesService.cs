using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

public interface IEpicLikesService
{
    Task<ToggleEpicLikeResponse> ToggleLikeAsync(Guid userId, string epicId);
    Task<EpicLikeStatusResponse> GetLikeStatusAsync(Guid userId, string epicId);
}

public record ToggleEpicLikeResponse
{
    public bool IsLiked { get; init; }
    public int LikesCount { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record EpicLikeStatusResponse
{
    public bool IsLiked { get; init; }
    public int LikesCount { get; init; }
}

public class EpicLikesService : IEpicLikesService
{
    private readonly IEpicLikesRepository _repository;

    public EpicLikesService(IEpicLikesRepository repository)
    {
        _repository = repository;
    }

    public async Task<ToggleEpicLikeResponse> ToggleLikeAsync(Guid userId, string epicId)
    {
        var isLiked = await _repository.ToggleLikeAsync(userId, epicId);
        var likesCount = await _repository.GetEpicLikesCountAsync(epicId);

        return new ToggleEpicLikeResponse
        {
            Success = true,
            IsLiked = isLiked,
            LikesCount = likesCount
        };
    }

    public async Task<EpicLikeStatusResponse> GetLikeStatusAsync(Guid userId, string epicId)
    {
        var isLiked = await _repository.IsLikedAsync(userId, epicId);
        var likesCount = await _repository.GetEpicLikesCountAsync(epicId);

        return new EpicLikeStatusResponse
        {
            IsLiked = isLiked,
            LikesCount = likesCount
        };
    }
}
