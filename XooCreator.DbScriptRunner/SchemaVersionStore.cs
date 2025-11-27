using Npgsql;

namespace XooCreator.DbScriptRunner;

internal sealed record SchemaVersionEntry(string ScriptName, string Checksum, string Status);

internal sealed class SchemaVersionStore(RunnerOptions options)
{
    private readonly RunnerOptions _options = options;

    public async Task<bool> EnsureTableAsync(NpgsqlConnection connection, bool createIfMissing, CancellationToken cancellationToken)
    {
        var exists = await TableExistsAsync(connection, cancellationToken);
        if (exists || !createIfMissing)
        {
            return exists;
        }

        var sql = $"""
                   CREATE SCHEMA IF NOT EXISTS {_options.Schema};

                   CREATE TABLE IF NOT EXISTS {_options.VersionsTable}
                   (
                       script_name TEXT PRIMARY KEY,
                       checksum TEXT NOT NULL,
                       executed_at TIMESTAMPTZ NOT NULL,
                       execution_time_ms BIGINT,
                       status TEXT NOT NULL
                   );
                   """;

        await using var cmd = new NpgsqlCommand(sql, connection);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
        return true;
    }

    public async Task<SchemaVersionEntry?> GetEntryAsync(NpgsqlConnection connection, string scriptName, CancellationToken cancellationToken)
    {
        var sql = $"""
                   SELECT script_name, checksum, status
                   FROM {_options.VersionsTable}
                   WHERE script_name = @name;
                   """;

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("name", scriptName);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var name = reader.GetString(0);
        var checksum = reader.GetString(1);
        var status = reader.GetString(2);
        return new SchemaVersionEntry(name, checksum, status);
    }

    public async Task UpsertAsync(NpgsqlConnection connection, SqlScript script, long elapsedMilliseconds, string status, CancellationToken cancellationToken)
    {
        var sql = $"""
                   INSERT INTO {_options.VersionsTable} (script_name, checksum, executed_at, execution_time_ms, status)
                   VALUES (@name, @checksum, @executedAt, @elapsed, @status)
                   ON CONFLICT (script_name) DO UPDATE
                   SET checksum = EXCLUDED.checksum,
                       executed_at = EXCLUDED.executed_at,
                       execution_time_ms = EXCLUDED.execution_time_ms,
                       status = EXCLUDED.status;
                   """;

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("name", script.Name);
        cmd.Parameters.AddWithValue("checksum", script.Checksum);
        cmd.Parameters.AddWithValue("executedAt", DateTimeOffset.UtcNow);
        cmd.Parameters.AddWithValue("elapsed", elapsedMilliseconds);
        cmd.Parameters.AddWithValue("status", status);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task<bool> TableExistsAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        var sql = "SELECT to_regclass(@tableName);";
        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("tableName", _options.VersionsTable);

        var result = await cmd.ExecuteScalarAsync(cancellationToken);
        return result is string { Length: > 0 };
    }
}

