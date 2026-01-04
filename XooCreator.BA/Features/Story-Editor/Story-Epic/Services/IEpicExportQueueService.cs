using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IEpicExportQueueService
{
    /// <summary>
    /// Enqueue an epic export job
    /// </summary>
    Task<Guid> EnqueueExportAsync(string epicId, EpicExportRequest options, bool isDraft, Guid ownerUserId, string requestedByEmail, string locale, CancellationToken ct);

    /// <summary>
    /// Get the status of an export job
    /// </summary>
    Task<EpicExportJob?> GetJobStatusAsync(Guid jobId, CancellationToken ct);

    /// <summary>
    /// Update job status and progress
    /// </summary>
    Task UpdateJobProgressAsync(Guid jobId, string status, string? currentPhase = null, Action<EpicExportJob>? updateAction = null, CancellationToken ct = default);

    /// <summary>
    /// Mark job as completed
    /// </summary>
    Task CompleteJobAsync(Guid jobId, string zipBlobPath, string zipFileName, long zipSizeBytes, CancellationToken ct);

    /// <summary>
    /// Mark job as failed
    /// </summary>
    Task FailJobAsync(Guid jobId, string errorMessage, CancellationToken ct);

    /// <summary>
    /// Get next job to process
    /// </summary>
    Task<EpicExportJob?> DequeueJobAsync(CancellationToken ct);

    /// <summary>
    /// Generate download URL for completed export
    /// </summary>
    Task<string?> GetDownloadUrlAsync(EpicExportJob job, CancellationToken ct);
}