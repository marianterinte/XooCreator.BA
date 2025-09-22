using XooCreator.BA.Features.TreeOfHeroes;

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
    public UserTokensDto UserTokens { get; init; } = new() { Courage = 0, Curiosity = 0, Thinking = 0, Creativity = 0, Safety = 0 };
}

public record CompletedStoryDto
{
    public required string StoryId { get; init; }
    public string? SelectedAnswer { get; init; }
    public List<TokenReward> Tokens { get; init; } = new();
    public DateTime CompletedAt { get; init; }
}
