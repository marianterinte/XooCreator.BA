namespace XooCreator.BA.Features.User.DTOs;

public record UserProfileDto
{
    public required string Id { get; init; }
    public required string DisplayName { get; init; }
    public required string Email { get; init; }
    public required UserCreditsDto Credits { get; init; }
    public required UserPermissionsDto Permissions { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record UserCreditsDto
{
    public int Balance { get; init; }
    public bool HasEverPurchased { get; init; }
    public DateTime? LastPurchaseAt { get; init; }
    public DateTime? LastTransactionAt { get; init; }
}

public record UserPermissionsDto
{
    public bool HasFullAccess { get; init; }
    public int UnlockedAnimalCount { get; init; }
    public List<string> UnlockedParts { get; init; } = new();
    public List<string> LockedParts { get; init; } = new();
}

public record GetUserProfileResponse
{
    public UserProfileDto? User { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record SpendCreditsRequest
{
    public int Amount { get; init; }
    public required string Reference { get; init; }
}

public record SpendCreditsResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int NewBalance { get; init; }
}
