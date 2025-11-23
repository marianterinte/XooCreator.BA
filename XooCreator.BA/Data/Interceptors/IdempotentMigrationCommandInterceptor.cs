using System.Data.Common;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Data.Interceptors;

/// <summary>
/// Interceptor that automatically makes migration SQL commands idempotent
/// Transforms CREATE TABLE, CREATE INDEX, ALTER TABLE ADD CONSTRAINT, etc. to use IF NOT EXISTS
/// </summary>
public class IdempotentMigrationCommandInterceptor : DbCommandInterceptor
{
    private readonly ILogger<IdempotentMigrationCommandInterceptor>? _logger;

    public IdempotentMigrationCommandInterceptor(ILogger<IdempotentMigrationCommandInterceptor>? logger = null)
    {
        _logger = logger;
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        if (IsMigrationCommand(eventData))
        {
            command.CommandText = MakeIdempotent(command.CommandText);
        }

        return base.NonQueryExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (IsMigrationCommand(eventData))
        {
            command.CommandText = MakeIdempotent(command.CommandText);
        }

        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    private static bool IsMigrationCommand(CommandEventData eventData)
    {
        // Check if this is a migration command
        // We check the command text for migration-specific patterns
        // and also check the context type
        var commandText = eventData.Command.CommandText ?? string.Empty;
        var isMigrationPattern = Regex.IsMatch(
            commandText,
            @"(CREATE\s+(TABLE|INDEX|SEQUENCE|TYPE)|ALTER\s+TABLE.*ADD\s+(CONSTRAINT|COLUMN)|INSERT\s+INTO)",
            RegexOptions.IgnoreCase);

        return isMigrationPattern
               || eventData.Context?.GetType().Name.Contains("Migration", StringComparison.OrdinalIgnoreCase) == true
               || (eventData.CommandSource != null && eventData.CommandSource.ToString().Contains("Migration", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Transforms SQL commands to be idempotent by adding IF NOT EXISTS clauses
    /// </summary>
    private string MakeIdempotent(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return sql;

        var originalSql = sql;
        var transformedSql = sql;

        // 1. CREATE TABLE -> Transform to handle existing tables and add missing columns
        transformedSql = TransformCreateTable(transformedSql);

        // 2. CREATE INDEX -> Wrap in DO block to check if columns exist
        transformedSql = TransformCreateIndex(transformedSql);

        // 4. ALTER TABLE ... ADD CONSTRAINT ... PRIMARY KEY -> Check if exists first
        // This is more complex, we'll wrap it in a DO block
        transformedSql = TransformAddConstraint(transformedSql, "PRIMARY KEY", "p");
        transformedSql = TransformAddConstraint(transformedSql, "FOREIGN KEY", "f");
        transformedSql = TransformAddConstraint(transformedSql, "UNIQUE", "u");
        transformedSql = TransformAddConstraint(transformedSql, "CHECK", "c");

        // 5. ALTER TABLE ... ADD COLUMN -> ALTER TABLE ... ADD COLUMN IF NOT EXISTS (PostgreSQL 9.6+)
        transformedSql = Regex.Replace(
            transformedSql,
            @"ALTER\s+TABLE\s+(""[^""]+""|\w+)\s+ADD\s+COLUMN\s+(""[^""]+""|\w+)",
            match => match.Value.Contains("IF NOT EXISTS", StringComparison.OrdinalIgnoreCase)
                ? match.Value
                : match.Value.Replace("ADD COLUMN", "ADD COLUMN IF NOT EXISTS", StringComparison.OrdinalIgnoreCase),
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        // 6. CREATE SEQUENCE -> CREATE SEQUENCE IF NOT EXISTS
        transformedSql = Regex.Replace(
            transformedSql,
            @"CREATE\s+SEQUENCE\s+(""[^""]+""|\w+)",
            match => match.Value.Contains("IF NOT EXISTS", StringComparison.OrdinalIgnoreCase)
                ? match.Value
                : match.Value.Replace("CREATE SEQUENCE", "CREATE SEQUENCE IF NOT EXISTS", StringComparison.OrdinalIgnoreCase),
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        // 7. CREATE TYPE -> CREATE TYPE IF NOT EXISTS
        transformedSql = Regex.Replace(
            transformedSql,
            @"CREATE\s+TYPE\s+(""[^""]+""|\w+)",
            match => match.Value.Contains("IF NOT EXISTS", StringComparison.OrdinalIgnoreCase)
                ? match.Value
                : match.Value.Replace("CREATE TYPE", "CREATE TYPE IF NOT EXISTS", StringComparison.OrdinalIgnoreCase),
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        // 8. INSERT INTO -> INSERT INTO ... ON CONFLICT DO NOTHING
        // This handles INSERT statements from migrations (seed data)
        transformedSql = TransformInsertStatements(transformedSql);

        // 9. Ensure ClassicAuthorId column exists in StoryDefinitions and StoryCrafts
        // This is a specific fix for the current migration issue
        transformedSql = EnsureClassicAuthorIdColumn(transformedSql);

        // Log transformation if SQL was changed
        if (transformedSql != originalSql && _logger != null)
        {
            _logger.LogInformation("🔄 Transformed SQL command to be idempotent");
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Original: {OriginalSql}", originalSql);
                _logger.LogTrace("Transformed: {TransformedSql}", transformedSql);
            }
        }

        return transformedSql;
    }

    /// <summary>
    /// Transforms ALTER TABLE ADD CONSTRAINT to be idempotent
    /// Wraps it in a DO block that checks if constraint exists
    /// </summary>
    private string TransformAddConstraint(string sql, string constraintType, string constraintTypeCode)
    {
        // Pattern: ALTER TABLE "TableName" ADD CONSTRAINT "ConstraintName" ... (full statement to semicolon)
        var pattern = $@"(ALTER\s+TABLE\s+(""[^""]+""|\w+)\s+ADD\s+CONSTRAINT\s+(""[^""]+""|\w+)\s+{constraintType}[^;]+;)";
        
        return Regex.Replace(
            sql,
            pattern,
            match =>
            {
                var fullMatch = match.Value;
                
                // Check if already wrapped in DO block
                var contextStart = Math.Max(0, match.Index - 200);
                var contextLength = Math.Min(400, sql.Length - contextStart);
                var context = sql.Substring(contextStart, contextLength);
                
                if (context.Contains("DO $$", StringComparison.OrdinalIgnoreCase))
                {
                    return fullMatch;
                }

                // Extract constraint name
                var constraintNameMatch = Regex.Match(fullMatch, @"CONSTRAINT\s+(""[^""]+""|\w+)", RegexOptions.IgnoreCase);
                if (!constraintNameMatch.Success)
                    return fullMatch;

                var constraintName = constraintNameMatch.Groups[1].Value.Trim('"');
                
                // Build idempotent version - wrap in DO block
                var idempotentSql = $@"
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = '{constraintName}' 
        AND contype = '{constraintTypeCode}'
    ) THEN
        {fullMatch.Trim()}
    END IF;
END $$;";

                return idempotentSql;
            },
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
    }

    /// <summary>
    /// Transforms INSERT statements to be idempotent by adding ON CONFLICT DO NOTHING
    /// Attempts to detect the primary key column automatically
    /// </summary>
    private string TransformInsertStatements(string sql)
    {
        var result = sql;
        var insertPattern = @"INSERT\s+INTO\s+(""[^""]+""|\w+)\s*\(([^)]+)\)\s+VALUES";
        
        // Find all INSERT statements
        var matches = Regex.Matches(sql, insertPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        
        // Process matches in reverse order to preserve indices
        for (var i = matches.Count - 1; i >= 0; i--)
        {
            var match = matches[i];
            var matchStart = match.Index;
            
            // Find the full INSERT statement (from INSERT to semicolon or end of statement)
            var fullStatementEnd = sql.IndexOf(';', matchStart + match.Length);
            if (fullStatementEnd < 0)
                fullStatementEnd = sql.Length;
            
            var fullStatement = sql.Substring(matchStart, fullStatementEnd - matchStart);
            
            // Check if already has ON CONFLICT
            if (fullStatement.Contains("ON CONFLICT", StringComparison.OrdinalIgnoreCase))
            {
                continue; // Skip, already idempotent
            }

            // Extract table name and columns
            var tableName = match.Groups[1].Value;
            var columnsStr = match.Groups[2].Value;
            
            // Parse columns - they might be quoted or not, separated by commas
            var columns = Regex.Matches(columnsStr, @"(""[^""]+""|\w+)")
                .Cast<Match>()
                .Select(m => m.Value.Trim('"'))
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList();

            if (columns.Count == 0)
            {
                continue; // Can't determine columns, skip transformation
            }

            // Try to detect primary key column
            var tableNameWithoutQuotes = tableName.Trim('"');
            var commonPkNames = new[] 
            { 
                "Id", 
                "id", 
                "ID",
                tableNameWithoutQuotes + "Id",
                tableNameWithoutQuotes + "_Id"
            };
            
            var primaryKeyColumn = columns.FirstOrDefault(c => 
                commonPkNames.Any(pk => c.Equals(pk, StringComparison.OrdinalIgnoreCase)))
                ?? columns[0]; // Fallback to first column

            // Build idempotent version with ON CONFLICT
            var quotedPkColumn = primaryKeyColumn.Contains('"') 
                ? primaryKeyColumn 
                : $@"""{primaryKeyColumn}""";

            // Find where to insert ON CONFLICT (before semicolon or at end)
            var valuesIndex = fullStatement.IndexOf("VALUES", StringComparison.OrdinalIgnoreCase);
            if (valuesIndex < 0)
                continue;

            // Find the end of VALUES clause by tracking parentheses
            var valuesStart = valuesIndex + 6; // Length of "VALUES"
            var parenDepth = 0;
            var inQuotes = false;
            var quoteChar = '\0';
            var valuesEnd = valuesStart;
            
            for (var j = valuesStart; j < fullStatement.Length; j++)
            {
                var ch = fullStatement[j];
                
                // Handle quotes
                if ((ch == '"' || ch == '\'') && (j == 0 || fullStatement[j - 1] != '\\'))
                {
                    if (!inQuotes)
                    {
                        inQuotes = true;
                        quoteChar = ch;
                    }
                    else if (ch == quoteChar)
                    {
                        inQuotes = false;
                        quoteChar = '\0';
                    }
                    continue;
                }
                
                if (inQuotes)
                    continue;
                
                // Track parentheses
                if (ch == '(')
                    parenDepth++;
                else if (ch == ')')
                {
                    parenDepth--;
                    if (parenDepth < 0)
                    {
                        valuesEnd = j + 1;
                        break;
                    }
                }
                // End of statement
                else if (parenDepth == 0 && ch == ';')
                {
                    valuesEnd = j;
                    break;
                }
            }
            
            if (valuesEnd <= valuesStart)
                valuesEnd = fullStatement.Length;

            var beforeConflict = fullStatement.Substring(0, valuesEnd).TrimEnd();
            var afterConflict = fullStatement.Substring(valuesEnd);
            
            // Use ON CONFLICT DO NOTHING without specifying a column
            // This will handle conflicts on ANY unique constraint or primary key
            // This is more robust than specifying a single column, as it covers:
            // - Primary key conflicts
            // - Unique index conflicts (like IX_HeroClickMessages_HeroId)
            // - Any other unique constraints
            var idempotentSql = $"{beforeConflict} ON CONFLICT DO NOTHING{afterConflict}";

            // Replace the full statement in the result
            result = result.Substring(0, matchStart) + idempotentSql + result.Substring(matchStart + fullStatement.Length);

            if (_logger != null)
            {
                _logger.LogDebug("🔄 Transformed INSERT statement to be idempotent (handles all unique constraints) for table: {TableName}", 
                    tableNameWithoutQuotes);
            }
        }
        
        return result;
    }

    /// <summary>
    /// Transforms CREATE INDEX to check if columns exist before creating index
    /// </summary>
    private string TransformCreateIndex(string sql)
    {
        // Pattern: CREATE [UNIQUE] INDEX "IndexName" ON "TableName" (columns)
        var indexPattern = @"CREATE\s+(UNIQUE\s+)?INDEX\s+(""[^""]+""|\w+)\s+ON\s+(""[^""]+""|\w+)\s*\(([^)]+)\)";
        
        return Regex.Replace(
            sql,
            indexPattern,
            match =>
            {
                var fullMatch = match.Value;
                
                // Check if already wrapped in DO block or has IF NOT EXISTS
                if (fullMatch.Contains("DO $$", StringComparison.OrdinalIgnoreCase) || 
                    fullMatch.Contains("IF NOT EXISTS", StringComparison.OrdinalIgnoreCase))
                {
                    return fullMatch;
                }

                var uniqueClause = match.Groups[1].Success ? "UNIQUE " : "";
                var indexName = match.Groups[2].Value.Trim('"');
                var tableName = match.Groups[3].Value;
                var columnsStr = match.Groups[4].Value;
                
                // Parse columns
                var columns = Regex.Matches(columnsStr, @"(""[^""]+""|\w+)")
                    .Cast<Match>()
                    .Select(m => m.Value.Trim('"'))
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .ToList();

                if (columns.Count == 0)
                    return fullMatch;

                // Build idempotent version - check if all columns exist
                var tableNameWithoutQuotes = tableName.Trim('"');
                var columnChecks = string.Join(" AND ", columns.Select(col => 
                    $@"EXISTS (SELECT 1 FROM information_schema.columns WHERE LOWER(table_name) = LOWER('{tableNameWithoutQuotes}') AND LOWER(column_name) = LOWER('{col}') AND table_schema = 'public')"));
                
                var idempotentSql = $@"
DO $$
BEGIN
    IF {columnChecks} THEN
        IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE indexname = '{indexName}' AND schemaname = 'public') THEN
            CREATE {uniqueClause}INDEX ""{indexName}"" ON {tableName} ({columnsStr});
        END IF;
    END IF;
END $$;";

                return idempotentSql;
            },
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
    }

    /// <summary>
    /// Transforms CREATE TABLE to handle existing tables and add missing columns
    /// </summary>
    private string TransformCreateTable(string sql)
    {
        // Pattern: CREATE TABLE "TableName" (column definitions...)
        // We need to match the full CREATE TABLE statement including all columns and constraints
        var tablePattern = @"CREATE\s+TABLE\s+(""[^""]+""|\w+)\s*\(([\s\S]*?)\)\s*;";
        
        return Regex.Replace(
            sql,
            tablePattern,
            match =>
            {
                var fullMatch = match.Value;
                
                // Check if already has IF NOT EXISTS or is wrapped in DO block
                if (fullMatch.Contains("IF NOT EXISTS", StringComparison.OrdinalIgnoreCase) ||
                    fullMatch.Contains("DO $$", StringComparison.OrdinalIgnoreCase))
                {
                    return fullMatch;
                }

                var tableName = match.Groups[1].Value;
                var tableDefinition = match.Groups[2].Value;
                var tableNameWithoutQuotes = tableName.Trim('"');
                
                // Extract column definitions - improved pattern to handle quoted names, types with parentheses, etc.
                var columnAdditions = new List<string>();
                var remainingDef = tableDefinition;
                var depth = 0;
                var inQuotes = false;
                var quoteChar = '\0';
                var currentCol = new System.Text.StringBuilder();
                var columns = new List<(string name, string definition)>();
                
                for (var i = 0; i < remainingDef.Length; i++)
                {
                    var ch = remainingDef[i];
                    
                    // Handle quotes
                    if ((ch == '"' || ch == '\'') && (i == 0 || remainingDef[i - 1] != '\\'))
                    {
                        if (!inQuotes)
                        {
                            inQuotes = true;
                            quoteChar = ch;
                        }
                        else if (ch == quoteChar)
                        {
                            inQuotes = false;
                            quoteChar = '\0';
                        }
                        currentCol.Append(ch);
                        continue;
                    }
                    
                    if (inQuotes)
                    {
                        currentCol.Append(ch);
                        continue;
                    }
                    
                    // Track parentheses for nested structures
                    if (ch == '(')
                        depth++;
                    else if (ch == ')')
                        depth--;
                    else if (depth == 0 && ch == ',')
                    {
                        var colStr = currentCol.ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(colStr) && !colStr.StartsWith("CONSTRAINT", StringComparison.OrdinalIgnoreCase))
                        {
                            // Parse column name and definition
                            var colParts = Regex.Match(colStr, @"^(""[^""]+""|\w+)\s+(.+)$");
                            if (colParts.Success)
                            {
                                var colName = colParts.Groups[1].Value.Trim('"');
                                var colDef = colParts.Groups[2].Value.Trim();
                                columns.Add((colName, colDef));
                            }
                        }
                        currentCol.Clear();
                        continue;
                    }
                    
                    currentCol.Append(ch);
                }
                
                // Handle last column
                var lastColStr = currentCol.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(lastColStr) && !lastColStr.StartsWith("CONSTRAINT", StringComparison.OrdinalIgnoreCase))
                {
                    var colParts = Regex.Match(lastColStr, @"^(""[^""]+""|\w+)\s+(.+)$");
                    if (colParts.Success)
                    {
                        var colName = colParts.Groups[1].Value.Trim('"');
                        var colDef = colParts.Groups[2].Value.Trim();
                        columns.Add((colName, colDef));
                    }
                }
                
                // Build column addition statements
                foreach (var (colName, colDef) in columns)
                {
                    columnAdditions.Add($@"
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.columns 
            WHERE LOWER(table_name) = LOWER('{tableNameWithoutQuotes}') 
            AND LOWER(column_name) = LOWER('{colName}') 
            AND table_schema = 'public'
        ) THEN
            ALTER TABLE {tableName} ADD COLUMN ""{colName}"" {colDef};
        END IF;");
                }

                if (columnAdditions.Count == 0)
                {
                    return fullMatch.Replace("CREATE TABLE", "CREATE TABLE IF NOT EXISTS", StringComparison.OrdinalIgnoreCase);
                }

                // Build idempotent version
                var idempotentSql = $@"
DO $$
BEGIN
    -- Create table if it doesn't exist
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE LOWER(table_name) = LOWER('{tableNameWithoutQuotes}') 
        AND table_schema = 'public'
    ) THEN
        CREATE TABLE {tableName} ({tableDefinition});
    ELSE
        -- Table exists, add missing columns
{string.Join("", columnAdditions)}
    END IF;
END $$;";

                if (_logger != null)
                {
                    _logger.LogDebug("🔄 Transformed CREATE TABLE for {TableName} - will add {ColumnCount} columns if table exists", 
                        tableNameWithoutQuotes, columnAdditions.Count);
                }

                return idempotentSql;
            },
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
    }

    /// <summary>
    /// Ensures ClassicAuthorId column exists in StoryDefinitions and StoryCrafts tables
    /// This is a specific fix for migration compatibility
    /// </summary>
    private string EnsureClassicAuthorIdColumn(string sql)
    {
        // Only add this check if we're dealing with StoryDefinitions or StoryCrafts or ClassicAuthorId
        if (!sql.Contains("StoryDefinitions", StringComparison.OrdinalIgnoreCase) && 
            !sql.Contains("StoryCrafts", StringComparison.OrdinalIgnoreCase) &&
            !sql.Contains("ClassicAuthorId", StringComparison.OrdinalIgnoreCase))
        {
            return sql;
        }

        var ensureColumns = @"
DO $$
BEGIN
    -- Ensure ClassicAuthorId exists in StoryDefinitions
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE LOWER(table_name) = 'storydefinitions' AND table_schema = 'public') THEN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE LOWER(table_name) = 'storydefinitions' AND LOWER(column_name) = 'classicauthorid' AND table_schema = 'public') THEN
            ALTER TABLE ""StoryDefinitions"" ADD COLUMN ""ClassicAuthorId"" uuid NULL;
        END IF;
    END IF;
    
    -- Ensure ClassicAuthorId exists in StoryCrafts
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE LOWER(table_name) = 'storycrafts' AND table_schema = 'public') THEN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE LOWER(table_name) = 'storycrafts' AND LOWER(column_name) = 'classicauthorid' AND table_schema = 'public') THEN
            ALTER TABLE ""StoryCrafts"" ADD COLUMN ""ClassicAuthorId"" uuid NULL;
        END IF;
    END IF;
END $$;";

        // Prepend the ensure statement before the main SQL
        return ensureColumns + "\n" + sql;
    }
}

