using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IStoryRegionService
{
    // Get region
    Task<StoryRegionDto?> GetRegionAsync(string regionId, CancellationToken ct = default);
    
    // Create region
    Task<StoryRegionDto> CreateRegionAsync(Guid ownerUserId, string regionId, string name, string? description, string languageCode, CancellationToken ct = default);
    
    // Save region
    Task SaveRegionAsync(Guid ownerUserId, string regionId, StoryRegionDto dto, CancellationToken ct = default);
    
    // List regions by owner
    Task<List<StoryRegionListItemDto>> ListRegionsByOwnerAsync(Guid ownerUserId, string? status = null, Guid? currentUserId = null, CancellationToken ct = default);
    
    // List regions available to the current editor (own + published)
    Task<List<StoryRegionListItemDto>> ListRegionsForEditorAsync(Guid currentUserId, string? status = null, CancellationToken ct = default);
    
    // List all regions (for admin)
    Task<List<StoryRegionListItemDto>> ListAllRegionsAsync(Guid currentUserId, string? status = null, CancellationToken ct = default);
    
    // Delete region
    Task DeleteRegionAsync(Guid ownerUserId, string regionId, CancellationToken ct = default);
    
    // Submit for review
    Task SubmitForReviewAsync(Guid ownerUserId, string regionId, CancellationToken ct = default);
    
    // Review (approve or request changes)
    Task ReviewAsync(Guid reviewerUserId, string regionId, bool approve, string? notes, CancellationToken ct = default);
    
    // Publish (synchronous - copies assets)
    Task PublishAsync(Guid ownerUserId, string regionId, string ownerEmail, bool isAdmin = false, CancellationToken ct = default);
    
    // Retract from review
    Task RetractAsync(Guid ownerUserId, string regionId, CancellationToken ct = default);
    
    // Create new version from published region
    Task CreateVersionFromPublishedAsync(Guid ownerUserId, string regionId, CancellationToken ct = default);
    
    // Unpublish a published region
    Task UnpublishAsync(Guid ownerUserId, string regionId, string reason, CancellationToken ct = default);
}

