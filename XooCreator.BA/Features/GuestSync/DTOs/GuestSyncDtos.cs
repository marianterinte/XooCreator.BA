namespace XooCreator.BA.Features.GuestSync.DTOs;

/// <summary>
/// Request body for syncing guest progress to authenticated user after login.
/// </summary>
public record GuestSyncRequest
{
    public List<string> CompletedStoryIds { get; init; } = new();
    public List<GuestTokenBalanceDto> TokenBalances { get; init; } = new();
    public List<string> LikedStoryIds { get; init; } = new();
    public List<string> FavoriteStoryIds { get; init; } = new();
    public List<string> FavoriteEpicIds { get; init; } = new();
}

public record GuestTokenBalanceDto
{
    public string Type { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public int Quantity { get; init; }
}

/// <summary>
/// Response for guest sync operation.
/// </summary>
public record GuestSyncResponse
{
    public bool Success { get; init; }
    public List<string> Warnings { get; init; } = new();
}
