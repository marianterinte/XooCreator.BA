using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public interface IStoryRegionRepository
{
    // Get region by ID
    Task<StoryRegion?> GetAsync(string regionId, CancellationToken ct = default);
    
    // Create new region
    Task<StoryRegion> CreateAsync(Guid ownerUserId, string regionId, string name, CancellationToken ct = default);
    
    // Save region
    Task SaveAsync(StoryRegion region, CancellationToken ct = default);
    
    // List regions by owner (with optional status filter)
    Task<List<StoryRegion>> ListByOwnerAsync(Guid ownerUserId, string? status = null, CancellationToken ct = default);

    // List published regions (optionally excluding an owner)
    Task<List<StoryRegion>> ListPublishedAsync(Guid? excludeOwnerId = null, CancellationToken ct = default);
    
    // List regions for review (sent_for_approval or in_review status)
    Task<List<StoryRegion>> ListForReviewAsync(CancellationToken ct = default);
    
    // Delete region
    Task DeleteAsync(string regionId, CancellationToken ct = default);
    
    // Check if region exists
    Task<bool> ExistsAsync(string regionId, CancellationToken ct = default);
    
    // Check if region is used in any epic
    Task<bool> IsUsedInEpicsAsync(string regionId, CancellationToken ct = default);
}

