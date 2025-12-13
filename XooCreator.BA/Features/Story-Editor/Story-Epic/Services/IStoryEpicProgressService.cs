using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IStoryEpicProgressService
{
    Task<StoryEpicStateWithProgressDto?> GetEpicStateWithProgressAsync(string epicId, Guid userId, CancellationToken ct = default);
    Task<CompleteEpicStoryResult> CompleteStoryAsync(string epicId, Guid userId, string storyId, string? selectedAnswer = null, CancellationToken ct = default);
}

public record CompleteEpicStoryResult
{
    public required bool Success { get; init; }
    public List<string> NewlyUnlockedRegions { get; init; } = new();
}

