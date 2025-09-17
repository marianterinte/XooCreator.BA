using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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
        var baseUnlockedAnimalIds = GetBaseUnlockedAnimalIds(config);
        var baseUnlockedBodyPartKeys = GetBaseUnlockedBodyPartKeys(config);
        var baseLockedParts = await _db.BodyParts.Where(p => p.IsBaseLocked).Select(p => p.Key).ToListAsync(ct);

        return new CreatureBuilderDataDto(
            parts,
            animals,
            baseUnlockedAnimalIds.Count,
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
        var baseUnlockedAnimalIds = GetBaseUnlockedAnimalIds(config);
        var baseUnlockedBodyPartKeys = GetBaseUnlockedBodyPartKeys(config);

        // Get parts with lock status
        var allParts = await _db.BodyParts.ToListAsync(ct);
        List<CreatureBuilderPartDto> parts = allParts.Select(p => new CreatureBuilderPartDto(
            p.Key, 
            p.Name, 
            p.Image,
            IsLocked: !hasFullAccess && !baseUnlockedBodyPartKeys.Contains(p.Key) // Locked if no full access and not in base unlocked list
        )).ToList();

        // Get animals with lock status
        var allAnimals = await _db.Animals
            .Where(a => !a.IsHybrid)
            .Include(a => a.SupportedParts)
            .ToListAsync(ct);

        var totalAnimalCount = allAnimals.Count;
        var animals = allAnimals.Select(a => new CreatureBuilderAnimalDto(
            a.Src,
            a.Label,
            a.SupportedParts.Select(sp => sp.PartKey).ToList(),
            IsLocked: !hasFullAccess && !baseUnlockedAnimalIds.Contains(a.Id.ToString())
            // Locked if no full access and not in base unlocked list
        )).ToList();

        var unlockedAnimalCount = hasFullAccess ? totalAnimalCount : baseUnlockedAnimalIds.Count;

        return new UserAwareCreatureBuilderDataDto(
            parts,
            animals,
            unlockedAnimalCount,
            totalAnimalCount,
            hasFullAccess,
            new UserCreditsInfoDto(credits, hasEverPurchased)
        );
    }

    private static List<string> GetBaseUnlockedAnimalIds(BuilderConfig? config)
    {
        if (config?.BaseUnlockedAnimalIds == null)
            return new List<string> { "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", "00000000-0000-0000-0000-000000000003" }; // Default: Bunny, Cat, Giraffe

        try
        {
            return JsonSerializer.Deserialize<List<string>>(config.BaseUnlockedAnimalIds) ?? new List<string>();
        }
        catch
        {
            return new List<string> { "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", "00000000-0000-0000-0000-000000000003" }; // Fallback
        }
    }

    private static List<string> GetBaseUnlockedBodyPartKeys(BuilderConfig? config)
    {
        if (config?.BaseUnlockedBodyPartKeys == null)
            return new List<string> { "head", "body", "arms" }; // Default: first 3 body parts

        try
        {
            return JsonSerializer.Deserialize<List<string>>(config.BaseUnlockedBodyPartKeys) ?? new List<string>();
        }
        catch
        {
            return new List<string> { "head", "body", "arms" }; // Fallback
        }
    }
}
