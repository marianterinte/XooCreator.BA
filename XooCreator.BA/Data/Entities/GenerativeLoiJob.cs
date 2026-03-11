namespace XooCreator.BA.Data.Entities;

/// <summary>Async job for LOI generative: image + text, 1 credit, persisted to BestiaryItem + UserBestiary + BestiaryItemTranslation.</summary>
public class GenerativeLoiJob
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Locale { get; set; } = string.Empty;
    /// <summary>JSON: { "head": "Bunny", "body": "Hippo", "arms": "Giraffe" }</summary>
    public string CombinationJson { get; set; } = string.Empty;
    /// <summary>Queued, Running, Completed, Failed</summary>
    public string Status { get; set; } = "Queued";
    public string? ProgressMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime QueuedAtUtc { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    /// <summary>When completed, the created BestiaryItem Id (for FE to load result).</summary>
    public Guid? BestiaryItemId { get; set; }
    public string? ResultName { get; set; }
    public string? ResultImageUrl { get; set; }
    public string? ResultStoryText { get; set; }
}
