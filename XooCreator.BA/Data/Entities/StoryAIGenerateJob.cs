namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Tracks background jobs for "Generate Full AI Story" from the story list (async flow).
/// Request payload is stored in RequestJson; worker deserializes and calls IGenerateFullStoryDraftHandler.
/// </summary>
public class StoryAIGenerateJob
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;
    /// <summary>Owner first name for draft metadata (set when enqueueing).</summary>
    public string? OwnerFirstName { get; set; }
    /// <summary>Owner last name for draft metadata (set when enqueueing).</summary>
    public string? OwnerLastName { get; set; }
    public string Locale { get; set; } = "ro-ro";
    public string Status { get; set; } = StoryAIGenerateJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    /// <summary>When Failed due to 429 rate limit, set to "RateLimitExceeded" for client display.</summary>
    public string? ErrorCode { get; set; }
    /// <summary>Set after draft is created (on completion).</summary>
    public string? StoryId { get; set; }
    /// <summary>JSON-serialized GenerateFullStoryDraftRequest or GeneratePrivateStoryRequest (when IsPrivateStoryGeneration).</summary>
    public string RequestJson { get; set; } = string.Empty;
    /// <summary>When true, job is for your-story private generation (credits + persist to StoryDefinition with IsPrivate).</summary>
    public bool IsPrivateStoryGeneration { get; set; }
    /// <summary>Optional progress message for SSE (e.g. "Generating text...").</summary>
    public string? ProgressMessage { get; set; }
}

public static class StoryAIGenerateJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
