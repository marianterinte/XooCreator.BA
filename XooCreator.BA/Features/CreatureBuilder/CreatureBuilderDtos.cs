namespace XooCreator.BA.Features.CreatureBuilder;

public record CreatureBuilderPartDto(string Key, string Name, string Image, bool IsLocked);
public record CreatureBuilderAnimalDto(string Src, string Label, List<string> Supports, bool IsLocked);

public record UserAwareCreatureBuilderDataDto(
    List<CreatureBuilderPartDto> Parts,
    List<CreatureBuilderAnimalDto> Animals,
    int UnlockedAnimalCount,
    int TotalAnimalCount,
    bool HasFullAccess,
    UserCreditsInfoDto Credits
);

public record UserCreditsInfoDto(
    int Balance,
    bool HasEverPurchased,
    int Discovery,
    int Generative
);
