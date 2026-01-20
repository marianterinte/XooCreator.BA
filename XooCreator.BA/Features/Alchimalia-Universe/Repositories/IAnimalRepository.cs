using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public interface IAnimalRepository
{
    Task<Animal?> GetAsync(Guid animalId, CancellationToken ct = default);
    Task<Animal?> GetWithTranslationsAsync(Guid animalId, CancellationToken ct = default);
    Task<Animal> CreateAsync(Animal animal, CancellationToken ct = default);
    Task SaveAsync(Animal animal, CancellationToken ct = default);
    Task<List<Animal>> ListAsync(string? status = null, Guid? regionId = null, bool? isHybrid = null, string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(string? status = null, Guid? regionId = null, bool? isHybrid = null, CancellationToken ct = default);
    Task DeleteAsync(Guid animalId, CancellationToken ct = default);
}
