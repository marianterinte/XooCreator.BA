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
    public List<TokenReward> Tokens { get; init; } = new();
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

public record TokenReward
{
    public required string TokenType { get; init; }
    public required int Quantity { get; init; }
}

public record CompleteStoryRequest
{
    public required string StoryId { get; init; }
    public string? SelectedAnswer { get; init; }
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

public record ResetProgressResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public string Message { get; init; } = string.Empty;
}

// Tree Model DTOs
public record TreeStateDto
{
    public TreeModelDto Model { get; init; } = new();
    public TreeProgressStateDto Progress { get; init; } = new();
}

public record TreeModelDto
{
    public List<TreeRegionDto> Regions { get; init; } = new();
    public List<TreeStoryDto> Stories { get; init; } = new();
    public List<TreeUnlockRuleDto> Rules { get; init; } = new();
}

public record TreeRegionDto
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public string? ImageUrl { get; init; }
    public string? PufpufMessage { get; init; }
    public int SortOrder { get; init; }
}

public record TreeStoryDto
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public required string RegionId { get; init; }
    public string? RewardImageUrl { get; init; }
    public int SortOrder { get; init; }
}

public record TreeUnlockRuleDto
{
    public required string Type { get; init; } // "story", "all", "any"
    public required string FromId { get; init; }
    public required string ToRegionId { get; init; }
    public List<string> RequiredStories { get; init; } = new();
    public int? MinCount { get; init; }
    public string? StoryId { get; init; }
}

public record TreeProgressStateDto
{
    public List<CompletedStoryDto> CompletedStories { get; init; } = new();
    public List<string> UnlockedRegions { get; init; } = new();
    public UserTokensDto UserTokens { get; init; } = new() { Courage = 0, Curiosity = 0, Thinking = 0, Creativity = 0 };
}

public record CompletedStoryDto
{
    public required string StoryId { get; init; }
    public string? SelectedAnswer { get; init; }
    public List<TokenReward> Tokens { get; init; } = new();
    public DateTime CompletedAt { get; init; }
}
