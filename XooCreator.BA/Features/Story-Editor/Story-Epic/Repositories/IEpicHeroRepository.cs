using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public interface IEpicHeroRepository
{
    // Get hero craft (draft) by ID
    Task<EpicHeroCraft?> GetCraftAsync(string heroId, CancellationToken ct = default);
    
    // Get hero definition (published) by ID
    Task<EpicHeroDefinition?> GetDefinitionAsync(string heroId, CancellationToken ct = default);
    
    // Create new hero craft
    Task<EpicHeroCraft> CreateCraftAsync(Guid ownerUserId, string heroId, string name, CancellationToken ct = default);
    
    // Save hero craft
    Task SaveCraftAsync(EpicHeroCraft heroCraft, CancellationToken ct = default);
    
    // List hero crafts by owner (with optional status filter)
    Task<List<EpicHeroCraft>> ListCraftsByOwnerAsync(Guid ownerUserId, string? status = null, CancellationToken ct = default);

    // List published hero definitions (optionally excluding an owner)
    Task<List<EpicHeroDefinition>> ListPublishedDefinitionsAsync(Guid? excludeOwnerId = null, CancellationToken ct = default);
    
    // List hero crafts for review (sent_for_approval or in_review status)
    Task<List<EpicHeroCraft>> ListCraftsForReviewAsync(CancellationToken ct = default);
    
    // Delete hero craft
    Task DeleteCraftAsync(string heroId, CancellationToken ct = default);
    
    // Check if hero craft exists
    Task<bool> CraftExistsAsync(string heroId, CancellationToken ct = default);
    
    // Check if hero definition exists
    Task<bool> DefinitionExistsAsync(string heroId, CancellationToken ct = default);
    
    // Check if hero is used in any epics
    Task<bool> IsUsedInEpicsAsync(string heroId, CancellationToken ct = default);
}

