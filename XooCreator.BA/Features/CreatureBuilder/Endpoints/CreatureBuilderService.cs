using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.CreatureBuilder;

namespace XooCreator.BA.Services;

public sealed class CreatureBuilderService : ICreatureBuilderService
{
    private readonly XooDbContext _db;

    public CreatureBuilderService(XooDbContext db)
    {
        _db = db;
    }

    public async Task<CreatureBuilderDataDto> GetDataAsync(CancellationToken ct)
    {
        var parts = await _db.BodyParts
            .Select(p => new CreatureBuilderPartDto(p.Key, p.Name, p.Image, p.IsBaseLocked))
            .ToListAsync(ct);

        var animals = await _db.Animals.Where(a => !a.IsHybrid)
            .Select(a => new CreatureBuilderAnimalDto(
                a.Src,
                a.Label,
                a.SupportedParts.Select(sp => sp.PartKey).ToList(),
                false)
            ).ToListAsync(ct);

        var config = await _db.BuilderConfigs.FirstOrDefaultAsync(ct);
        var baseLockedParts = await _db.BodyParts.Where(p => p.IsBaseLocked).Select(p => p.Key).ToListAsync(ct);

        return new CreatureBuilderDataDto(
            parts,
            animals,
            config?.BaseUnlockedAnimalCount ?? 3,
            baseLockedParts
        );
    }

    public async Task<UserAwareCreatureBuilderDataDto> GetUserAwareDataAsync(Guid userId, CancellationToken ct)
    {
        // Get user credit info
        var wallet = await _db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
        var credits = wallet?.Balance ?? 0;

        var hasEverPurchased = await _db.CreditTransactions
            .AnyAsync(t => t.UserId == userId && t.Type == CreditTransactionType.Purchase, ct);

        // Logic: Full access if user has ever purchased
        var hasFullAccess = hasEverPurchased;

        // Get base configuration
        var config = await _db.BuilderConfigs.FirstOrDefaultAsync(ct);
        var baseUnlockedAnimalCount = config?.BaseUnlockedAnimalCount ?? 3;

        // Get parts with lock status
        var allParts = await _db.BodyParts.ToListAsync(ct);
        List<CreatureBuilderPartDto> parts = allParts.Select(p => new CreatureBuilderPartDto(
            p.Key, 
            p.Name, 
            p.Image,
            IsLocked: !hasFullAccess && p.IsBaseLocked // Locked if no full access and part is base locked
        )).ToList();

        // Get animals with lock status
        var allAnimals = await _db.Animals
            .Where(a => !a.IsHybrid)
            .Include(a => a.SupportedParts)
            .ToListAsync(ct);

        var totalAnimalCount = allAnimals.Count;
        var animals = allAnimals.Select((a, index) => new CreatureBuilderAnimalDto(
            a.Src,
            a.Label,
            a.SupportedParts.Select(sp => sp.PartKey).ToList(),
            IsLocked: !hasFullAccess && index >= baseUnlockedAnimalCount 
            // Locked if no full access and beyond base count
        )).ToList();

        var unlockedAnimalCount = hasFullAccess ? totalAnimalCount : baseUnlockedAnimalCount;

        return new UserAwareCreatureBuilderDataDto(
            parts,
            animals,
            unlockedAnimalCount,
            totalAnimalCount,
            hasFullAccess,
            new UserCreditsInfoDto(credits, hasEverPurchased)
        );
    }
}
