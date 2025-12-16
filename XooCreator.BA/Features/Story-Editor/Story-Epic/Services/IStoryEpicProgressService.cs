using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IStoryEpicProgressService
{
    Task<StoryEpicStateWithProgressDto?> GetEpicStateWithProgressAsync(string epicId, Guid userId, CancellationToken ct = default);
    Task<CompleteEpicStoryResult> CompleteStoryAsync(string epicId, Guid userId, string storyId, string? selectedAnswer = null, CancellationToken ct = default);
    Task<ResetEpicProgressResult> ResetProgressAsync(string epicId, Guid userId, CancellationToken ct = default);
}

public record ResetEpicProgressResult
{
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record CompleteEpicStoryResult
{
    public required bool Success { get; init; }
    public List<string> NewlyUnlockedRegions { get; init; } = new();
    public List<UnlockedHeroDto> NewlyUnlockedHeroes { get; init; } = new();
    public string? StoryCoverImageUrl { get; init; }
}

