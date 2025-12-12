using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public interface IStoryEpicRepository
{
    // Get epic by ID (basic)
    Task<Data.DbStoryEpic?> GetAsync(string epicId, CancellationToken ct = default);
    
    // Get epic with all related data (regions, stories, rules)
    Task<Data.DbStoryEpic?> GetFullAsync(string epicId, CancellationToken ct = default);
    
    // Create new epic
    Task<Data.DbStoryEpic> CreateAsync(Guid ownerUserId, string epicId, string name, CancellationToken ct = default);
    
    // Save epic (with all related entities)
    Task SaveAsync(Data.DbStoryEpic epic, CancellationToken ct = default);
    
    // List epics by owner
    Task<List<Data.DbStoryEpic>> ListByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
    
    // Delete epic
    Task DeleteAsync(string epicId, CancellationToken ct = default);
    
    // Check if epic exists
    Task<bool> ExistsAsync(string epicId, CancellationToken ct = default);
}

