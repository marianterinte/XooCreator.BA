using XooCreator.BA.Data;
using XooCreator.BA.Features.Stories.DTOs;

namespace XooCreator.BA.Features.Stories.Repositories;

public interface IStoriesRepository
{
    Task<List<StoryContentDto>> GetAllStoriesAsync(string locale);
    Task<StoryContentDto?> GetStoryByIdAsync(string storyId, string locale);
    /// <summary>Removes cached story content for the given story so the next read gets fresh data (e.g. after publish).</summary>
    void InvalidateStoryContentCache(string storyId);
    Task<StoryDefinition?> GetStoryDefinitionByIdAsync(string storyId);
    Task<List<UserStoryProgressDto>> GetUserStoryProgressAsync(Guid userId, string storyId);
    Task<StoryCompletionInfo> GetStoryCompletionStatusAsync(Guid userId, string storyId);
    Task<bool> MarkTileAsReadAsync(Guid userId, string storyId, string tileId, CancellationToken ct = default);
    Task ResetStoryProgressAsync(Guid userId, string storyId, CancellationToken ct = default);
    Task<bool> StoryIdExistsAsync(string storyId);
    Task SeedStoriesAsync(CancellationToken ct = default);
    Task SeedIndependentStoriesAsync(CancellationToken ct = default);
}


