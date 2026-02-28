using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.Bestiary.Services;

public class StoryHeroBestiaryService : IStoryHeroBestiaryService
{
    private const string StoryHeroType = "storyhero";
    private const string PlaceholderKey = "—";

    private readonly XooDbContext _db;

    public StoryHeroBestiaryService(XooDbContext db)
    {
        _db = db;
    }

    public async Task AddDiscoveredStoryHeroesAsync(Guid userId, IReadOnlyList<UnlockedHeroDto> heroes, CancellationToken ct = default)
    {
        if (heroes == null || heroes.Count == 0) return;

        foreach (var hero in heroes)
        {
            if (string.IsNullOrWhiteSpace(hero.HeroId)) continue;

            var heroId = hero.HeroId.Trim();

            var bestiaryItem = await _db.BestiaryItems
                .FirstOrDefaultAsync(bi => bi.ArmsKey == heroId && bi.BodyKey == PlaceholderKey && bi.HeadKey == PlaceholderKey, ct);

            // If not found by exact ArmsKey, check for a legacy BestiaryItem
            // whose ArmsKey matches a translation Name for this EpicHeroDefinition.
            // This prevents duplicates when the same hero was previously discovered
            // with a legacy heroId (e.g. "linkaro") and is now rediscovered with
            // the EpicHeroDefinition.Id (e.g. "hero-linkaro-20250115").
            if (bestiaryItem == null)
            {
                var heroTranslationNames = await _db.EpicHeroDefinitionTranslations
                    .Where(t => t.EpicHeroDefinitionId == heroId)
                    .Select(t => t.Name.ToLower())
                    .ToListAsync(ct);

                if (heroTranslationNames.Count > 0)
                {
                    bestiaryItem = await _db.BestiaryItems
                        .FirstOrDefaultAsync(bi =>
                            bi.BodyKey == PlaceholderKey &&
                            bi.HeadKey == PlaceholderKey &&
                            heroTranslationNames.Contains(bi.ArmsKey.ToLower()), ct);

                    if (bestiaryItem != null)
                    {
                        bestiaryItem.ArmsKey = heroId;
                    }
                }
            }

            if (bestiaryItem == null)
            {
                bestiaryItem = new BestiaryItem
                {
                    Id = Guid.NewGuid(),
                    ArmsKey = heroId,
                    BodyKey = PlaceholderKey,
                    HeadKey = PlaceholderKey,
                    Name = string.Empty,
                    Story = string.Empty,
                    CreatedAt = DateTime.UtcNow
                };
                _db.BestiaryItems.Add(bestiaryItem);
                await _db.SaveChangesAsync(ct);
            }

            var alreadyHas = await _db.UserBestiary
                .AnyAsync(ub => ub.UserId == userId && ub.BestiaryItemId == bestiaryItem.Id && ub.BestiaryType == StoryHeroType, ct);

            if (!alreadyHas)
            {
                _db.UserBestiary.Add(new UserBestiary
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BestiaryItemId = bestiaryItem.Id,
                    BestiaryType = StoryHeroType,
                    DiscoveredAt = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync(ct);
    }
}
