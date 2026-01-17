using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.AlchimaliaUniverse.DTOs;

#region Hero Definition DTOs

public record HeroDefinitionDto
{
    public required string Id { get; init; }
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
    public Guid? Id { get; init; } // Optional - backend generates new GUID for new translations
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Story { get; init; }
    public string? AudioUrl { get; init; }
}

public record HeroDefinitionListItemDto
{
    public required string Id { get; init; }
    public required string Name { get; init; } // From translation (for selected language or first available)
    public string? Image { get; init; }
    public required string Status { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid? CreatedByUserId { get; init; }
    public List<string> AvailableLanguages { get; init; } = new(); // List of language codes that have translations
}

public record CreateHeroDefinitionRequest
{
    public string? Id { get; init; }
    public int? CourageCost { get; init; }
    public int? CuriosityCost { get; init; }
    public int? ThinkingCost { get; init; }
    public int? CreativityCost { get; init; }
    public int? SafetyCost { get; init; }
    public string PrerequisitesJson { get; init; } = "[]";
    public string RewardsJson { get; init; } = "[]";
    public double PositionX { get; init; }
    public double PositionY { get; init; }
    public string Image { get; init; } = string.Empty;
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Story { get; init; }
    public string? AudioUrl { get; init; }
}

public record UpdateHeroDefinitionRequest
{
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

#region Hero Craft DTOs

public record HeroDefinitionCraftDto
{
    public required string Id { get; init; }
    public string? PublishedDefinitionId { get; init; }
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
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<HeroDefinitionTranslationDto> Translations { get; init; } = new();
}

public record HeroDefinitionCraftListItemDto
{
    public required string Id { get; init; }
    public string? PublishedDefinitionId { get; init; }
    public required string Name { get; init; }
    public string? Image { get; init; }
    public required string Status { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid? CreatedByUserId { get; init; }
    public List<string> AvailableLanguages { get; init; } = new();
}

public record CreateHeroDefinitionCraftRequest
{
    public string? Id { get; init; }
    public int? CourageCost { get; init; }
    public int? CuriosityCost { get; init; }
    public int? ThinkingCost { get; init; }
    public int? CreativityCost { get; init; }
    public int? SafetyCost { get; init; }
    public string PrerequisitesJson { get; init; } = "[]";
    public string RewardsJson { get; init; } = "[]";
    public double PositionX { get; init; }
    public double PositionY { get; init; }
    public string Image { get; init; } = string.Empty;
    public required string LanguageCode { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Story { get; init; }
    public string? AudioUrl { get; init; }
}

public record UpdateHeroDefinitionCraftRequest
{
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

public record ReviewHeroDefinitionCraftRequest
{
    public required bool Approve { get; init; }
    public string? Notes { get; init; }
}

public record ListHeroDefinitionCraftsResponse
{
    public List<HeroDefinitionCraftListItemDto> Heroes { get; init; } = new();
    public int TotalCount { get; init; }
}

#endregion

#region Tree of Heroes Config DTOs

public record TreeOfHeroesConfigNodeDto
{
    public Guid Id { get; init; }
    public required string HeroDefinitionId { get; init; }
    public double PositionX { get; init; }
    public double PositionY { get; init; }
    public int CourageCost { get; init; }
    public int CuriosityCost { get; init; }
    public int ThinkingCost { get; init; }
    public int CreativityCost { get; init; }
    public int SafetyCost { get; init; }
    public bool? IsStartup { get; init; }
    public List<string>? Prerequisites { get; init; } // Array of hero definition IDs that must be unlocked before this node
}

public record TreeOfHeroesConfigEdgeDto
{
    public Guid Id { get; init; }
    public required string FromHeroId { get; init; }
    public required string ToHeroId { get; init; }
}

public record TreeOfHeroesConfigCraftDto
{
    public Guid Id { get; init; }
    public Guid? PublishedDefinitionId { get; init; }
    public required string Label { get; init; }
    public string Status { get; init; } = "draft";
    public Guid? CreatedByUserId { get; init; }
    public Guid? ReviewedByUserId { get; init; }
    public string? ReviewNotes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<TreeOfHeroesConfigNodeDto> Nodes { get; init; } = new();
    public List<TreeOfHeroesConfigEdgeDto> Edges { get; init; } = new();
}

public record TreeOfHeroesConfigDefinitionDto
{
    public Guid Id { get; init; }
    public required string Label { get; init; }
    public string Status { get; init; } = "published";
    public Guid? PublishedByUserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    public List<TreeOfHeroesConfigNodeDto> Nodes { get; init; } = new();
    public List<TreeOfHeroesConfigEdgeDto> Edges { get; init; } = new();
}

public record TreeOfHeroesConfigListItemDto
{
    public Guid Id { get; init; }
    public required string Label { get; init; }
    public string Status { get; init; } = "draft";
    public DateTime UpdatedAt { get; init; }
    public Guid? CreatedByUserId { get; init; }
}

public record CreateTreeOfHeroesConfigCraftRequest
{
    public required string Label { get; init; }
    public List<TreeOfHeroesConfigNodeDto> Nodes { get; init; } = new();
    public List<TreeOfHeroesConfigEdgeDto> Edges { get; init; } = new();
}

public record UpdateTreeOfHeroesConfigCraftRequest
{
    public string? Label { get; init; }
    public List<TreeOfHeroesConfigNodeDto>? Nodes { get; init; }
    public List<TreeOfHeroesConfigEdgeDto>? Edges { get; init; }
}

public record ReviewTreeOfHeroesConfigCraftRequest
{
    public required bool Approve { get; init; }
    public string? Notes { get; init; }
}

public record ListTreeOfHeroesConfigCraftsResponse
{
    public List<TreeOfHeroesConfigListItemDto> Configs { get; init; } = new();
    public int TotalCount { get; init; }
}

public record ListTreeOfHeroesConfigDefinitionsResponse
{
    public List<TreeOfHeroesConfigListItemDto> Configs { get; init; } = new();
    public int TotalCount { get; init; }
}

#endregion

#region Animal DTOs

public record AnimalHybridPartDto
{
    public Guid SourceAnimalId { get; init; }
    public required string BodyPartKey { get; init; }
    public int OrderIndex { get; init; }
}

public record AnimalDto
{
    public Guid Id { get; init; }
    public required string Label { get; init; }
    public required string Src { get; init; }
    public bool IsHybrid { get; init; }
    public Guid? RegionId { get; init; }
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
    public List<AnimalHybridPartDto> HybridParts { get; init; } = new();
    public List<AnimalTranslationDto> Translations { get; init; } = new();
}

public record AnimalTranslationDto
{
    public Guid Id { get; init; }
    public required string LanguageCode { get; init; }
    public required string Label { get; init; }
    public string? AudioUrl { get; init; }
}

public record AnimalListItemDto
{
    public Guid Id { get; init; }
    public required string Label { get; init; } // From translation or base
    public string? Src { get; init; }
    public bool IsHybrid { get; init; }
    public Guid? RegionId { get; init; }
    public string? RegionName { get; init; }
    public required string Status { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public Guid? CreatedByUserId { get; init; }
    public List<string> AvailableLanguages { get; init; } = new(); // List of language codes that have translations
}

public record CreateAnimalRequest
{
    public required string Label { get; init; }
    public required string Src { get; init; }
    public bool IsHybrid { get; init; }
    public Guid? RegionId { get; init; }
    public List<string> SupportedParts { get; init; } = new();
    public List<AnimalHybridPartDto> HybridParts { get; init; } = new();
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
    public List<AnimalHybridPartDto>? HybridParts { get; init; }
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

#region Animal Craft DTOs

public record AnimalCraftDto
{
    public Guid Id { get; init; }
    public Guid? PublishedDefinitionId { get; init; }
    public required string Label { get; init; }
    public required string Src { get; init; }
    public bool IsHybrid { get; init; }
    public Guid? RegionId { get; init; }
    public string? RegionName { get; init; }
    public string Status { get; init; } = "draft";
    public Guid? CreatedByUserId { get; init; }
    public Guid? ReviewedByUserId { get; init; }
    public string? ReviewNotes { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<string> SupportedParts { get; init; } = new();
    public List<AnimalHybridPartDto> HybridParts { get; init; } = new();
    public List<AnimalTranslationDto> Translations { get; init; } = new();
}

public record AnimalCraftListItemDto
{
    public Guid Id { get; init; }
    public Guid? PublishedDefinitionId { get; init; }
    public required string Label { get; init; }
    public string? Src { get; init; }
    public bool IsHybrid { get; init; }
    public Guid? RegionId { get; init; }
    public string? RegionName { get; init; }
    public required string Status { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public Guid? CreatedByUserId { get; init; }
    public List<string> AvailableLanguages { get; init; } = new();
}

public record CreateAnimalCraftRequest
{
    public required string Label { get; init; }
    public required string Src { get; init; }
    public bool IsHybrid { get; init; }
    public Guid? RegionId { get; init; }
    public List<string> SupportedParts { get; init; } = new();
    public List<AnimalHybridPartDto> HybridParts { get; init; } = new();
    public required string LanguageCode { get; init; }
    public required string TranslatedLabel { get; init; }
}

public record UpdateAnimalCraftRequest
{
    public string? Label { get; init; }
    public string? Src { get; init; }
    public bool? IsHybrid { get; init; }
    public Guid? RegionId { get; init; }
    public List<string>? SupportedParts { get; init; }
    public List<AnimalHybridPartDto>? HybridParts { get; init; }
    public Dictionary<string, AnimalTranslationDto>? Translations { get; init; }
}

public record ReviewAnimalCraftRequest
{
    public required bool Approve { get; init; }
    public string? Notes { get; init; }
}

public record ListAnimalCraftsResponse
{
    public List<AnimalCraftListItemDto> Animals { get; init; } = new();
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
    public List<string> AvailableLanguages { get; init; } = new(); // List of language codes that have translations
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
