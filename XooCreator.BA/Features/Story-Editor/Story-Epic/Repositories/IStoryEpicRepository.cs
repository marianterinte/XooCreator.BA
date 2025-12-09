using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public interface IStoryEpicRepository
{
    // Get epic by ID (basic)
    Task<Data.StoryEpic?> GetAsync(string epicId, CancellationToken ct = default);
    
    // Get epic with all related data (regions, stories, rules)
    Task<Data.StoryEpic?> GetFullAsync(string epicId, CancellationToken ct = default);
    
    // Create new epic
    Task<Data.StoryEpic> CreateAsync(Guid ownerUserId, string epicId, string name, CancellationToken ct = default);
    
    // Save epic (with all related entities)
    Task SaveAsync(Data.StoryEpic epic, CancellationToken ct = default);
    
    // List epics by owner
    Task<List<Data.StoryEpic>> ListByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
    
    // Delete epic
    Task DeleteAsync(string epicId, CancellationToken ct = default);
    
    // Check if epic exists
    Task<bool> ExistsAsync(string epicId, CancellationToken ct = default);
}

