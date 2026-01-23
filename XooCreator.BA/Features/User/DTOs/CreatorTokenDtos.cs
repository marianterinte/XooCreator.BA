namespace XooCreator.BA.Features.User.DTOs;

/// <summary>
/// DTO for creator token balance
/// </summary>
public record CreatorTokenBalanceDto
{
    public required string TokenType { get; init; } // e.g., "Alchimalia", "Personality", "Alchemy"
    public required string TokenValue { get; init; } // e.g., "Alchimalia Token", "courage", "Karott"
    public int Quantity { get; init; }
    public bool IsAdminOverride { get; init; }
    public DateTime? OverrideAt { get; init; }
}

/// <summary>
/// DTO for getting all tokens for a creator
/// </summary>
public record GetCreatorTokensResponse
{
    public Guid UserId { get; init; }
    public List<CreatorTokenBalanceDto> Tokens { get; init; } = new();
}

/// <summary>
/// DTO for admin override request
/// </summary>
public record OverrideCreatorTokenRequest
{
    public required string TokenType { get; init; }
    public required string TokenValue { get; init; }
    public int Quantity { get; init; }
}
