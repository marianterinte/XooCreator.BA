using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.AlchimaliaUniverse.DTOs;

#region Hero Definition DTOs

public record HeroDefinitionDto
{
    public required string Id { get; init; }
    public required string Type { get; init; }
    public int CourageCost { get; init; }
    public int CuriosityCost { get; init; }
    public int ThinkingCost { get; init; }
    public int CreativityCost { get; init; }
    public int SafetyCost { get; init; }
    public string PrerequisitesJson { get; init; } = "[]";
    public string RewardsJson { get; init; } = "[]";
    public bool IsUnlocked { get; init; }
    public double PositionX { get; init; }
    public double PositionY { get; init; }
    public string Image { get; init; } = string.Empty;
    public string Status { get; init; } = "draft";
    public Guid? CreatedByUserId { get; init; }
    public Guid? ReviewedByUserId { get; init; }
    public string? ReviewNotes { get; init; }
    public int Version { get; init; } = 1;
    public string? ParentVersionId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<HeroDefinitionTranslationDto> Translations { get; init; } = new();
}

public record HeroDefinitionTranslationDto
{
    public Guid Id { get; init; }
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Story { get; init; }
}

public record HeroDefinitionListItemDto
{
    public required string Id { get; init; }
    public required string Type { get; init; }
    public required string Name { get; init; } // From translation
    public string? Image { get; init; }
    public required string Status { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid? CreatedByUserId { get; init; }
}

public record CreateHeroDefinitionRequest
{
    public string? Id { get; init; }
    public required string Type { get; init; }
    public int CourageCost { get; init; }
    public int CuriosityCost { get; init; }
    public int ThinkingCost { get; init; }
    public int CreativityCost { get; init; }
    public int SafetyCost { get; init; }
    public string PrerequisitesJson { get; init; } = "[]";
    public string RewardsJson { get; init; } = "[]";
    public double PositionX { get; init; }
    public double PositionY { get; init; }
    public string Image { get; init; } = string.Empty;
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Story { get; init; }
}

public record UpdateHeroDefinitionRequest
{
    public string? Type { get; init; }
    public int? CourageCost { get; init; }
    public int? CuriosityCost { get; init; }
    public int? ThinkingCost { get; init; }
    public int? CreativityCost { get; init; }
    public int? SafetyCost { get; init; }
    public string? PrerequisitesJson { get; init; }
    public string? RewardsJson { get; init; }
    public bool? IsUnlocked { get; init; }
    public double? PositionX { get; init; }
    public double? PositionY { get; init; }
    public string? Image { get; init; }
    public Dictionary<string, HeroDefinitionTranslationDto>? Translations { get; init; }
}

public record ReviewHeroDefinitionRequest
{
    public required bool Approve { get; init; }
    public string? Notes { get; init; }
}

public record ListHeroDefinitionsResponse
{
    public List<HeroDefinitionListItemDto> Heroes { get; init; } = new();
    public int TotalCount { get; init; }
}

#endregion

#region Animal DTOs

public record AnimalDto
{
    public Guid Id { get; init; }
    public required string Label { get; init; }
    public required string Src { get; init; }
    public bool IsHybrid { get; init; }
    public Guid RegionId { get; init; }
    public string? RegionName { get; init; }
    public string Status { get; init; } = "draft";
    public Guid? CreatedByUserId { get; init; }
    public Guid? ReviewedByUserId { get; init; }
    public string? ReviewNotes { get; init; }
    public int Version { get; init; } = 1;
    public Guid? ParentVersionId { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<string> SupportedParts { get; init; } = new();
    public List<AnimalTranslationDto> Translations { get; init; } = new();
}

public record AnimalTranslationDto
{
    public Guid Id { get; init; }
    public required string LanguageCode { get; init; }
    public required string Label { get; init; }
}

public record AnimalListItemDto
{
    public Guid Id { get; init; }
    public required string Label { get; init; } // From translation or base
    public string? Src { get; init; }
    public bool IsHybrid { get; init; }
    public Guid RegionId { get; init; }
    public string? RegionName { get; init; }
    public required string Status { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public Guid? CreatedByUserId { get; init; }
}

public record CreateAnimalRequest
{
    public required string Label { get; init; }
    public required string Src { get; init; }
    public bool IsHybrid { get; init; }
    public required Guid RegionId { get; init; }
    public List<string> SupportedParts { get; init; } = new();
    public required string LanguageCode { get; init; }
    public required string TranslatedLabel { get; init; }
}

public record UpdateAnimalRequest
{
    public string? Label { get; init; }
    public string? Src { get; init; }
    public bool? IsHybrid { get; init; }
    public Guid? RegionId { get; init; }
    public List<string>? SupportedParts { get; init; }
    public Dictionary<string, AnimalTranslationDto>? Translations { get; init; }
}

public record ReviewAnimalRequest
{
    public required bool Approve { get; init; }
    public string? Notes { get; init; }
}

public record ListAnimalsResponse
{
    public List<AnimalListItemDto> Animals { get; init; } = new();
    public int TotalCount { get; init; }
}

#endregion

#region Story Hero DTOs

public record StoryHeroDto
{
    public Guid Id { get; init; }
    public required string HeroId { get; init; }
    public required string ImageUrl { get; init; }
    public string UnlockConditionsJson { get; init; } = "{}";
    public string Status { get; init; } = "draft";
    public Guid? CreatedByUserId { get; init; }
    public Guid? ReviewedByUserId { get; init; }
    public string? ReviewNotes { get; init; }
    public int Version { get; init; } = 1;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<StoryHeroTranslationDto> Translations { get; init; } = new();
}

public record StoryHeroTranslationDto
{
    public Guid Id { get; init; }
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? GreetingText { get; init; }
    public string? GreetingAudioUrl { get; init; }
}

public record StoryHeroListItemDto
{
    public Guid Id { get; init; }
    public required string HeroId { get; init; }
    public required string Name { get; init; } // From translation
    public string? ImageUrl { get; init; }
    public required string Status { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid? CreatedByUserId { get; init; }
}

public record CreateStoryHeroRequest
{
    public required string HeroId { get; init; }
    public required string ImageUrl { get; init; }
    public string UnlockConditionsJson { get; init; } = "{}";
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? GreetingText { get; init; }
    public string? GreetingAudioUrl { get; init; }
}

public record UpdateStoryHeroRequest
{
    public string? ImageUrl { get; init; }
    public string? UnlockConditionsJson { get; init; }
    public Dictionary<string, StoryHeroTranslationDto>? Translations { get; init; }
}

public record ReviewStoryHeroRequest
{
    public required bool Approve { get; init; }
    public string? Notes { get; init; }
}

public record ListStoryHeroesResponse
{
    public List<StoryHeroListItemDto> Heroes { get; init; } = new();
    public int TotalCount { get; init; }
}

#endregion
