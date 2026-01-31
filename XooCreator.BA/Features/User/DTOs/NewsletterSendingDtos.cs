namespace XooCreator.BA.Features.User.DTOs;

public record SendNewsletterRequest
{
    public required string Subject { get; init; }
    public required string HtmlContent { get; init; }
    public string? TextContent { get; init; }
    public List<string>? SelectedStoryIds { get; init; }
    public List<string>? SelectedEpicIds { get; init; }
}

public record SendNewsletterResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int TotalRecipients { get; init; }
    public int SentCount { get; init; }
    public int FailedCount { get; init; }
}

public record GetSubscribedUsersResponse
{
    public bool Success { get; init; }
    public List<string> Emails { get; init; } = new();
    public int Count { get; init; }
}

public record NewsletterStoryItem
{
    public required string StoryId { get; init; }
    public required string Title { get; init; }
    public string? Summary { get; init; }
    public string? CoverImageUrl { get; init; }
}

public record NewsletterEpicItem
{
    public required string EpicId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CoverImageUrl { get; init; }
}

public record GetNewsletterStoriesResponse
{
    public bool Success { get; init; }
    public List<NewsletterStoryItem> Stories { get; init; } = new();
}

public record GetNewsletterEpicsResponse
{
    public bool Success { get; init; }
    public List<NewsletterEpicItem> Epics { get; init; } = new();
}
