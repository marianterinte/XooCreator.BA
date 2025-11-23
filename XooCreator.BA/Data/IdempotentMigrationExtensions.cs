using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;

namespace XooCreator.BA.Data;

/// <summary>
/// Extension methods for making migrations idempotent
/// Note: For creating tables, use MigrationHelpers.CreateTableIfNotExists with SQL instead
/// </summary>
public static class IdempotentMigrationExtensions
{
    /// <summary>
    /// Executes a migration command and ignores "already exists" errors
    /// This makes migrations idempotent - they can be run multiple times safely
    /// </summary>
    public static void ExecuteIdempotent(this MigrationBuilder migrationBuilder, Action action)
    {
        try
        {
            action();
        }
        catch (PostgresException ex) when (ex.SqlState == "42P07") // relation already exists
        {
            // Table/index already exists, which is fine - migration is idempotent
            Console.WriteLine($"⚠️  Table/index already exists, skipping: {ex.MessageText}");
        }
        catch (PostgresException ex) when (ex.SqlState == "42710") // duplicate object
        {
            // Constraint/index already exists, which is fine
            Console.WriteLine($"⚠️  Constraint/index already exists, skipping: {ex.MessageText}");
        }
    }

    // Note: For creating indexes idempotently, use MigrationHelpers.CreateIndexIfNotExists with SQL instead
    // This extension method is kept for reference but CreateIndex signature varies by EF Core version
}

