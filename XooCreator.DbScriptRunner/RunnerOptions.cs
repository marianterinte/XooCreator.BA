using System.Globalization;

namespace XooCreator.DbScriptRunner;

public sealed class RunnerOptions
{
    public string? ConnectionString { get; set; }
    public string? ScriptsPath { get; set; }
    public string? RollbacksPath { get; set; }
    public string Schema { get; set; } = "alchimalia_schema";
    public bool DryRun { get; set; }
    public string? RollbackTarget { get; set; }

    public string VersionsTable => $"{Schema}.schema_versions";
    public bool IsRollback => !string.IsNullOrWhiteSpace(RollbackTarget);

    public void ApplyDefaults()
    {
        Schema = string.IsNullOrWhiteSpace(Schema) ? "alchimalia_schema" : Schema.Trim();
        ScriptsPath = NormalizePath(ScriptsPath, Path.Combine(Directory.GetCurrentDirectory(), "Database", "Scripts"));
        RollbacksPath = NormalizePath(RollbacksPath, Path.Combine(ScriptsPath!, "Rollbacks"));
    }

    public string NormalizeRollbackPrefix()
    {
        if (!IsRollback)
        {
            throw new InvalidOperationException("Rollback prefix requested but --rollback option was not provided.");
        }

        var trimmed = RollbackTarget!.Trim();
        if (trimmed.StartsWith("R", true, CultureInfo.InvariantCulture))
        {
            return trimmed;
        }

        if (trimmed.StartsWith("V", true, CultureInfo.InvariantCulture))
        {
            return $"R{trimmed[1..]}";
        }

        return trimmed;
    }

    private static string NormalizePath(string? provided, string fallback)
    {
        var path = string.IsNullOrWhiteSpace(provided) ? fallback : provided!;
        return Path.GetFullPath(path);
    }
}

