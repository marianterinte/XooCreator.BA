namespace XooCreator.BA.Features.HeroStoryRewards.DTOs;

/// <summary>
/// Request to complete a story and award tokens (generic for indie, epic, or legacy sources).
/// </summary>
public record CompleteStoryRewardRequest
{
    public required string StoryId { get; init; }
    /// <summary>Source of the story: "indie", "epic", or "legacy".</summary>
    public string Source { get; init; } = "indie";
    public string? SelectedAnswer { get; init; }
    public List<TokenRewardDto>? Tokens { get; init; }
    public string? EpicId { get; init; }
}

public record TokenRewardDto
{
    public string Type { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public int Quantity { get; init; }
}

/// <summary>
/// Response for complete story reward. Completion is never blocked by reward validation.
/// </summary>
public record CompleteStoryRewardResponse
{
    public bool Completed { get; init; }
    public bool TokensAwarded { get; init; }
    public List<string> Warnings { get; init; } = new();
}
