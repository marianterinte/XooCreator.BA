using System.Security.Cryptography;
using System.Text;

namespace XooCreator.DbScriptRunner;

internal sealed record SqlScript(
    string Name,
    string FullPath,
    string Content,
    string Checksum,
    bool ManagesOwnTransaction,
    bool IsRollback,
    int Order);

internal static class SqlScriptFactory
{
    public static SqlScript FromFile(string path, bool isRollback)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"SQL script not found at path '{path}'.", path);
        }

        var name = Path.GetFileName(path);
        var content = File.ReadAllText(path, Encoding.UTF8);
        var checksum = ComputeChecksum(content);
        var managesOwnTransaction = DetectManualTransaction(content);
        var order = ExtractOrderToken(name);

        return new SqlScript(name, path, content, checksum, managesOwnTransaction, isRollback, order);
    }

    private static string ComputeChecksum(string content)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(content));
        var builder = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }

    private static bool DetectManualTransaction(string content)
    {
        var normalized = content.ToUpperInvariant();
        var hasBegin = normalized.Contains("BEGIN;") ||
                       normalized.Contains("BEGIN ") ||
                       normalized.Contains("START TRANSACTION");
        var hasCommit = normalized.Contains("COMMIT;");
        return hasBegin && hasCommit;
    }

    private static int ExtractOrderToken(string name)
    {
        var numericPortion = new string(
            name
                .Skip(1)
                .TakeWhile(char.IsDigit)
                .ToArray());

        return int.TryParse(numericPortion, out var value) ? value : int.MaxValue;
    }
}

