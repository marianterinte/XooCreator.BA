using XooCreator.BA.Data;
using XooCreator.BA.Features.Stories.DTOs;

namespace XooCreator.BA.Features.Stories.Repositories;

public interface IStoriesRepository
{
    Task<List<StoryContentDto>> GetAllStoriesAsync(string locale);
    Task<StoryContentDto?> GetStoryByIdAsync(string storyId, string locale);
    Task<StoryDefinition?> GetStoryDefinitionByIdAsync(string storyId);
    Task<List<UserStoryProgressDto>> GetUserStoryProgressAsync(Guid userId, string storyId);
    Task<StoryCompletionInfo> GetStoryCompletionStatusAsync(Guid userId, string storyId);
    Task<bool> MarkTileAsReadAsync(Guid userId, string storyId, string tileId);
    Task ResetStoryProgressAsync(Guid userId, string storyId);
    Task<bool> StoryIdExistsAsync(string storyId);
    Task SeedStoriesAsync();
    Task SeedIndependentStoriesAsync();
}


