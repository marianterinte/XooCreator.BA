using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace XooCreator.BA.Data.Enums;

/// <summary>
/// Represents the status of a story in the system
/// </summary>
public enum StoryStatus
{
    /// <summary>
    /// Draft - story is being edited
    /// </summary>
    [Description("Draft")]
    Draft = 0,

    /// <summary>
    /// SentForApproval - story has been submitted for review
    /// </summary>
    [Description("Sent for Approval")]
    SentForApproval = 1,

    /// <summary>
    /// InReview - story is currently being reviewed
    /// </summary>
    [Description("In Review")]
    InReview = 2,

    /// <summary>
    /// Approved - story has been approved for publication
    /// </summary>
    [Description("Approved")]
    Approved = 3,

    /// <summary>
    /// ChangesRequested - reviewer requested changes
    /// </summary>
    [Description("Changes Requested")]
    ChangesRequested = 4,

    /// <summary>
    /// Published - story is published and available
    /// </summary>
    [Description("Published")]
    Published = 5,

    /// <summary>
    /// Archived - story has been archived
    /// </summary>
    [Description("Archived")]
    Archived = 6,

    /// <summary>
    /// Retreated - story has been retracted
    /// </summary>
    [Description("Retreated")]
    Retreated = 7
}

public static class StoryStatusExtensions
{
    /// <summary>
    /// Converts a database status string to StoryStatus enum
    /// </summary>
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

    /// <summary>
    /// Converts StoryStatus enum to database string format
    /// </summary>
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

    /// <summary>
    /// Gets the description attribute value for the status
    /// </summary>
    public static string GetDescription(this StoryStatus status)
    {
        var field = status.GetType().GetField(status.ToString());
        if (field == null) return status.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? status.ToString();
    }

    /// <summary>
    /// Gets all available status values with their descriptions
    /// </summary>
    public static Dictionary<StoryStatus, string> GetAllWithDescriptions()
    {
        return Enum.GetValues<StoryStatus>()
            .ToDictionary(status => status, status => status.GetDescription());
    }
}
