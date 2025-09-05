namespace XooCreator.BA.Features.TreeOfLight;

public record TreeProgressDto
{
    public required string RegionId { get; init; }
    public bool IsUnlocked { get; init; }
    public DateTime? UnlockedAt { get; init; }
}

public record StoryProgressDto
{
    public required string StoryId { get; init; }
    public string? SelectedAnswer { get; init; }
    public string? RewardReceived { get; init; }
    public DateTime CompletedAt { get; init; }
}

public record UserTokensDto
{
    public int Courage { get; init; }
    public int Curiosity { get; init; }
    public int Thinking { get; init; }
    public int Creativity { get; init; }
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
    public DateTime UnlockedAt { get; init; }
}

public record CompleteStoryRequest
{
    public required string StoryId { get; init; }
    public string? SelectedAnswer { get; init; }
    public string? RewardReceived { get; init; }
}

public record CompleteStoryResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public List<string> NewlyUnlockedRegions { get; init; } = new();
    public UserTokensDto? UpdatedTokens { get; init; }
}

public record UnlockHeroTreeNodeRequest
{
    public required string NodeId { get; init; }
    public int TokensCostCourage { get; init; }
    public int TokensCostCuriosity { get; init; }
    public int TokensCostThinking { get; init; }
    public int TokensCostCreativity { get; init; }
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
}
