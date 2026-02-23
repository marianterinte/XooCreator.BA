using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public interface IStoryHeroRepository
{
    Task<StoryHero?> GetAsync(Guid storyHeroId, CancellationToken ct = default);
    Task<StoryHero?> GetByHeroIdAsync(string heroId, CancellationToken ct = default);
    /// <summary>Get by HeroId with Translations in a single query (avoids double-fetch).</summary>
    Task<StoryHero?> GetByHeroIdWithTranslationsAsync(string heroId, CancellationToken ct = default);
    Task<StoryHero?> GetWithTranslationsAsync(Guid storyHeroId, CancellationToken ct = default);
    Task<StoryHero> CreateAsync(StoryHero storyHero, CancellationToken ct = default);
    Task SaveAsync(StoryHero storyHero, CancellationToken ct = default);
    Task<List<StoryHero>> ListAsync(string? status = null, string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(string? status = null, CancellationToken ct = default);
    Task DeleteAsync(Guid storyHeroId, CancellationToken ct = default);
}
