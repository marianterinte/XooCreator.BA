using System.Text.Json;
using System.Text.Json.Nodes;

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
        var paths = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "discover-bestiary.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "discover-bestiary.json")
        };

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


