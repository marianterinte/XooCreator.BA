namespace XooCreator.BA.Features.Bestiary;

public record DiscoveryBestiaryItemDto(
    Guid Id,
    string Name,
    string ImageUrl,
    string Story,
    DateTime DiscoveredAt
);


