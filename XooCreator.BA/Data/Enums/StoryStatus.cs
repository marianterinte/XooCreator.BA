namespace XooCreator.BA.Data.Enums;

public enum StoryStatus
{
    Draft = 0,
    SentForApproval = 1,
    InReview = 2,
    Approved = 3,
    ChangesRequested = 4,
    Published = 5,
    Archived = 6,
    Retreated = 7
}

public static class StoryStatusExtensions
{
    public static StoryStatus FromDb(string? status)
    {
        var s = (status ?? "draft").Trim().ToLowerInvariant();
        return s switch
        {
            "sent_for_approval" => StoryStatus.SentForApproval,
            "in_review" => StoryStatus.InReview,
            "approved" => StoryStatus.Approved,
            "changes_requested" => StoryStatus.ChangesRequested,
            "published" => StoryStatus.Published,
            "archived" => StoryStatus.Archived,
            _ => StoryStatus.Draft
        };
    }

    public static string ToDb(this StoryStatus status)
        => status switch
        {
            StoryStatus.SentForApproval => "sent_for_approval",
            StoryStatus.InReview => "in_review",
            StoryStatus.Approved => "approved",
            StoryStatus.ChangesRequested => "changes_requested",
            StoryStatus.Published => "published",
            StoryStatus.Archived => "archived",
            _ => "draft"
        };
}
