namespace XooCreator.BA.Features.TreeOfHeroes.DTOs;

public record HeroDefinitionDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Story { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public int CourageCost { get; init; }
    public int CuriosityCost { get; init; }
    public int ThinkingCost { get; init; }
    public int CreativityCost { get; init; }
    public int SafetyCost { get; init; }
    public string PrerequisitesJson { get; init; } = string.Empty;
    public string RewardsJson { get; init; } = string.Empty;
    public bool IsUnlocked { get; init; }
    public double PositionX { get; init; }
    public double PositionY { get; init; }
}

public record UserTokensDto
{
    public int Courage { get; init; }
    public int Curiosity { get; init; }
    public int Thinking { get; init; }
    public int Creativity { get; init; }
    public int Safety { get; init; }
}

/// <summary>Single token balance item for GET tokens/all (all types + quantities).</summary>
public record TokenBalanceItemDto
{
    public required string Type { get; init; }
    public required string Value { get; init; }
    public int Quantity { get; init; }
}

public record HeroDto
{
    public required string HeroId { get; init; }
    public required string HeroType { get; init; }
    public string? SourceStoryId { get; init; }
    public DateTime UnlockedAt { get; init; }
}

public record HeroTreeNodeDto
{
    public required string NodeId { get; init; }
    public int TokensCostCourage { get; init; }
    public int TokensCostCuriosity { get; init; }
    public int TokensCostThinking { get; init; }
    public int TokensCostCreativity { get; init; }
    public int TokensCostSafety { get; init; }
    public DateTime UnlockedAt { get; init; }
}

public record UnlockHeroTreeNodeRequest
{
    public required string NodeId { get; init; }
    public int TokensCostCourage { get; init; }
    public int TokensCostCuriosity { get; init; }
    public int TokensCostThinking { get; init; }
    public int TokensCostCreativity { get; init; }
    public int TokensCostSafety { get; init; }
}

public record UnlockHeroTreeNodeResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public UserTokensDto? UpdatedTokens { get; init; }
}

public record TransformToHeroRequest
{
    public required string HeroId { get; init; }
}

public record TransformToHeroResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public HeroDto? UnlockedHero { get; init; }
    public List<string>? NewlyUnlockedNodes { get; init; }
    public UserTokensDto? UpdatedTokens { get; init; }
}

public record TokenConfigDto
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public required string Trait { get; init; }
    public required string Icon { get; init; }
    public double Angle { get; init; }
}

public record HeroImageDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Image { get; init; }
}

public record TreeOfHeroesConfigDto
{
    public required List<TokenConfigDto> Tokens { get; init; }
    public required List<HeroImageDto> HeroImages { get; init; }
    public required List<string> BaseHeroIds { get; init; }
    public required Dictionary<string, string> CanonicalHybridByPair { get; init; }
    /// <summary>Maps personality trait to base hero ID (level 1), e.g. "thinking" -> "hero_wise_owl".</summary>
    public required Dictionary<string, string> TraitToBaseHeroId { get; init; }
    /// <summary>Maps trait to [level1HeroId, level2HeroId, level3HeroId].</summary>
    public required Dictionary<string, List<string>> HeroIdsByTraitAndLevel { get; init; }
}

public record ResetPersonalityTokensResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int TokensReturned { get; init; }
}

/// <summary>Alchimalian Hero profile: selected hero for display and profile picture; discovered hero IDs for ??? UI.</summary>
public record AlchimalianHeroProfileDto
{
    public string? SelectedHeroId { get; init; }
    public string? SelectedHeroImageUrl { get; init; }
    /// <summary>Hero IDs the user has discovered (tapped Descoperă / Vezi ce erou poți deveni).</summary>
    public IReadOnlyList<string> DiscoveredHeroIds { get; init; } = Array.Empty<string>();
}

public record UpdateAlchimalianHeroProfileRequest
{
    public string? SelectedHeroId { get; init; }
}

public record UpdateAlchimalianHeroProfileResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public AlchimalianHeroProfileDto? Profile { get; init; }
}

public record DiscoverAlchimalianHeroRequest
{
    public required string HeroId { get; init; }
    /// <summary>If true, set this hero as the selected hero after discovering.</summary>
    public bool SetAsSelected { get; init; } = true;
}

public record DiscoverAlchimalianHeroResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public AlchimalianHeroProfileDto? Profile { get; init; }
}

/// <summary>Response for GET alchimalian-hero/available: heroes the user can display (no token consumption).</summary>
public record AlchimalianHeroAvailableResponse
{
    public required List<string> AvailableHeroIds { get; init; }
    public string? RecommendedHeroId { get; init; }
    public required List<string> UpgradeOptionHeroIds { get; init; }
    public required UserTokensDto Tokens { get; init; }
}



