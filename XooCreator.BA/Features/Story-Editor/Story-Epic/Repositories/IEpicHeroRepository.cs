using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

public interface IEpicHeroRepository
{
    // Get hero by ID
    Task<EpicHero?> GetAsync(string heroId, CancellationToken ct = default);
    
    // Create new hero
    Task<EpicHero> CreateAsync(Guid ownerUserId, string heroId, string name, CancellationToken ct = default);
    
    // Save hero
    Task SaveAsync(EpicHero hero, CancellationToken ct = default);
    
    // List heroes by owner (with optional status filter)
    Task<List<EpicHero>> ListByOwnerAsync(Guid ownerUserId, string? status = null, CancellationToken ct = default);

    // List published heroes (optionally excluding an owner)
    Task<List<EpicHero>> ListPublishedAsync(Guid? excludeOwnerId = null, CancellationToken ct = default);
    
    // List heroes for review (sent_for_approval or in_review status)
    Task<List<EpicHero>> ListForReviewAsync(CancellationToken ct = default);
    
    // Delete hero
    Task DeleteAsync(string heroId, CancellationToken ct = default);
    
    // Check if hero exists
    Task<bool> ExistsAsync(string heroId, CancellationToken ct = default);
    
    // Check if hero is used in any epic
    Task<bool> IsUsedInEpicsAsync(string heroId, CancellationToken ct = default);
}

