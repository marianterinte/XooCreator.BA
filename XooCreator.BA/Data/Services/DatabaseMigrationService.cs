
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace XooCreator.BA.Data.Services;

/// <summary>
/// Robust service for applying database migrations incrementally
/// Uses idempotent SQL operations instead of exception handling
/// </summary>
public class DatabaseMigrationService : IDatabaseMigrationService
{
    private readonly XooDbContext _context;
    private readonly ILogger<DatabaseMigrationService> _logger;

    public DatabaseMigrationService(
        XooDbContext context,
        ILogger<DatabaseMigrationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Ensures the migrations history table exists (idempotent operation)
    /// This is critical for tracking which migrations have been applied
    /// </summary>
    private string GetDefaultSchema()
        => _context.Model.GetDefaultSchema() ?? "public";

    private async Task EnsureSchemaExistsAsync(CancellationToken cancellationToken = default)
    {
        var schema = GetDefaultSchema();
        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                $"CREATE SCHEMA IF NOT EXISTS \"{schema.Replace("\"", "\"\"")}\";",
                cancellationToken);
            _logger.LogDebug("✅ Schema {Schema} ensured", schema);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to ensure schema {Schema} exists", schema);
            throw;
        }
    }

    public async Task EnsureMigrationsHistoryTableExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureSchemaExistsAsync(cancellationToken);
            var schema = GetDefaultSchema();
            var schemaName = $"\"{schema.Replace("\"", "\"\"")}\"";
            // Create migrations history table using idempotent SQL
            // This table is created by EF Core automatically, but we ensure it exists
            // in case the database was created manually or partially migrated
            await _context.Database.ExecuteSqlRawAsync(
                $@"
                    CREATE TABLE IF NOT EXISTS {schemaName}.""__EFMigrationsHistory"" (
                        ""MigrationId"" character varying(150) NOT NULL,
                        ""ProductVersion"" character varying(32) NOT NULL,
                        CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY (""MigrationId"")
                    );
                ",
                cancellationToken);

            _logger.LogDebug("✅ Migrations history table ensured");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to ensure migrations history table exists");
            throw;
        }
    }

    /// <summary>
    /// Gets all migrations that have been applied to the database
    /// Uses EF Core's built-in method which queries the __EFMigrationsHistory table
    /// </summary>
    public async Task<IReadOnlyList<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureMigrationsHistoryTableExistsAsync(cancellationToken);

            // Use EF Core's built-in method to get applied migrations
            // This queries the __EFMigrationsHistory table
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync(cancellationToken);
            return appliedMigrations.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to get applied migrations");
            throw;
        }
    }

    /// <summary>
    /// Gets all migrations that are pending (not yet applied)
    /// </summary>
    public async Task<IReadOnlyList<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureMigrationsHistoryTableExistsAsync(cancellationToken);

            // Use EF Core's built-in method to get pending migrations
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
            return pendingMigrations.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to get pending migrations");
            throw;
        }
    }

    /// <summary>
    /// Applies all pending migrations incrementally
    /// This method is idempotent and can be safely called multiple times
    /// Uses EF Core's MigrateAsync which applies all pending migrations in order
    /// </summary>
    public async Task<bool> ApplyMigrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Ensure migrations history table exists
            await EnsureMigrationsHistoryTableExistsAsync(cancellationToken);

            // Ensure critical columns exist before applying migrations
            await EnsureCriticalColumnsExistAsync(cancellationToken);

            // Get state before applying migrations
            var appliedMigrationsBefore = await GetAppliedMigrationsAsync(cancellationToken);
            var pendingMigrations = await GetPendingMigrationsAsync(cancellationToken);

            if (pendingMigrations.Count == 0)
            {
                _logger.LogInformation("✅ No pending migrations to apply");
                return true;
            }

            _logger.LogInformation("🔄 Found {Count} pending migration(s) to apply: {Migrations}",
                pendingMigrations.Count,
                string.Join(", ", pendingMigrations));

            _logger.LogInformation("📋 Currently applied migrations: {AppliedMigrations}",
                appliedMigrationsBefore.Count > 0
                    ? string.Join(", ", appliedMigrationsBefore)
                    : "none");

            // Apply all pending migrations using EF Core's built-in method
            // This will apply migrations in order and update the history table
            // If migrations use idempotent SQL (IF NOT EXISTS), they can be safely re-run
            await _context.Database.MigrateAsync(cancellationToken);

            // Verify which migrations were applied
            var appliedMigrationsAfter = await GetAppliedMigrationsAsync(cancellationToken);
            var newlyApplied = appliedMigrationsAfter
                .Where(m => !appliedMigrationsBefore.Contains(m, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (newlyApplied.Count > 0)
            {
                _logger.LogInformation("✅ Successfully applied {Count} migration(s): {Migrations}",
                    newlyApplied.Count,
                    string.Join(", ", newlyApplied));
            }

            _logger.LogInformation("🎉 All pending migrations applied successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to apply migrations");

            // Also write to console for Azure log stream visibility
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("❌ MIGRATION FAILURE - DETAILED ERROR INFORMATION:");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine($"Exception Type: {ex.GetType().FullName}");
            Console.WriteLine($"Exception Message: {ex.Message}");

            // Log PostgreSQL-specific error details if available
            if (ex is Npgsql.PostgresException pgEx)
            {
                Console.WriteLine($"PostgreSQL Error Code: {pgEx.SqlState}");
                Console.WriteLine($"PostgreSQL Error Detail: {pgEx.Detail ?? "N/A"}");
                Console.WriteLine($"PostgreSQL Error Hint: {pgEx.Hint ?? "N/A"}");
                Console.WriteLine($"PostgreSQL Error Position: {pgEx.Position}");
                if (!string.IsNullOrWhiteSpace(pgEx.InternalQuery))
                {
                    Console.WriteLine($"PostgreSQL Error Internal Query: {pgEx.InternalQuery}");
                }

                _logger.LogError("PostgreSQL Error Code: {SqlState}", pgEx.SqlState);
                _logger.LogError("PostgreSQL Error Detail: {Detail}", pgEx.Detail ?? "N/A");
                _logger.LogError("PostgreSQL Error Hint: {Hint}", pgEx.Hint ?? "N/A");
                _logger.LogError("PostgreSQL Error Position: {Position}", pgEx.Position);
                if (!string.IsNullOrWhiteSpace(pgEx.InternalQuery))
                {
                    _logger.LogError("PostgreSQL Error Internal Query: {InternalQuery}", pgEx.InternalQuery);
                }
            }

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception Type: {ex.InnerException.GetType().FullName}");
                Console.WriteLine($"Inner Exception Message: {ex.InnerException.Message}");
                if (ex.InnerException is Npgsql.PostgresException innerPgEx)
                {
                    Console.WriteLine($"Inner PostgreSQL Error Code: {innerPgEx.SqlState}");
                    Console.WriteLine($"Inner PostgreSQL Error Detail: {innerPgEx.Detail ?? "N/A"}");
                }

                _logger.LogError("Inner Exception Type: {InnerExceptionType}", ex.InnerException.GetType().FullName);
                _logger.LogError("Inner Exception Message: {InnerExceptionMessage}", ex.InnerException.Message);
                if (ex.InnerException is Npgsql.PostgresException innerPgEx1)
                {
                    _logger.LogError("Inner PostgreSQL Error Code: {SqlState}", innerPgEx1.SqlState);
                    _logger.LogError("Inner PostgreSQL Error Detail: {Detail}", innerPgEx1.Detail ?? "N/A");
                }
            }

            // Log schema information
            var schema = GetDefaultSchema();
            Console.WriteLine($"Target Schema: {schema}");
            _logger.LogError("Target Schema: {Schema}", schema);

            // Log which migrations were applied before the failure
            try
            {
                var appliedMigrations = await GetAppliedMigrationsAsync(cancellationToken);
                var appliedMsg = appliedMigrations.Count > 0
                    ? string.Join(", ", appliedMigrations)
                    : "none";
                Console.WriteLine($"✅ Migrations successfully applied before failure: {appliedMsg}");
                _logger.LogError("✅ Migrations successfully applied before failure: {Migrations}", appliedMsg);

                // Try to identify which migration might have failed
                var pendingMigrations = await GetPendingMigrationsAsync(cancellationToken);
                if (pendingMigrations.Count > 0)
                {
                    var pendingMsg = string.Join(", ", pendingMigrations);
                    Console.WriteLine($"⚠️  Migrations that were NOT applied: {pendingMsg}");
                    Console.WriteLine("💡 The first migration in the list above is likely the one that failed");
                    _logger.LogError("⚠️  Migrations that were NOT applied: {Migrations}", pendingMsg);
                    _logger.LogError("💡 The first migration in the list above is likely the one that failed");
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"⚠️  Could not retrieve migration state: {logEx.Message}");
                _logger.LogWarning(logEx, "Could not retrieve migration state for diagnostic logging");
            }

            // Log full stack trace at Error level so it's always visible
            if (ex.StackTrace != null)
            {
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine("STACK TRACE:");
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("═══════════════════════════════════════════════════════════");

                _logger.LogError("═══════════════════════════════════════════════════════════");
                _logger.LogError("STACK TRACE:");
                _logger.LogError("═══════════════════════════════════════════════════════════");
                _logger.LogError("{StackTrace}", ex.StackTrace);
                _logger.LogError("═══════════════════════════════════════════════════════════");
            }

            return false;
        }
    }

    /// <summary>
    /// Ensures critical columns exist in tables before migrations run
    /// This fixes cases where tables exist but are missing columns from newer migrations
    /// </summary>
    private async Task EnsureCriticalColumnsExistAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var schema = GetDefaultSchema();
            var schemaName = $"\"{schema.Replace("\"", "\"\"")}\"";
            // Ensure ClassicAuthorId exists in StoryDefinitions
            await _context.Database.ExecuteSqlRawAsync($@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE LOWER(table_name) = 'storydefinitions' AND table_schema = '{schema}') THEN
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE LOWER(table_name) = 'storydefinitions' AND LOWER(column_name) = 'classicauthorid' AND table_schema = '{schema}') THEN
                            ALTER TABLE {schemaName}.""StoryDefinitions"" ADD COLUMN ""ClassicAuthorId"" uuid NULL;
                        END IF;
                    END IF;
                END $$;
            ", cancellationToken);

            // Ensure ClassicAuthorId exists in StoryCrafts
            await _context.Database.ExecuteSqlRawAsync($@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE LOWER(table_name) = 'storycrafts' AND table_schema = '{schema}') THEN
                        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE LOWER(table_name) = 'storycrafts' AND LOWER(column_name) = 'classicauthorid' AND table_schema = '{schema}') THEN
                            ALTER TABLE {schemaName}.""StoryCrafts"" ADD COLUMN ""ClassicAuthorId"" uuid NULL;
                        END IF;
                    END IF;
                END $$;
            ", cancellationToken);

            _logger.LogDebug("✅ Ensured critical columns exist");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️  Could not ensure critical columns exist, continuing with migration");
            // Don't throw - this is a best-effort operation
        }
    }
}
