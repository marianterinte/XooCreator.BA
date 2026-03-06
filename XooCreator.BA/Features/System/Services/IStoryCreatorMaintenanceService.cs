namespace XooCreator.BA.Features.System.Services;

/// <summary>
/// Provides the Story Creator maintenance (disabled) flag for use by endpoints and workers.
/// When disabled, job submission should be rejected and workers should skip processing.
/// </summary>
public interface IStoryCreatorMaintenanceService
{
    /// <summary>
    /// Returns true if Story Creator is in maintenance mode (editing and jobs disabled).
    /// </summary>
    Task<bool> IsStoryCreatorDisabledAsync(CancellationToken ct = default);

    /// <summary>
    /// Invalidates the cached value. Call after admin updates the flag so the new value is visible within the cache TTL.
    /// </summary>
    void InvalidateCache();
}
