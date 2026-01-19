using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace XooCreator.BA.Data.Enums;

/// <summary>
/// Represents the status of an entity in Alchimalia Universe Editor
/// </summary>
public enum AlchimaliaUniverseStatus
{
    /// <summary>
    /// Draft - entity is being edited
    /// </summary>
    [Description("Draft")]
    Draft = 0,

    /// <summary>
    /// SentForApproval - entity has been submitted for review
    /// </summary>
    [Description("Sent for Approval")]
    SentForApproval = 1,

    /// <summary>
    /// InReview - entity is currently being reviewed
    /// </summary>
    [Description("In Review")]
    InReview = 2,

    /// <summary>
    /// Approved - entity has been approved for publication
    /// </summary>
    [Description("Approved")]
    Approved = 3,

    /// <summary>
    /// ChangesRequested - reviewer requested changes
    /// </summary>
    [Description("Changes Requested")]
    ChangesRequested = 4,

    /// <summary>
    /// Published - entity is published and available
    /// </summary>
    [Description("Published")]
    Published = 5,

    /// <summary>
    /// Archived - entity has been archived
    /// </summary>
    [Description("Archived")]
    Archived = 6
}

public static class AlchimaliaUniverseStatusExtensions
{
    /// <summary>
    /// Converts a database status string to AlchimaliaUniverseStatus enum
    /// </summary>
    public static AlchimaliaUniverseStatus FromDb(string? status)
    {
        var s = (status ?? "draft").Trim().ToLowerInvariant();
        return s switch
        {
            "sent_for_approval" => AlchimaliaUniverseStatus.SentForApproval,
            "in_review" => AlchimaliaUniverseStatus.InReview,
            "approved" => AlchimaliaUniverseStatus.Approved,
            "changes_requested" => AlchimaliaUniverseStatus.ChangesRequested,
            "published" => AlchimaliaUniverseStatus.Published,
            "archived" => AlchimaliaUniverseStatus.Archived,
            _ => AlchimaliaUniverseStatus.Draft
        };
    }

    /// <summary>
    /// Converts AlchimaliaUniverseStatus enum to database string format
    /// </summary>
    public static string ToDb(this AlchimaliaUniverseStatus status)
        => status switch
        {
            AlchimaliaUniverseStatus.SentForApproval => "sent_for_approval",
            AlchimaliaUniverseStatus.InReview => "in_review",
            AlchimaliaUniverseStatus.Approved => "approved",
            AlchimaliaUniverseStatus.ChangesRequested => "changes_requested",
            AlchimaliaUniverseStatus.Published => "published",
            AlchimaliaUniverseStatus.Archived => "archived",
            _ => "draft"
        };

    /// <summary>
    /// Gets the description attribute value for the status
    /// </summary>
    public static string GetDescription(this AlchimaliaUniverseStatus status)
    {
        var field = status.GetType().GetField(status.ToString());
        if (field == null) return status.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? status.ToString();
    }

    /// <summary>
    /// Gets all available status values with their descriptions
    /// </summary>
    public static Dictionary<AlchimaliaUniverseStatus, string> GetAllWithDescriptions()
    {
        return Enum.GetValues<AlchimaliaUniverseStatus>()
            .ToDictionary(status => status, status => status.GetDescription());
    }
}
