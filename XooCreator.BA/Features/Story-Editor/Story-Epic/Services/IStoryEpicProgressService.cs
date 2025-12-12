using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IStoryEpicProgressService
{
    Task<StoryEpicStateWithProgressDto?> GetEpicStateWithProgressAsync(string epicId, Guid userId, CancellationToken ct = default);
    Task<bool> CompleteStoryAsync(string epicId, Guid userId, string storyId, string? selectedAnswer = null, CancellationToken ct = default);
}

