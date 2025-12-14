using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IEpicHeroService
{
    // Get hero
    Task<EpicHeroDto?> GetHeroAsync(string heroId, CancellationToken ct = default);
    
    // Create hero
    Task<EpicHeroDto> CreateHeroAsync(Guid ownerUserId, string heroId, string name, CancellationToken ct = default);
    
    // Save hero
    Task SaveHeroAsync(Guid ownerUserId, string heroId, EpicHeroDto dto, CancellationToken ct = default);
    
    // List heroes by owner
    Task<List<EpicHeroListItemDto>> ListHeroesByOwnerAsync(Guid ownerUserId, string? status = null, Guid? currentUserId = null, CancellationToken ct = default);
    
    // List heroes available to the current editor (own + published)
    Task<List<EpicHeroListItemDto>> ListHeroesForEditorAsync(Guid currentUserId, string? status = null, CancellationToken ct = default);
    
    // Delete hero
    Task DeleteHeroAsync(Guid ownerUserId, string heroId, CancellationToken ct = default);
    
    // Submit for review
    Task SubmitForReviewAsync(Guid ownerUserId, string heroId, CancellationToken ct = default);
    
    // Review (approve or request changes)
    Task ReviewAsync(Guid reviewerUserId, string heroId, bool approve, string? notes, CancellationToken ct = default);
    
    // Publish (synchronous - copies assets)
    Task PublishAsync(Guid ownerUserId, string heroId, string ownerEmail, CancellationToken ct = default);
    
    // Retract from review
    Task RetractAsync(Guid ownerUserId, string heroId, CancellationToken ct = default);
}

