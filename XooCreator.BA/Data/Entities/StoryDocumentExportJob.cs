namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Background job for exporting a story to a printable document (PDF/DOCX).
/// Designed to support "reader" jobs (marketplace) by separating story owner from requestor.
/// </summary>
public class StoryDocumentExportJob
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty;

    // Story creator/owner (used for asset resolution and policy checks)
    public Guid StoryOwnerUserId { get; set; }

    // The user who requested this job (used for status/download authorization)
    public Guid RequestedByUserId { get; set; }
    public string RequestedByEmail { get; set; } = string.Empty;

    public string Locale { get; set; } = "ro-ro";
    public bool IsDraft { get; set; }

    // Options
    public string Format { get; set; } = StoryDocumentExportFormat.Pdf; // pdf|docx
    public string PaperSize { get; set; } = "A4"; // A4|Letter
    public bool IncludeCover { get; set; } = true;
    public bool IncludeQuizAnswers { get; set; } = false;
    public bool UseMobileImageLayout { get; set; } = true; // If true, render images at ~60% width (mobile-like) to leave space for text.

    // Execution tracking
    public string Status { get; set; } = StoryDocumentExportJobStatus.Queued;
    public int DequeueCount { get; set; }
    public DateTime QueuedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }

    // Output
    public string? OutputBlobPath { get; set; }
    public string? OutputFileName { get; set; }
    public long? OutputSizeBytes { get; set; }
}

public static class StoryDocumentExportJobStatus
{
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}

public static class StoryDocumentExportFormat
{
    public const string Pdf = "pdf";
    public const string Docx = "docx";
}

