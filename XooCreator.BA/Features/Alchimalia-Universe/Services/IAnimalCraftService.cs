using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public interface IAnimalCraftService
{
    Task<AnimalCraftDto> GetAsync(Guid animalId, string? languageCode = null, CancellationToken ct = default);
    Task<ListAnimalCraftsResponse> ListAsync(Guid currentUserId, string? status = null, Guid? regionId = null, bool? isHybrid = null, string? search = null, string? languageCode = null, CancellationToken ct = default);
    Task<AnimalCraftDto> CreateAsync(Guid userId, CreateAnimalCraftRequest request, CancellationToken ct = default);
    Task<AnimalCraftDto> CreateCraftFromDefinitionAsync(Guid userId, Guid definitionId, bool allowAdminOverride = false, CancellationToken ct = default);
    Task<AnimalCraftDto> UpdateAsync(Guid userId, Guid animalId, UpdateAnimalCraftRequest request, bool allowAdminOverride = false, CancellationToken ct = default);
    Task SubmitForReviewAsync(Guid userId, Guid animalId, CancellationToken ct = default);
    Task ReviewAsync(Guid reviewerId, Guid animalId, ReviewAnimalCraftRequest request, CancellationToken ct = default);
    Task ClaimAsync(Guid reviewerId, Guid animalId, CancellationToken ct = default);
    Task RetractAsync(Guid userId, Guid animalId, CancellationToken ct = default);
    Task PublishAsync(Guid publisherId, Guid animalId, bool allowAdminOverride = false, CancellationToken ct = default);
    Task DeleteAsync(Guid userId, Guid animalId, CancellationToken ct = default);
}
