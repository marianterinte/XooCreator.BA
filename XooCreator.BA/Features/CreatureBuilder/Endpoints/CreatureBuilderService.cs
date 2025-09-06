using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

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
            .Select(p => new CreatureBuilderPartDto(p.Key, p.Name, p.Image))
            .ToListAsync(ct);

        var animals = await _db.Animals.Where(a => !a.IsHybrid)
            .Select(a => new CreatureBuilderAnimalDto(
                a.Src,
                a.Label,
                a.SupportedParts.Select(sp => sp.PartKey)))
            .ToListAsync(ct);

        var config = await _db.BuilderConfigs.FirstOrDefaultAsync(ct);
        var baseLockedParts = await _db.BodyParts.Where(p => p.IsBaseLocked).Select(p => p.Key).ToListAsync(ct);

        return new CreatureBuilderDataDto(
            parts,
            animals,
            config?.BaseUnlockedAnimalCount ?? 3,
            baseLockedParts
        );
    }
}
