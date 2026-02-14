namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Records that a user printed a story (client-side PDF generation).
/// Used for audit/analytics; no blob or job.
/// </summary>
public class StoryPrintRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public DateTime PrintedAtUtc { get; set; }
    public string LanguageCode { get; set; } = "ro-ro";
    public bool IsDraft { get; set; }
}
