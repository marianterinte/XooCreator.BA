using XooCreator.BA.Features.CreatureBuilder;

namespace XooCreator.BA.Services;

//public record CreatureBuilderPartDto(string Key, string Name, string Image, bool IsLocked);
//public record CreatureBuilderAnimalDto(string Src, string Label, IEnumerable<string> Supports, bool IsLocked);

public record CreatureBuilderDataDto(
    IEnumerable<CreatureBuilderPartDto> Parts,
    IEnumerable<CreatureBuilderAnimalDto> Animals,
    int BaseUnlockedAnimalCount,
    IEnumerable<string> BaseLockedParts
);

public interface ICreatureBuilderService
{
    Task<CreatureBuilderDataDto> GetDataAsync(CancellationToken ct);
    Task<UserAwareCreatureBuilderDataDto> GetUserAwareDataAsync(Guid userId, CancellationToken ct);
}
