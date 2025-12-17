using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public interface IStoryRegionRepository
{
    // Get region craft (draft) by ID
    Task<StoryRegionCraft?> GetCraftAsync(string regionId, CancellationToken ct = default);
    
    // Get region definition (published) by ID
    Task<StoryRegionDefinition?> GetDefinitionAsync(string regionId, CancellationToken ct = default);
    
    // Create new region craft
    Task<StoryRegionCraft> CreateCraftAsync(Guid ownerUserId, string regionId, string name, CancellationToken ct = default);
    
    // Save region craft
    Task SaveCraftAsync(StoryRegionCraft regionCraft, CancellationToken ct = default);
    
    // List region crafts by owner (with optional status filter)
    Task<List<StoryRegionCraft>> ListCraftsByOwnerAsync(Guid ownerUserId, string? status = null, CancellationToken ct = default);

    // List published region definitions (optionally excluding an owner)
    Task<List<StoryRegionDefinition>> ListPublishedDefinitionsAsync(Guid? excludeOwnerId = null, CancellationToken ct = default);
    
    // List region crafts for review (sent_for_approval or in_review status)
    Task<List<StoryRegionCraft>> ListCraftsForReviewAsync(CancellationToken ct = default);
    
    // Delete region craft
    Task DeleteCraftAsync(string regionId, CancellationToken ct = default);
    
    // Check if region craft exists
    Task<bool> CraftExistsAsync(string regionId, CancellationToken ct = default);
    
    // Check if region definition exists
    Task<bool> DefinitionExistsAsync(string regionId, CancellationToken ct = default);
    
    // Check if region is used in any epics
    Task<bool> IsUsedInEpicsAsync(string regionId, CancellationToken ct = default);
}

