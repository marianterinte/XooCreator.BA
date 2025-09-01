namespace XooCreator.BA.Services;

public record CreatureBuilderPartDto(string Key, string Name, string Image);
public record CreatureBuilderAnimalDto(string Src, string Label, IEnumerable<string> Supports);
public record CreatureBuilderDataDto(
    IEnumerable<CreatureBuilderPartDto> Parts,
    IEnumerable<CreatureBuilderAnimalDto> Animals,
    int BaseUnlockedAnimalCount,
    IEnumerable<string> BaseLockedParts
);

public interface ICreatureBuilderService
{
    Task<CreatureBuilderDataDto> GetDataAsync(CancellationToken ct);
}
