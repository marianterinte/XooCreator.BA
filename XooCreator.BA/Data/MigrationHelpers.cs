using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;

namespace XooCreator.BA.Data;

/// <summary>
/// Helper methods for creating idempotent migrations that can be safely run multiple times
/// </summary>
public static class MigrationHelpers
{
    /// <summary>
    /// Creates a table only if it doesn't already exist (idempotent)
    /// </summary>
    public static void CreateTableIfNotExists(
        MigrationBuilder migrationBuilder,
        string tableName,
        string tableDefinition)
    {
        migrationBuilder.Sql($@"
            CREATE TABLE IF NOT EXISTS ""{tableName}"" (
                {tableDefinition}
            );
        ");
    }

    /// <summary>
    /// Creates an index only if it doesn't already exist (idempotent)
    /// </summary>
    public static void CreateIndexIfNotExists(
        MigrationBuilder migrationBuilder,
        string indexName,
        string tableName,
        string columns,
        bool isUnique = false)
    {
        var uniqueClause = isUnique ? "UNIQUE" : "";
        migrationBuilder.Sql($@"
            CREATE {uniqueClause} INDEX IF NOT EXISTS ""{indexName}"" 
            ON ""{tableName}"" ({columns});
        ");
    }

    /// <summary>
    /// Adds a column only if it doesn't already exist (idempotent)
    /// </summary>
    public static void AddColumnIfNotExists(
        MigrationBuilder migrationBuilder,
        string tableName,
        string columnName,
        string columnDefinition)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 FROM information_schema.columns 
                    WHERE LOWER(table_name) = LOWER('{tableName}') 
                    AND LOWER(column_name) = LOWER('{columnName}')
                    AND table_schema = 'public'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    ADD COLUMN ""{columnName}"" {columnDefinition};
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Drops a table only if it exists (idempotent)
    /// </summary>
    public static void DropTableIfExists(
        MigrationBuilder migrationBuilder,
        string tableName)
    {
        migrationBuilder.Sql($@"DROP TABLE IF EXISTS ""{tableName}"";");
    }

    /// <summary>
    /// Drops an index only if it exists (idempotent)
    /// </summary>
    public static void DropIndexIfExists(
        MigrationBuilder migrationBuilder,
        string indexName)
    {
        migrationBuilder.Sql($@"DROP INDEX IF EXISTS ""{indexName}"";");
    }

    /// <summary>
    /// Inserts data only if it doesn't already exist (idempotent)
    /// Uses ON CONFLICT DO NOTHING for PostgreSQL
    /// </summary>
    /// <param name="migrationBuilder">The migration builder</param>
    /// <param name="tableName">The table name</param>
    /// <param name="conflictColumn">Single column name for conflict resolution (primary key or unique index)</param>
    /// <param name="insertSql">The INSERT SQL statement (columns and VALUES clause)</param>
    public static void InsertDataIfNotExists(
        MigrationBuilder migrationBuilder,
        string tableName,
        string conflictColumn,
        string insertSql)
    {
        migrationBuilder.Sql($@"
            INSERT INTO ""{tableName}"" {insertSql}
            ON CONFLICT (""{conflictColumn}"") DO NOTHING;
        ");
    }

    /// <summary>
    /// Inserts data only if it doesn't already exist (idempotent)
    /// Uses ON CONFLICT DO NOTHING for PostgreSQL with composite unique constraint
    /// </summary>
    /// <param name="migrationBuilder">The migration builder</param>
    /// <param name="tableName">The table name</param>
    /// <param name="conflictColumns">Array of column names for composite unique constraint</param>
    /// <param name="insertSql">The INSERT SQL statement (columns and VALUES clause)</param>
    public static void InsertDataIfNotExists(
        MigrationBuilder migrationBuilder,
        string tableName,
        string[] conflictColumns,
        string insertSql)
    {
        var conflictClause = string.Join(", ", conflictColumns.Select(c => $@"""{c}"""));
        migrationBuilder.Sql($@"
            INSERT INTO ""{tableName}"" {insertSql}
            ON CONFLICT ({conflictClause}) DO NOTHING;
        ");
    }

    /// <summary>
    /// Adds a primary key constraint only if it doesn't already exist (idempotent)
    /// </summary>
    public static void AddPrimaryKeyIfNotExists(
        MigrationBuilder migrationBuilder,
        string constraintName,
        string tableName,
        string columns)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint 
                    WHERE conname = '{constraintName}' 
                    AND contype = 'p'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    ADD CONSTRAINT ""{constraintName}"" PRIMARY KEY ({columns});
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Adds a foreign key constraint only if it doesn't already exist (idempotent)
    /// </summary>
    public static void AddForeignKeyIfNotExists(
        MigrationBuilder migrationBuilder,
        string constraintName,
        string tableName,
        string columns,
        string referencedTableName,
        string referencedColumns,
        string? onDelete = null,
        string? onUpdate = null)
    {
        var onDeleteClause = string.IsNullOrWhiteSpace(onDelete) ? "" : $" ON DELETE {onDelete}";
        var onUpdateClause = string.IsNullOrWhiteSpace(onUpdate) ? "" : $" ON UPDATE {onUpdate}";
        
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint 
                    WHERE conname = '{constraintName}' 
                    AND contype = 'f'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    ADD CONSTRAINT ""{constraintName}"" 
                    FOREIGN KEY ({columns}) 
                    REFERENCES ""{referencedTableName}"" ({referencedColumns}){onDeleteClause}{onUpdateClause};
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Adds a unique constraint only if it doesn't already exist (idempotent)
    /// </summary>
    public static void AddUniqueConstraintIfNotExists(
        MigrationBuilder migrationBuilder,
        string constraintName,
        string tableName,
        string columns)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint 
                    WHERE conname = '{constraintName}' 
                    AND contype = 'u'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    ADD CONSTRAINT ""{constraintName}"" UNIQUE ({columns});
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Adds a check constraint only if it doesn't already exist (idempotent)
    /// </summary>
    public static void AddCheckConstraintIfNotExists(
        MigrationBuilder migrationBuilder,
        string constraintName,
        string tableName,
        string checkExpression)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 FROM pg_constraint 
                    WHERE conname = '{constraintName}' 
                    AND contype = 'c'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    ADD CONSTRAINT ""{constraintName}"" CHECK ({checkExpression});
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Drops a constraint only if it exists (idempotent)
    /// </summary>
    public static void DropConstraintIfExists(
        MigrationBuilder migrationBuilder,
        string constraintName,
        string tableName)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM pg_constraint 
                    WHERE conname = '{constraintName}'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    DROP CONSTRAINT IF EXISTS ""{constraintName}"";
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Drops a column only if it exists (idempotent)
    /// </summary>
    public static void DropColumnIfExists(
        MigrationBuilder migrationBuilder,
        string tableName,
        string columnName)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM information_schema.columns 
                    WHERE LOWER(table_name) = LOWER('{tableName}') 
                    AND LOWER(column_name) = LOWER('{columnName}')
                    AND table_schema = 'public'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    DROP COLUMN ""{columnName}"";
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Renames a column only if it exists (idempotent)
    /// </summary>
    public static void RenameColumnIfExists(
        MigrationBuilder migrationBuilder,
        string tableName,
        string oldColumnName,
        string newColumnName)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM information_schema.columns 
                    WHERE LOWER(table_name) = LOWER('{tableName}') 
                    AND LOWER(column_name) = LOWER('{oldColumnName}')
                    AND table_schema = 'public'
                ) AND NOT EXISTS (
                    SELECT 1 FROM information_schema.columns 
                    WHERE LOWER(table_name) = LOWER('{tableName}') 
                    AND LOWER(column_name) = LOWER('{newColumnName}')
                    AND table_schema = 'public'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    RENAME COLUMN ""{oldColumnName}"" TO ""{newColumnName}"";
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Modifies a column type only if the column exists (idempotent)
    /// </summary>
    public static void AlterColumnTypeIfExists(
        MigrationBuilder migrationBuilder,
        string tableName,
        string columnName,
        string newType)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM information_schema.columns 
                    WHERE LOWER(table_name) = LOWER('{tableName}') 
                    AND LOWER(column_name) = LOWER('{columnName}')
                    AND table_schema = 'public'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    ALTER COLUMN ""{columnName}"" TYPE {newType};
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Sets a default value for a column only if the column exists (idempotent)
    /// </summary>
    public static void SetColumnDefaultIfExists(
        MigrationBuilder migrationBuilder,
        string tableName,
        string columnName,
        string defaultValue)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM information_schema.columns 
                    WHERE LOWER(table_name) = LOWER('{tableName}') 
                    AND LOWER(column_name) = LOWER('{columnName}')
                    AND table_schema = 'public'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    ALTER COLUMN ""{columnName}"" SET DEFAULT {defaultValue};
                END IF;
            END $$;
        ");
    }

    /// <summary>
    /// Drops a default value from a column only if the column exists (idempotent)
    /// </summary>
    public static void DropColumnDefaultIfExists(
        MigrationBuilder migrationBuilder,
        string tableName,
        string columnName)
    {
        migrationBuilder.Sql($@"
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM information_schema.columns 
                    WHERE LOWER(table_name) = LOWER('{tableName}') 
                    AND LOWER(column_name) = LOWER('{columnName}')
                    AND table_schema = 'public'
                ) THEN
                    ALTER TABLE ""{tableName}"" 
                    ALTER COLUMN ""{columnName}"" DROP DEFAULT;
                END IF;
            END $$;
        ");
    }
}

