using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.Bestiary.Services;

/// <summary>
/// Persists newly unlocked story heroes (from epic story completion) into UserBestiary + BestiaryItems.
/// </summary>
public interface IStoryHeroBestiaryService
{
    /// <summary>
    /// For each hero in the list, ensures a BestiaryItem exists (ArmsKey = HeroId) and adds UserBestiary with BestiaryType = "storyhero" if not already present.
    /// </summary>
    Task AddDiscoveredStoryHeroesAsync(Guid userId, IReadOnlyList<UnlockedHeroDto> heroes, CancellationToken ct = default);
}
