using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public interface IEpicProgressRepository
{
    Task<List<EpicProgressDto>> GetEpicProgressAsync(Guid userId, string epicId);
    Task<List<EpicStoryProgressDto>> GetEpicStoryProgressAsync(Guid userId, string epicId);
    Task<bool> CompleteStoryAsync(Guid userId, string epicId, string storyId, string? selectedAnswer = null, CancellationToken ct = default);
    Task<bool> UnlockRegionAsync(Guid userId, string epicId, string regionId, CancellationToken ct = default);
    Task<bool> ResetProgressAsync(Guid userId, string epicId, CancellationToken ct = default);
}

