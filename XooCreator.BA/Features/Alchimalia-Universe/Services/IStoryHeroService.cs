using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public interface IStoryHeroService
{
    Task<StoryHeroDto> GetAsync(Guid storyHeroId, string? languageCode = null, CancellationToken ct = default);
    Task<StoryHeroDto> GetByHeroIdAsync(string heroId, string? languageCode = null, CancellationToken ct = default);
    Task<ListStoryHeroesResponse> ListAsync(string? status = null, string? search = null, string? languageCode = null, CancellationToken ct = default);
    Task<StoryHeroDto> CreateAsync(Guid userId, CreateStoryHeroRequest request, CancellationToken ct = default);
    Task<StoryHeroDto> UpdateAsync(Guid userId, Guid storyHeroId, UpdateStoryHeroRequest request, CancellationToken ct = default);
    Task SubmitForReviewAsync(Guid userId, Guid storyHeroId, CancellationToken ct = default);
    Task ReviewAsync(Guid reviewerId, Guid storyHeroId, ReviewStoryHeroRequest request, CancellationToken ct = default);
    Task PublishAsync(Guid userId, Guid storyHeroId, CancellationToken ct = default);
    Task DeleteAsync(Guid userId, Guid storyHeroId, CancellationToken ct = default);
}
