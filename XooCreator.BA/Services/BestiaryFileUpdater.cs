using System.Text.Json;
using System.Text.Json.Nodes;
using XooCreator.BA.Data;

namespace XooCreator.BA.Services;

public interface IBestiaryFileUpdater
{
    Task EnsureImageFileNamesAsync(CancellationToken ct = default);
}

public sealed class BestiaryFileUpdater : IBestiaryFileUpdater
{
    public async Task EnsureImageFileNamesAsync(CancellationToken ct = default)
    {
        // Try both output directory and project directory
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var paths = new List<string>();
        foreach (var lc in Data.LanguageCodeExtensions.All())
        {
            paths.Add(Path.Combine(baseDir, "Data", "SeedData", lc.ToFolder(), "discover-bestiary.json"));
        }
        paths.Add(Path.Combine(baseDir, "Data", "SeedData", "discover-bestiary.json"));

        foreach (var path in paths.Distinct())
        {
            if (!File.Exists(path)) continue;
            await UpdateFileAsync(path, ct);
        }
    }

    private static async Task UpdateFileAsync(string path, CancellationToken ct)
    {
        var json = await File.ReadAllTextAsync(path, ct);
        JsonNode? root;
        try
        {
            root = JsonNode.Parse(json);
        }
        catch
        {
            return; // invalid json, skip
        }
        if (root is not JsonArray arr) return;

        bool changed = false;
        foreach (var node in arr)
        {
            if (node is not JsonObject obj) continue;
            if (!obj.ContainsKey("ImageFileName"))
            {
                var combo = obj["Combination"]?.GetValue<string>() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(combo))
                {
                    obj["ImageFileName"] = combo + ".png";
                    changed = true;
                }
            }
        }

        if (changed)
        {
            var opts = new JsonSerializerOptions { WriteIndented = true };
            await File.WriteAllTextAsync(path, arr.ToJsonString(opts), ct);
        }
    }
}


