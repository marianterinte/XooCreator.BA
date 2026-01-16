using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public interface IHeroDefinitionCraftService
{
    Task<HeroDefinitionCraftDto> GetAsync(string heroId, string? languageCode = null, CancellationToken ct = default);
    Task<ListHeroDefinitionCraftsResponse> ListAsync(string? status = null, string? type = null, string? search = null, string? languageCode = null, CancellationToken ct = default);
    Task<HeroDefinitionCraftDto> CreateAsync(Guid userId, CreateHeroDefinitionCraftRequest request, CancellationToken ct = default);
    Task<HeroDefinitionCraftDto> CreateCraftFromDefinitionAsync(Guid userId, string definitionId, CancellationToken ct = default);
    Task<HeroDefinitionCraftDto> UpdateAsync(Guid userId, string heroId, UpdateHeroDefinitionCraftRequest request, CancellationToken ct = default);
    Task SubmitForReviewAsync(Guid userId, string heroId, CancellationToken ct = default);
    Task ReviewAsync(Guid reviewerId, string heroId, ReviewHeroDefinitionCraftRequest request, CancellationToken ct = default);
    Task PublishAsync(Guid publisherId, string heroId, CancellationToken ct = default);
}
