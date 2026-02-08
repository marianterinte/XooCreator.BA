namespace XooCreator.BA.Data.Entities;

public class StoryTranslationJob
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string ReferenceLanguage { get; set; } = "ro-ro";
    public string TargetLanguagesJson { get; set; } = "[]";
    public string? SelectedTileIdsJson { get; set; }
    public string? ApiKeyOverride { get; set; }
    public string Status { get; set; } = StoryTranslationJobStatus.Queued;
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public int? FieldsTranslated { get; set; }
    public string? UpdatedLanguagesJson { get; set; }
}

public static class StoryTranslationJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
