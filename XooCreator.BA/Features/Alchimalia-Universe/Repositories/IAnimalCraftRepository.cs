using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public interface IAnimalCraftRepository
{
    Task<AnimalCraft?> GetAsync(Guid animalId, CancellationToken ct = default);
    Task<AnimalCraft?> GetWithTranslationsAsync(Guid animalId, CancellationToken ct = default);
    Task<AnimalCraft> CreateAsync(AnimalCraft animal, CancellationToken ct = default);
    Task SaveAsync(AnimalCraft animal, CancellationToken ct = default);
    Task<List<AnimalCraft>> ListAsync(string? status = null, Guid? regionId = null, bool? isHybrid = null, string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(string? status = null, Guid? regionId = null, CancellationToken ct = default);
}
