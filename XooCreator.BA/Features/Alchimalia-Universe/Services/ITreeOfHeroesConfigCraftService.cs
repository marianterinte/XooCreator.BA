using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public interface ITreeOfHeroesConfigCraftService
{
    Task<TreeOfHeroesConfigCraftDto> GetCraftAsync(Guid configId, CancellationToken ct = default);
    Task<ListTreeOfHeroesConfigCraftsResponse> ListCraftsAsync(string? status = null, CancellationToken ct = default);
    Task<ListTreeOfHeroesConfigDefinitionsResponse> ListDefinitionsAsync(string? status = null, CancellationToken ct = default);
    Task<TreeOfHeroesConfigCraftDto> CreateCraftAsync(Guid userId, CreateTreeOfHeroesConfigCraftRequest request, CancellationToken ct = default);
    Task<TreeOfHeroesConfigCraftDto> UpdateCraftAsync(Guid userId, Guid configId, UpdateTreeOfHeroesConfigCraftRequest request, CancellationToken ct = default);
    Task SubmitForReviewAsync(Guid userId, Guid configId, CancellationToken ct = default);
    Task ReviewAsync(Guid reviewerId, Guid configId, ReviewTreeOfHeroesConfigCraftRequest request, CancellationToken ct = default);
    Task PublishAsync(Guid publisherId, Guid configId, CancellationToken ct = default);
}
