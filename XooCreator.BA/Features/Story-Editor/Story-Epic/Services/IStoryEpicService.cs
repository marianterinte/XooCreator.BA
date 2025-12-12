using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IStoryEpicService
{
    // Ensure epic exists (create if not)
    Task EnsureEpicAsync(Guid ownerUserId, string epicId, string name, CancellationToken ct = default);
    
    // Save epic from DTO
    Task SaveEpicAsync(Guid ownerUserId, string epicId, StoryEpicDto dto, CancellationToken ct = default);
    
    // Get epic as DTO
    Task<StoryEpicDto?> GetEpicAsync(string epicId, CancellationToken ct = default);
    
    // Get epic state for preview
    Task<StoryEpicStateDto?> GetEpicStateAsync(string epicId, CancellationToken ct = default);
    
    // List epics by owner
    Task<List<StoryEpicListItemDto>> ListEpicsByOwnerAsync(Guid ownerUserId, Guid? currentUserId = null, CancellationToken ct = default);
    
    // Delete epic
    Task DeleteEpicAsync(Guid ownerUserId, string epicId, CancellationToken ct = default);
}

