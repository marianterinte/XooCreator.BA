namespace XooCreator.BA.Features.TreeOfHeroes;

public record HeroDefinitionDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = string.Empty;
    public required string Type { get; init; }
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

// New DTOs for UI configuration
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
}