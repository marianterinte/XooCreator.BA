using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public interface IHeroDefinitionService
{
    Task<HeroDefinitionDto> GetAsync(string heroId, string? languageCode = null, CancellationToken ct = default);
    Task<ListHeroDefinitionsResponse> ListAsync(string? status = null, string? type = null, string? search = null, string? languageCode = null, CancellationToken ct = default);
    Task<HeroDefinitionDto> CreateAsync(Guid userId, CreateHeroDefinitionRequest request, CancellationToken ct = default);
    Task<HeroDefinitionDto> UpdateAsync(Guid userId, string heroId, UpdateHeroDefinitionRequest request, CancellationToken ct = default);
    Task SubmitForReviewAsync(Guid userId, string heroId, CancellationToken ct = default);
    Task ReviewAsync(Guid reviewerId, string heroId, ReviewHeroDefinitionRequest request, CancellationToken ct = default);
    Task PublishAsync(Guid userId, string heroId, CancellationToken ct = default);
    Task DeleteAsync(Guid userId, string heroId, CancellationToken ct = default);
}
