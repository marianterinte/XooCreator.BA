namespace XooCreator.BA.Data.Services;

/// <summary>
/// Service for managing database migrations in a robust, incremental way
/// </summary>
public interface IDatabaseMigrationService
{
    /// <summary>
    /// Applies pending migrations incrementally without recreating the database
    /// Uses idempotent operations to ensure migrations can be safely re-run
    /// </summary>
    Task<bool> ApplyMigrationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of pending migrations that haven't been applied yet
    /// </summary>
    Task<IReadOnlyList<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of all applied migrations
    /// </summary>
    Task<IReadOnlyList<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures the migrations history table exists (idempotent)
    /// </summary>
    Task EnsureMigrationsHistoryTableExistsAsync(CancellationToken cancellationToken = default);
}

