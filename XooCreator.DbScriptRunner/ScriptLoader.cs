namespace XooCreator.DbScriptRunner;

public static class ScriptLoader
{
    public static IReadOnlyList<SqlScript> LoadForwardScripts(string directory)
    {
        GuardDirectory(directory);

        return Directory
            .EnumerateFiles(directory, "V*.sql", SearchOption.TopDirectoryOnly)
            .Select(path => SqlScriptFactory.FromFile(path, isRollback: false))
            .OrderBy(script => script.Order)
            .ThenBy(script => script.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static SqlScript LoadRollbackScript(string directory, string prefix)
    {
        GuardDirectory(directory);

        var file = Directory
            .EnumerateFiles(directory, $"{prefix}*.sql", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        if (file is null)
        {
            throw new FileNotFoundException($"Rollback script matching '{prefix}' not found under '{directory}'.");
        }

        return SqlScriptFactory.FromFile(file, isRollback: true);
    }

    private static void GuardDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"Directory '{directory}' does not exist.");
        }
    }
}

