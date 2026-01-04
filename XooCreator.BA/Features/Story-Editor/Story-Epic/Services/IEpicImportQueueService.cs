using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IEpicImportQueueService
{
    /// <summary>
    /// Enqueue an epic import job
    /// </summary>
    Task<Guid> EnqueueImportAsync(
        string originalEpicId,
        string zipBlobPath,
        string zipFileName,
        long zipSizeBytes,
        EpicImportRequest options,
        Guid ownerUserId,
        string requestedByEmail,
        string locale,
        CancellationToken ct);

    /// <summary>
    /// Get the status of an import job
    /// </summary>
    Task<EpicImportJob?> GetJobStatusAsync(Guid jobId, CancellationToken ct);

    /// <summary>
    /// Update job status and phases
    /// </summary>
    Task UpdateJobStatusAsync(Guid jobId, string status, CancellationToken ct);

    /// <summary>
    /// Update a specific phase progress
    /// </summary>
    Task UpdatePhaseProgressAsync(Guid jobId, string phaseName, ImportPhaseProgress progress, CancellationToken ct);

    /// <summary>
    /// Complete the job successfully
    /// </summary>
    Task CompleteJobAsync(Guid jobId, string importedEpicId, EpicIdMappings idMappings, List<string> warnings, CancellationToken ct);

    /// <summary>
    /// Mark job as failed
    /// </summary>
    Task FailJobAsync(Guid jobId, string errorMessage, CancellationToken ct);

    /// <summary>
    /// Get next job to process
    /// </summary>
    Task<EpicImportJob?> DequeueJobAsync(CancellationToken ct);

    /// <summary>
    /// Add warning to job
    /// </summary>
    Task AddWarningAsync(Guid jobId, string warning, CancellationToken ct);
}