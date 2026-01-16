using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public interface IHeroDefinitionCraftRepository
{
    Task<HeroDefinitionCraft?> GetAsync(string heroId, CancellationToken ct = default);
    Task<HeroDefinitionCraft?> GetWithTranslationsAsync(string heroId, CancellationToken ct = default);
    Task<HeroDefinitionCraft> CreateAsync(HeroDefinitionCraft hero, CancellationToken ct = default);
    Task SaveAsync(HeroDefinitionCraft hero, CancellationToken ct = default);
    Task<List<HeroDefinitionCraft>> ListAsync(string? status = null, string? type = null, string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(string? status = null, string? type = null, CancellationToken ct = default);
}
