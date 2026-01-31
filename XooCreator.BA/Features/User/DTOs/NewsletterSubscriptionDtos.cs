namespace XooCreator.BA.Features.User.DTOs;

public record UpdateNewsletterSubscriptionRequest
{
    public bool IsSubscribed { get; init; }
}

public record UpdateNewsletterSubscriptionResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public bool IsSubscribed { get; init; }
}

public record GetNewsletterSubscriptionResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public bool IsSubscribed { get; init; }
    public DateTime? SubscribedAt { get; init; }
    public DateTime? UnsubscribedAt { get; init; }
}
