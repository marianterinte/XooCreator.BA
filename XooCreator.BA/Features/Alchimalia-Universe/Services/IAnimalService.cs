using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public interface IAnimalService
{
    Task<AnimalDto> GetAsync(Guid animalId, string? languageCode = null, CancellationToken ct = default);
    Task<ListAnimalsResponse> ListAsync(string? status = null, Guid? regionId = null, bool? isHybrid = null, string? search = null, string? languageCode = null, CancellationToken ct = default);
    Task<AnimalDto> CreateAsync(Guid userId, CreateAnimalRequest request, CancellationToken ct = default);
    Task<AnimalDto> UpdateAsync(Guid userId, Guid animalId, UpdateAnimalRequest request, CancellationToken ct = default);
    Task SubmitForReviewAsync(Guid userId, Guid animalId, CancellationToken ct = default);
    Task ReviewAsync(Guid reviewerId, Guid animalId, ReviewAnimalRequest request, CancellationToken ct = default);
    Task PublishAsync(Guid userId, Guid animalId, CancellationToken ct = default);
    Task DeleteAsync(Guid userId, Guid animalId, CancellationToken ct = default);
}
