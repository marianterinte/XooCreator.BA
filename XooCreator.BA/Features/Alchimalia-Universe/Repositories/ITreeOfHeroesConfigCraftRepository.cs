using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public interface ITreeOfHeroesConfigCraftRepository
{
    Task<TreeOfHeroesConfigCraft?> GetAsync(Guid configId, CancellationToken ct = default);
    Task<TreeOfHeroesConfigCraft?> GetWithDetailsAsync(Guid configId, CancellationToken ct = default);
    Task<List<TreeOfHeroesConfigCraft>> ListAsync(string? status = null, CancellationToken ct = default);
    Task<int> CountAsync(string? status = null, CancellationToken ct = default);
    Task<TreeOfHeroesConfigCraft> CreateAsync(TreeOfHeroesConfigCraft config, CancellationToken ct = default);
    Task SaveAsync(TreeOfHeroesConfigCraft config, CancellationToken ct = default);
}
