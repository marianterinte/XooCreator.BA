using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public interface IEpicProgressRepository
{
    Task<List<EpicProgressDto>> GetEpicProgressAsync(Guid userId, string epicId);
    Task<List<EpicStoryProgressDto>> GetEpicStoryProgressAsync(Guid userId, string epicId);
    Task<bool> CompleteStoryAsync(Guid userId, string epicId, string storyId, string? selectedAnswer = null);
    Task<bool> UnlockRegionAsync(Guid userId, string epicId, string regionId);
    Task<bool> ResetProgressAsync(Guid userId, string epicId);
}

