using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Common.Exceptions;
using XooCreator.BA.Data;

namespace XooCreator.BA.Common.Services;

/// <summary>
/// Service to handle concurrency control for entity updates
/// </summary>
public interface IConcurrencyService
{
    /// <summary>
    /// Saves changes with concurrency handling
    /// </summary>
    Task<int> SaveChangesWithConcurrencyHandlingAsync(
        XooDbContext context,
        string entityName,
        string entityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes with concurrency handling and custom retry logic
    /// </summary>
    Task<int> SaveChangesWithConcurrencyHandlingAsync(
        XooDbContext context,
        string entityName,
        string entityId,
        Func<Task> onRetry,
        int maxRetries = 3,
        CancellationToken cancellationToken = default);
}

public class ConcurrencyService : IConcurrencyService
{
    public async Task<int> SaveChangesWithConcurrencyHandlingAsync(
        XooDbContext context,
        string entityName,
        string entityId,
        CancellationToken cancellationToken = default)
    {
        return await SaveChangesWithConcurrencyHandlingAsync(
            context,
            entityName,
            entityId,
            onRetry: null,
            maxRetries: 1,
            cancellationToken);
    }

    public async Task<int> SaveChangesWithConcurrencyHandlingAsync(
        XooDbContext context,
        string entityName,
        string entityId,
        Func<Task> onRetry,
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                return await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryCount++;

                if (retryCount >= maxRetries)
                {
                    throw new ConcurrencyException(
                        entityName,
                        entityId,
                        $"Unable to save changes to {entityName} after {maxRetries} attempts. Another user has modified this record.",
                        ex);
                }

                // If retry logic is provided, execute it
                if (onRetry != null)
                {
                    await onRetry();
                }
                else
                {
                    // Default behavior: reload the entity from database
                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync(cancellationToken);
                    }
                }
            }
        }

        // Should not reach here
        return 0;
    }
}