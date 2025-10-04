namespace XooCreator.BA.Features.Bestiary;

public record BestiaryItemDto(
    Guid Id,
    string Name,
    string ImageUrl,
    string Story,
    DateTime DiscoveredAt,
    string BestiaryType
);

public record BestiaryResponse(List<BestiaryItemDto> Items);


