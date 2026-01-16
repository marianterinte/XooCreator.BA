using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public interface IHeroDefinitionRepository
{
    Task<HeroDefinition?> GetAsync(string heroId, CancellationToken ct = default);
    Task<HeroDefinition?> GetWithTranslationsAsync(string heroId, CancellationToken ct = default);
    Task<HeroDefinition> CreateAsync(HeroDefinition hero, CancellationToken ct = default);
    Task SaveAsync(HeroDefinition hero, CancellationToken ct = default);
    Task<List<HeroDefinition>> ListAsync(string? status = null, string? type = null, string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(string? status = null, string? type = null, CancellationToken ct = default);
    Task DeleteAsync(string heroId, CancellationToken ct = default);
}
