using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace XooCreator.DbScriptRunner;

internal sealed class ScriptRunnerService
{
    private readonly RunnerOptions _options;
    private readonly ILogger<ScriptRunnerService> _logger;
    private readonly SchemaVersionStore _schemaVersionStore;

    public ScriptRunnerService(RunnerOptions options, ILogger<ScriptRunnerService> logger)
    {
        _options = options;
        _logger = logger;
        _schemaVersionStore = new SchemaVersionStore(options);
    }

    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        var scripts = ResolveScripts();
        if (scripts.Count == 0)
        {
            _logger.LogInformation("No scripts found under '{ScriptsPath}'. Nothing to do.", _options.IsRollback ? _options.RollbacksPath : _options.ScriptsPath);
            return 0;
        }

        await using var connection = new NpgsqlConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        var tableExists = await _schemaVersionStore.EnsureTableAsync(connection, createIfMissing: !_options.DryRun, cancellationToken);
        if (!tableExists && _options.DryRun)
        {
            _logger.LogWarning("Versions table '{VersionsTable}' does not exist. Dry-run will assume no scripts have been applied.", _options.VersionsTable);
        }

        foreach (var script in scripts)
        {
            if (_options.DryRun)
            {
                await ReportDryRunAsync(connection, script, tableExists, cancellationToken);
                continue;
            }

            if (!script.IsRollback && tableExists)
            {
                var entry = await _schemaVersionStore.GetEntryAsync(connection, script.Name, cancellationToken);
                if (entry is not null && entry.Checksum == script.Checksum)
                {
                    _logger.LogInformation("Skipping {Script} (already applied).", script.Name);
                    continue;
                }
            }

            await ExecuteScriptAsync(connection, script, cancellationToken);
            tableExists = true;
        }

        return 0;
    }

    private IReadOnlyList<SqlScript> ResolveScripts()
    {
        if (_options.IsRollback)
        {
            var script = ScriptLoader.LoadRollbackScript(_options.RollbacksPath!, _options.NormalizeRollbackPrefix());
            return new[] { script };
        }

        return ScriptLoader.LoadForwardScripts(_options.ScriptsPath!);
    }

    private async Task ReportDryRunAsync(NpgsqlConnection connection, SqlScript script, bool tableExists, CancellationToken cancellationToken)
    {
        if (script.IsRollback)
        {
            _logger.LogInformation("[DRY-RUN] Rollback script {Script} would be executed.", script.Name);
            return;
        }

        if (tableExists)
        {
            var entry = await _schemaVersionStore.GetEntryAsync(connection, script.Name, cancellationToken);
            if (entry is not null && entry.Checksum == script.Checksum)
            {
                _logger.LogInformation("[DRY-RUN] Script {Script} already applied. Would skip.", script.Name);
                return;
            }

            var reason = entry is null ? "new script" : "checksum changed";
            _logger.LogInformation("[DRY-RUN] Script {Script} would run ({Reason}).", script.Name, reason);
            return;
        }

        _logger.LogInformation("[DRY-RUN] Script {Script} would run (versions table missing).", script.Name);
    }

    private async Task ExecuteScriptAsync(NpgsqlConnection connection, SqlScript script, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var status = script.IsRollback ? "RolledBack" : "Succeeded";

        await using var transaction = script.ManagesOwnTransaction ? null : await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await using var command = new NpgsqlCommand(script.Content, connection, transaction);
            await command.ExecuteNonQueryAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            throw;
        }
        finally
        {
            stopwatch.Stop();
        }

        await _schemaVersionStore.UpsertAsync(connection, script, stopwatch.ElapsedMilliseconds, status, cancellationToken);
        _logger.LogInformation("Applied {Script} in {Elapsed:hh\\:mm\\:ss\\.fff}.", script.Name, stopwatch.Elapsed);
    }
}

