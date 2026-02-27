namespace XooCreator.BA.Features.RewardTokens.DTOs;

public record RewardTokenDto
{
    public Guid Id { get; init; }
    public required string Type { get; init; }
    public required string Value { get; init; }
    public required string DisplayNameKey { get; init; }
    public string? Icon { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
}

public record RewardTokenCreateOrUpdateDto
{
    public required string Type { get; init; }
    public required string Value { get; init; }
    public required string DisplayNameKey { get; init; }
    public string? Icon { get; init; }
    public int SortOrder { get; init; }
}
